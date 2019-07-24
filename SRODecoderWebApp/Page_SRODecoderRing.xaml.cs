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
    public partial class Page_SRODecoderRing : Page
    {
        public Page_SRODecoderRing()
        {
            this.InitializeComponent();

            // Populate the ComboBox and ListBox with the list of planets:
            ComboBox1.ItemsSource = Planet.GetListOfPlanets();
            ListBox1.ItemsSource = Planet.GetListOfPlanets();

            // Populate the data grids with the list of planets
            DataGrid1.ItemsSource = Planet.GetListOfPlanets();
            DataGrid2.ItemsSource = Planet.GetListOfPlanets();
        }

        void MyButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You clicked me.");
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Your name is: " + TextBoxName.Text);
        }

        void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You checked me.");
        }

        void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You unchecked me.");
        }

        void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(RadioButton1.IsChecked == true ? "Option 1 selected" : "Option 2 selected");
        }

        void ButtonToPlayAudio_Click(object sender, RoutedEventArgs e)
        {
            MediaElementForAudio.Play();
        }

        void ButtonToPauseAudio_Click(object sender, RoutedEventArgs e)
        {
            MediaElementForAudio.Pause();
        }

        void ButtonToPlayVideo_Click(object sender, RoutedEventArgs e)
        {
            MediaElementForVideo.Play();
        }

        void ButtonToPauseVideo_Click(object sender, RoutedEventArgs e)
        {
            MediaElementForVideo.Pause();
        }

        void ViewHideSourceCodeForButtonDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForButtonDemo.Visibility = (SourceCodeForButtonDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForTextBoxDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForTextBoxDemo.Visibility = (SourceCodeForTextBoxDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForTextBlockDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForTextBlockDemo.Visibility = (SourceCodeForTextBlockDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForCheckBoxDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForCheckBoxDemo.Visibility = (SourceCodeForCheckBoxDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForRadioButtonDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForRadioButtonDemo.Visibility = (SourceCodeForRadioButtonDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForComboBoxDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForComboBoxDemo.Visibility = (SourceCodeForComboBoxDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForListBoxDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForListBoxDemo.Visibility = (SourceCodeForListBoxDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForImageDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForImageDemo.Visibility = (SourceCodeForImageDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForPathDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForPathDemo.Visibility = (SourceCodeForPathDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForShapesDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForShapesDemo.Visibility = (SourceCodeForShapesDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForBorderDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForBorderDemo.Visibility = (SourceCodeForBorderDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForHyperlinkButtonDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForHyperlinkButtonDemo.Visibility = (SourceCodeForHyperlinkButtonDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForDataGridDemo1_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForDataGridDemo1.Visibility = (SourceCodeForDataGridDemo1.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForDataGridDemo2_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForDataGridDemo2.Visibility = (SourceCodeForDataGridDemo2.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForMediaElementDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForMediaElementDemo.Visibility = (SourceCodeForMediaElementDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }
    }
}
