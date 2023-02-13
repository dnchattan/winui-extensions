// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

using WinUI.Extensions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SampleExtension.UI
{
    public sealed partial class Greeter : UserControl
	{
		public Greeter()
		{
			// this.InitializeComponent();
			this.LoadComponent(ref _contentLoaded, "UI");
		}

		public GreetEntity TargetEntity
		{
			get { return (GreetEntity)GetValue(TargetEntityProperty); }
			set { SetValue(TargetEntityProperty, value); }
		}

		// Using a DependencyProperty as the backing store for TargetEntity.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TargetEntityProperty =
			DependencyProperty.Register(nameof(TargetEntity), typeof(GreetEntity), typeof(Greeter), new PropertyMetadata(GreetEntity.World));
	}
}