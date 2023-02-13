using System.Runtime.CompilerServices;

namespace CustomExtensions.WinUI;

public partial class ExtensionAssembly
{
    public Uri LocateResource<T>(T component, [CallerFilePath] string callerFilePath = "")
    {
        component.AssertDefined();
        if (component.GetType().Assembly != ForeignAssembly)
        {
            throw new InvalidProgramException();
        }
        string resourceName = Path.GetFileName(callerFilePath)[..^3];

        string[] pathParts = callerFilePath.Split('\\')[..^1];
        for (int i = pathParts.Length - 1; i > 1; i++)
        {
            string pathCandidate = Path.Join(pathParts[i..pathParts.Length].Concat(new string[] { resourceName }).ToArray());
            FileInfo hotResource = new(Path.Combine(ForeignAssemblyDir, ForeignAssemblyName, pathCandidate));
            FileInfo colocatedResource = new(Path.Combine(HostingProcessDir, ForeignAssemblyName, pathCandidate));
            if (hotResource.Exists || colocatedResource.Exists)
            {
                if (TryEnableHotReload() && hotResource.Exists)
                {
                    return new Uri($"ms-appx:///{hotResource.FullName.Replace('\\', '/')}");
                }
                if (colocatedResource.Exists)
                {
                    return new Uri($"ms-appx:///{pathCandidate.Replace('\\', '/')}");
                }
                throw new FileNotFoundException("Could not find resource", resourceName);
            }
        }
        throw new FileNotFoundException("Could not find resource", resourceName);
    }
}
