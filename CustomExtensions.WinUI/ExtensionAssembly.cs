using Microsoft.UI.Xaml.Markup;

using System.Diagnostics;
using System.Reflection;

using ResourceManager = Windows.ApplicationModel.Resources.Core.ResourceManager;
using StorageFile = Windows.Storage.StorageFile;

namespace CustomExtensions.WinUI;

internal partial class ExtensionAssembly : IExtensionAssembly
{
	public Assembly ForeignAssembly { get; }

	private readonly string ForeignAssemblyDir;
	private readonly string ForeignAssemblyName;
	private bool? IsHotReloadAvailable;
	private DisposableCollection Disposables = new();
	private bool IsDisposed;

	internal ExtensionAssembly(Assembly assembly)
	{
		ForeignAssembly = assembly;
		ForeignAssemblyDir = Path.GetDirectoryName(ForeignAssembly.Location.AssertDefined()).AssertDefined();
		ForeignAssemblyName = ForeignAssembly.GetName().Name.AssertDefined();
	}

	public async Task LoadAsync()
	{
		if (IsDisposed)
			throw new ObjectDisposedException(nameof(ExtensionAssembly));

		RegisterXamlTypeMetadataProviders();
		await LoadResources();
	}

	private async Task LoadResources()
	{
		FileInfo resourcePriFileInfo = new(Path.Combine(ForeignAssemblyDir, "resources.pri"));
		if (!resourcePriFileInfo.Exists)
		{
			resourcePriFileInfo = new(Path.Combine(ForeignAssemblyDir, $"{ForeignAssemblyName}.pri"));
		}

		if (!resourcePriFileInfo.Exists)
			return;

		StorageFile file = await StorageFile.GetFileFromPathAsync(resourcePriFileInfo.FullName);
		ResourceManager.Current.LoadPriFiles(new [] { file });
	}

	private void RegisterXamlTypeMetadataProviders()
	{
		if (IsDisposed)
			throw new ObjectDisposedException(nameof(ExtensionAssembly));

		Disposables.AddRange(ForeignAssembly.ExportedTypes
					.Where(type => type.IsAssignableTo(typeof(IXamlMetadataProvider)))
					.Select(metadataType => (Activator.CreateInstance(metadataType) as IXamlMetadataProvider).AssertDefined())
					.Select(ApplicationExtensionHost.Current.RegisterXamlTypeMetadataProvider));
	}

	public bool TryEnableHotReload()
	{
		if (IsHotReloadAvailable.HasValue)
			return IsHotReloadAvailable.Value;

		if (ForeignAssemblyDir == HostingProcessDir)
		{
			Trace.TraceWarning($"HotReload(Debug) : Output directory for {ForeignAssembly.FullName} appears to be in the same location as the application directory. HotReload may not function in this environment.");
			IsHotReloadAvailable = false;
			return false;
		}

		// NB: this assumes all your resources exist under the current assembly name
		// this won't be true for nested dependencies or the like, so they will need to 
		// enable the same capabilities or they may crash when using hot reload
		string assemblyResDir = Path.Combine(ForeignAssemblyDir, ForeignAssemblyName);
		if (!Directory.Exists(assemblyResDir))
		{
			Trace.TraceError($"HotReload(Debug) : Cannot enable hot reload for {ForeignAssembly.FullName} because {assemblyResDir} does not exist on the system");
			IsHotReloadAvailable = false;
			return false;
		}
		string debugTargetResDir = Path.Combine(HostingProcessDir, ForeignAssemblyName);
		DirectoryInfo debugTargetResDirInfo = new(debugTargetResDir);
		if (debugTargetResDirInfo.Exists)
		{
			if (!debugTargetResDirInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
			{
				Trace.TraceError($"HotReload(Debug) : Cannot enable hot reload for {ForeignAssembly.FullName} because {debugTargetResDir} already exists as a non-symbolic linked directory");
				IsHotReloadAvailable = false;
				return false;
			}
			Directory.Delete(debugTargetResDir, recursive: true);
		}
		Directory.CreateSymbolicLink(debugTargetResDir, assemblyResDir);
		IsHotReloadAvailable = true;
		return true;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			if (disposing)
			{
				Disposables?.Dispose();
			}

			IsDisposed = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
