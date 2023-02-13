using System.Diagnostics;
using System.Reflection;

namespace WinUI.Extensions;

public partial class ExtensionAssembly
{
	public Assembly ForeignAssembly { get; }
	private readonly string ForeignAssemblyDir;
	private readonly string ForeignAssemblyName;
	private bool? IsHotReloadAvailable;

	private ExtensionAssembly(Assembly assembly)
	{
		ForeignAssembly = assembly;
		ForeignAssemblyDir = Path.GetDirectoryName(ForeignAssembly.Location.AssertDefined()).AssertDefined();
		ForeignAssemblyName = ForeignAssembly.GetName().Name.AssertDefined();
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

}
