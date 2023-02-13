using System.Collections.Concurrent;
using System.Reflection;

namespace CustomExtensions.WinUI;

public partial class ExtensionAssembly
{
    internal static bool IsHotReloadEnabled => Environment.GetEnvironmentVariable("ENABLE_XAML_DIAGNOSTICS_SOURCE_INFO") == "1";
    internal static readonly Assembly? EntryAssembly;
    internal static readonly string HostingProcessDir;
    internal static readonly ConcurrentDictionary<Assembly, ExtensionAssembly> Assemblies = new();
    static ExtensionAssembly()
    {
        EntryAssembly = Assembly.GetEntryAssembly();
        HostingProcessDir = Path.GetDirectoryName(EntryAssembly.AssertDefined().Location).AssertDefined();
    }

    public static ExtensionAssembly FromAssembly(Assembly assembly)
    {
        return Assemblies.GetOrAdd(assembly, asm => new(asm));
    }
}
