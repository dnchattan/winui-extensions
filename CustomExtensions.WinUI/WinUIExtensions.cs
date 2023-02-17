using Microsoft.UI.Xaml;
using WinRT;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Runtime.CompilerServices;

namespace CustomExtensions.WinUI;

public static class WinUIExtensions
{
    public static void LoadComponent<T>(this T component, ref bool contentLoaded, [CallerFilePath] string callerFilePath = "") where T : IWinRTObject
    {
        if (contentLoaded)
            return;

        contentLoaded = true;

        Uri resourceLocator = ApplicationExtensionHost.Current.LocateResource(component, callerFilePath);
		Application.LoadComponent(component, resourceLocator, ComponentResourceLocation.Nested);
    }
}
