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
    class Vector
    {
        public string Label { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
    public partial class Page_SRODecoderRing : Page
    {
        public Page_SRODecoderRing()
        {
            this.InitializeComponent();

            // Populate the data grids with the list of planets
            MatrixDataGrid.ItemsSource = new List<Vector>()
            {
                new Vector() { Label = "Row 1", X = 1.1, Y = 1.0, Z = 1.0 },
                new Vector() { Label = "Row 2", X = 1.2, Y = 1.0, Z = 1.0 },
                new Vector() { Label = "Row 3", X = 1.0, Y = 0.99, Z = 1.1},
            };
            TranslateRotateDataGrid.ItemsSource = new List<Vector>()
            {
                new Vector() { Label = "Translate", X = -10.0, Y = 20.0, Z = 30.0 },
                new Vector() { Label = "Rotate", X = -10.0, Y = 20.0, Z = 30.0 }
            };
        }

        void MyButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You clicked me.");
        }

        void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You checked me.");
        }

        void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You unchecked me.");
        }
    }
}
