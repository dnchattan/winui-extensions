using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SampleApp
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
		}

		public ObservableCollection<ExtensionDescriptorModel> MenuItems = new();

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			string searchDir =
				Path.Combine(
					Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
					@"..\..\..\..\..\..\SampleExtension\bin\Debug\net6.0-windows10.0.19041.0"
					);
			foreach (FileInfo fileInfo in new DirectoryInfo(searchDir).EnumerateFiles("*.SampleAppExtension.dll", new EnumerationOptions() { RecurseSubdirectories = true }))
			{
				MenuItems.Add(new(fileInfo.FullName));
			}
		}

		private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			if (args.SelectedItem is not ExtensionDescriptorModel model)
				return;
			model.Load();
		}
	}
}
