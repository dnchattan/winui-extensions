using Microsoft.UI.Xaml.Markup;

using System.Reflection;
using System.Runtime.CompilerServices;

using WinRT;

namespace CustomExtensions.WinUI
{
	public interface IApplicationExtensionHost
	{
		Task<IExtensionAssembly> LoadExtensionAsync(Assembly assembly);
		IDisposable RegisterXamlTypeMetadataProvider(IXamlMetadataProvider provider);
		Uri LocateResource(object component, [CallerFilePath] string callerFilePath = "");
	}
}
