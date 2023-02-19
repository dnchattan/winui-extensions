using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CustomExtensions.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SampleApp.Extensibility;

namespace SampleApp
{
	public class ExtensionDescriptorModel : DependencyObject
	{
		public readonly string ExtensionPath;
		private IExtensionAssembly ExtensionAsm;

		public ExtensionDescriptorModel(string extensionPath)
		{
			ExtensionPath = extensionPath;
			DisplayName = Path.GetFileNameWithoutExtension(extensionPath);
		}

		public async Task Load()
		{
			if (IsLoaded)
				return;

			ExtensionAsm = await ApplicationExtensionHost.Current.LoadExtensionAsync(ExtensionPath);

			// Instance
			foreach (ISampleExtension instance in ExtensionAsm.ForeignAssembly.GetExportedTypes()
				.Where(type => type.IsAssignableTo(typeof(ISampleExtension)))
				.Select(type => Activator.CreateInstance(type) as ISampleExtension))
			{
				Instances.Add(instance);
			}

			Icon = Symbol.Page;
			IsLoaded = true;
		}

		public ObservableCollection<ISampleExtension> Instances { get; } = new();

		public string DisplayName { get; }

		public bool IsLoaded
		{
			get { return (bool)GetValue(IsLoadedProperty); }
			set { SetValue(IsLoadedProperty, value); }
		}

		public Symbol Icon
		{
			get { return (Symbol)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public static readonly DependencyProperty IsLoadedProperty =
			DependencyProperty.Register(nameof(IsLoaded), typeof(bool), typeof(ExtensionDescriptorModel), new PropertyMetadata(false));

		public static readonly DependencyProperty IconProperty =
			DependencyProperty.Register(nameof(Icon), typeof(Symbol), typeof(ExtensionDescriptorModel), new PropertyMetadata(Symbol.Page2));

	}
}
