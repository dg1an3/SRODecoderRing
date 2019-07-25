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
    }
}
