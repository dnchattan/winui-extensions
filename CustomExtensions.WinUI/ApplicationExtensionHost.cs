using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

using System.Reflection;

namespace CustomExtensions.WinUI;

public partial class ApplicationExtensionHost<T> : IApplicationExtensionHost where T : Application
{
    private readonly T Application;

    public ApplicationExtensionHost(T application)
    {
        Application = application;
    }

    public IDisposable LoadExtension(Assembly assembly)
    {
        IDisposable[] metadataProviders = assembly.ExportedTypes
            .Where(type => type.IsAssignableTo(typeof(IXamlMetadataProvider)))
            .Select(metadataType => (Activator.CreateInstance(metadataType) as IXamlMetadataProvider).AssertDefined())
            .Select(RegisterXamlTypeMetadataProvider)
            .ToArray();

        return new DisposableCollection(metadataProviders);
    }
}
