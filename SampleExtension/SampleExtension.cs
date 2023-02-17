// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml;

using SampleApp.Extensibility;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SampleExtension
{
	public class SampleExtension : ISampleExtension
	{
		private FrameworkElement _content;

		public string Name => "Sample Extension";

		public FrameworkElement Content => EnsureContent();

		private FrameworkElement EnsureContent()
		{
			if (_content != null)
				return _content;
			return _content = new UI.SamplePage();
		}
	}
}
