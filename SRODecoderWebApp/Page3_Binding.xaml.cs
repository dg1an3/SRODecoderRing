using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SRODecoderWebApp
{
    public partial class Page3_Binding : Page
    {
        ObservableCollection<Person> _listOfContacts;

        public Page3_Binding()
        {
            this.InitializeComponent();

            //-------------
            // First Demo
            //-------------
            ObservableCollection<Planet> listOfPlanets = Planet.GetListOfPlanets();
            ItemsControl1.ItemsSource = listOfPlanets;
            ContentControl1.Content = listOfPlanets[0];

            //-------------
            // Second Demo
            //-------------
            _listOfContacts = new ObservableCollection<Person>()
            {
                new Person() { FirstName = "Albert", LastName = "Einstein" },
                new Person() { FirstName = "Isaac", LastName = "Newton" },
                new Person() { FirstName = "Galileo", LastName = "Galilei" },
                new Person() { FirstName = "Marie", LastName = "Curie" },
            };
            ItemsControl2.ItemsSource = _listOfContacts;
        }

        //-------------
        // First Demo
        //-------------

        private void ButtonPlanet_Click(object sender, RoutedEventArgs e)
        {
            var planet = ((Button)sender).DataContext;
            ContentControl1.Content = planet;
        }

        void ViewHideSourceCodeForFirstDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForFirstDemo.Visibility = (SourceCodeForFirstDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        //-------------
        // Second Demo
        //-------------

        class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var person = (Person)((Button)sender).DataContext;
            _listOfContacts.Remove(person);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            _listOfContacts.Add(new Person() { FirstName = FirstNameTextBox.Text, LastName = LastNameTextBox.Text });
        }

        void ViewHideSourceCodeForSecondDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForSecondDemo.Visibility = (SourceCodeForSecondDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        //-------------
        // Third Demo
        //-------------

        void ViewHideSourceCodeForThirdDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForThirdDemo.Visibility = (SourceCodeForThirdDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }
    }
}
