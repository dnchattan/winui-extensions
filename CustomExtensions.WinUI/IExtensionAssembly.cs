using System.Runtime.CompilerServices;

namespace CustomExtensions.WinUI
{
	public interface IExtensionAssembly : IDisposable
	{
		Task LoadAsync();
		bool TryEnableHotReload();
		Uri LocateResource(object component, [CallerFilePath] string callerFilePath = "");
	}
}
