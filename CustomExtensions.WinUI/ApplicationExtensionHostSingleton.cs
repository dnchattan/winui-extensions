using Microsoft.UI.Xaml;

using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

using WinRT;

namespace CustomExtensions.WinUI;

internal partial class ApplicationExtensionHostSingleton<T> : ApplicationExtensionHostSingletonBase, IApplicationExtensionHost where T : Application
{
	private readonly T Application;
	private readonly ConcurrentDictionary<Assembly, IExtensionAssembly> Assemblies = new();

	public ApplicationExtensionHostSingleton(T application)
		: base()
	{
		Application = application;
	}

	public async Task<IExtensionAssembly> LoadExtensionAsync(Assembly assembly)
	{
		IExtensionAssembly asm = GetExtensionAssembly(assembly);
		await asm.LoadAsync();
		return asm;
	}

	public Uri LocateResource(object component, [CallerFilePath] string callerFilePath = "")
	{
		IExtensionAssembly extensionAsm = GetExtensionAssembly(component.GetType().Assembly);
		return extensionAsm.LocateResource(component, callerFilePath);
	}


	private IExtensionAssembly GetExtensionAssembly(Assembly assembly)
	{
		return Assemblies.GetOrAdd(assembly, asm => new ExtensionAssembly(asm));
	}
}
