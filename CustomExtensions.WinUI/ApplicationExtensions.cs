using Microsoft.UI.Xaml;

using System.Reflection;

namespace CustomExtensions.WinUI;

public static class ApplicationExtensions
{
    public static IDisposable LoadExtension<T>(this T application, Assembly asm) where T : Application
    {
        Type appType = application.GetType();
        Type genericExtHost = typeof(ApplicationExtensionHost<>).MakeGenericType(appType);
        if (Activator.CreateInstance(genericExtHost, application) is IApplicationExtensionHost host)
            return host.LoadExtension(asm);

        throw new InvalidProgramException();
    }
}