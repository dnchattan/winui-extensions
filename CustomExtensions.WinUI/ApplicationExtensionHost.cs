using Microsoft.UI.Xaml;

using System.Reflection;

namespace CustomExtensions.WinUI;

public static partial class ApplicationExtensionHost
{
	internal static bool IsHotReloadEnabled => Environment.GetEnvironmentVariable("ENABLE_XAML_DIAGNOSTICS_SOURCE_INFO") == "1";

	private static IApplicationExtensionHost? _Current;
	public static IApplicationExtensionHost Current => _Current ?? throw new InvalidOperationException("ApplicationExtensionHost is not initialized");

	public static void Initialize<TApplication>(TApplication application) where TApplication : Application
	{
		if (_Current != null)
			throw new InvalidOperationException("Cannot initialize application twice");

		_Current = new ApplicationExtensionHostSingleton<TApplication>(application);
	}
}
