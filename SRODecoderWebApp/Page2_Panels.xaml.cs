using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SRODecoderWebApp
{
    public partial class Page2_Panels : Page
    {
        public Page2_Panels()
        {
            this.InitializeComponent();
        }

        void ViewHideSourceCodeForStackPanelDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForStackPanelDemo.Visibility = (SourceCodeForStackPanelDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForCanvasDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForCanvasDemo.Visibility = (SourceCodeForCanvasDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForGridDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForGridDemo.Visibility = (SourceCodeForGridDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForWrapPanelDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForWrapPanelDemo.Visibility = (SourceCodeForWrapPanelDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }
    }
}
