# winui-extensions

This package provides the ability to load loose "extension" (or addin) assemblies which may contain WinUI components and allow them to correctly render in the hosting process. Additionally, it provides some limited support for Hot Reload, depending on how your extensions are packaged.

## Loading an extension

Once you have loaded the extension assembly (such as via `Assembly.LoadFrom`), call the following extension method on your XamlApplication instance:

```cs
using WinUI.Extensions;

/* ... */

void LoadMyExtension(Assembly asm)
{
    // save off the handle so we can clean up our registration with the
    // hosting process later
    _extensionHandle = Application.Current.LoadExtension(ExtensionAsm);
}
```

This API will return an `IDisposable` that when disposed will remove your extension's Xaml type metadata registration from the hosting assembly, making it safe(er) to unload an assembly that was previously rendered (haven't actually tested this).

## Extension UI Requirements

Any extension assembly must disable the generated `InitializeComponent()` method from their codebehind, and instead call the extension method:

```cs
using Microsoft.UI.Xaml.Controls;
using WinUI.Extensions;

namespace SampleExtension.UI;

public sealed partial class SamplePage : Page
{
    public SamplePage()
    {
        // this.InitializeComponent();
        // Should pass a path to the XAML file's directory
        // relative to the project root.
        this.LoadComponent(ref _contentLoaded, "UI");
    }
}
```

## Hot-reload

Hot-reload will function as long as your application is loading the extension directly from its output directory. If not, it is expected that your dll has all the required resource files adjacent to it, and hot reload will likely not work. If you have any issues check the trace log for any messages regarding Hot Reload.

# How it works

There's two main things that need to be accounted for when loading extensions: registering the generated `XamlTypeInfo.g.cs` into the host process, and changing the way your Xaml components load themselves.

## XamlTypeInfo

The generated `XamlTypeInfo.g.cs` file for a WinUI assembly contains all kinds of generated type and metadata mappings that the host process will need to be able to properly find things by type. This needs to be connected to the same kind of generated code in the parent process, however since it is generated late in the build process, it's rather difficult to get a project to reference these artifacts in code.

In order to make this easier, there is an extension method which the hosting XamlApplication can call on itself to connect another assembly's type information into its own registrations by using some reflection on both the host and extension assemblies to find the correct types.

## ~~InitializeComponent~~ -> LoadComponent

The generated code for your Xaml's `InitializeComponent` will look something like this:

```cs
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 1.0.0.0")]
[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
public void InitializeComponent()
{
    if (_contentLoaded)
        return;

    _contentLoaded = true;

    global::System.Uri resourceLocator = new global::System.Uri("ms-appx:///SampleExtension.SampleAppExtension/UI/SamplePage.xaml");
    global::Microsoft.UI.Xaml.Application.LoadComponent(this, resourceLocator, global::Microsoft.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Nested);
}
```

This won't work if your extension isn't placed side-by-side with your application resources (usually you want to put them in their own isolated directories so they can be easily added/removed), because the `ms:appx///` path derives from the host application directory.

The resource loader will accept absolute paths from a drive root (e.g. `ms-appx://c:/MyApp/Extensions/Foo/FooPage.xaml`). Fortunately, Xaml is pretty consistent in how it gets packaged, so the `LoadComponent` extension method can fill the place of `InitializeComponent`, and will infer the correct information based on reflection. It will also re-use the generated `_contentLoaded` variable, which it accepts as a `ref`.

