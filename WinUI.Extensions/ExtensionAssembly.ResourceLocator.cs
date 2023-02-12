namespace WinUI.Extensions;

public partial class ExtensionAssembly
{
	public Uri LocateResource<T>(T component, string? componentPath = null)
	{
		component.AssertDefined();
		if (component.GetType().Assembly != Assembly)
		{
			throw new InvalidProgramException();
		}
		string resourcePath = Path.Combine(typeof(T).Assembly.GetName().Name.AssertDefined(), componentPath ?? string.Empty, $"{typeof(T).Name}.xaml");
		if (!TryEnableHotReload())
		{
			resourcePath = Path.Combine(AssemblyDir, resourcePath).Replace('\\', '/');
		}
		return new Uri($"ms-appx:///{resourcePath.Replace('\\', '/')}");
	}
}
