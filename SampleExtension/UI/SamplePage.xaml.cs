using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using WinUI.Extensions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SampleExtension.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SamplePage : Page
	{
		public SamplePage()
		{
			//this.InitializeComponent();
			this.LoadComponent(ref _contentLoaded);
		}

		public GreetEntity InitialSelectedEntity
		{
			get { return (GreetEntity)GetValue(SelectedEntityProperty); }
			set { SetValue(SelectedEntityProperty, value); }
		}

		// Using a DependencyProperty as the backing store for InitialSelectedEntity.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedEntityProperty =
			DependencyProperty.Register(nameof(InitialSelectedEntity), typeof(GreetEntity), typeof(SamplePage), new PropertyMetadata(GreetEntity.World));


	}
}
