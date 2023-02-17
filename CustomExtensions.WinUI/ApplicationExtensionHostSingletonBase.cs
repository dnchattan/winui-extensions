using System.Reflection;

namespace CustomExtensions.WinUI;

internal abstract class ApplicationExtensionHostSingletonBase
{
	public Assembly EntryAssembly { get; }
	public string HostingProcessDir { get; }

	public ApplicationExtensionHostSingletonBase()
	{
		EntryAssembly = Assembly.GetEntryAssembly().AssertDefined();
		HostingProcessDir = Path.GetDirectoryName(EntryAssembly.AssertDefined().Location).AssertDefined();
	}
}
