using Microsoft.UI.Xaml;
using WinRT;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Runtime.CompilerServices;

namespace WinUI.Extensions;

public static class WinUIExtensions
{
	public static void LoadComponent<T>(this T component, ref bool contentLoaded, [CallerFilePath] string callerFilePath = "") where T : IWinRTObject
	{
		if (contentLoaded)
			return;

		contentLoaded = true;

		ExtensionAssembly extensionAsm = ExtensionAssembly.FromAssembly(component.GetType().Assembly);
		Uri resourceLocator = extensionAsm.LocateResource(component, callerFilePath);
		Application.LoadComponent(component, resourceLocator, ComponentResourceLocation.Nested);
	}
}
