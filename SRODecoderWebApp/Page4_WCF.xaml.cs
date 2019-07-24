using ServiceReference1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SRODecoderWebApp
{
    public partial class Page4_WCF : Page
    {
        Guid _ownerId;

        public Page4_WCF()
        {
            this.InitializeComponent();

            // The "Owner ID" ensures that every person that uses the Showcase App has its own list of To-Do's:
            _ownerId = Guid.NewGuid();
        }

        void ViewHideSourceCodeForWebClientDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForWebClientDemo.Visibility = (SourceCodeForWebClientDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForSoapDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForSoapDemo.Visibility = (SourceCodeForSoapDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        //-------------
        // REST Demo
        //-------------

        async Task RefreshRestToDos()
        {
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            webClient.Headers[HttpRequestHeader.Accept] = "application/xml";

            string response = await webClient.DownloadStringTaskAsync("http://cshtml5-rest-sample.azurewebsites.net/api/Todo?OwnerId=" + _ownerId.ToString());

            var dataContractSerializer = new DataContractSerializer(typeof(List<ToDoItem>));
            List<ToDoItem> toDoItems = (List<ToDoItem>)dataContractSerializer.DeserializeFromString(response);
            RestToDosItemsControl.ItemsSource = toDoItems;
        }

        async void ButtonRefreshRestToDos_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.Content = "Please wait...";
            button.IsEnabled = false;

            await RefreshRestToDos();

            button.IsEnabled = true;
            button.Content = "Refresh the list";
        }

        async void ButtonAddRestToDo_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.Content = "Please wait...";
            button.IsEnabled = false;

            string data = string.Format(@"{{""OwnerId"": ""{0}"",""Id"": ""{1}"",""Description"": ""{2}""}}", _ownerId, Guid.NewGuid(), RestToDoTextBox.Text.Replace("\"", "'"));
            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            webClient.Encoding = Encoding.UTF8;
            string response = await webClient.UploadStringTaskAsync("http://cshtml5-rest-sample.azurewebsites.net/api/Todo/", "POST", data);

            await RefreshRestToDos();

            button.IsEnabled = true;
            button.Content = "Create";
        }

        async void ButtonDeleteRestToDo_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.Content = "Please wait...";
            button.IsEnabled = false;

            ToDoItem todo = ((ToDoItem)button.DataContext);
            var webClient = new WebClient();
            string response = await webClient.UploadStringTaskAsync("http://cshtml5-rest-sample.azurewebsites.net/api/Todo/" + todo.Id.ToString() + "?OwnerId=" + _ownerId.ToString(), "DELETE", "");

            await RefreshRestToDos();

            button.IsEnabled = true;
            button.Content = "Delete";
        }

        //-------------
        // SOAP Demo
        //-------------

        async Task RefreshSoapToDos()
        {
            Service1Client soapClient =
                new Service1Client(
                    new BasicHttpBinding(),
                    new EndpointAddress(
                        new Uri("http://cshtml5-soap-sample.azurewebsites.net/Service1.svc")));

            var result = await soapClient.GetToDosAsync(_ownerId);
            ToDoItem[] todos = result.Body.GetToDosResult;
            SoapToDosItemsControl.ItemsSource = todos;
        }

        async void ButtonRefreshSoapToDos_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.Content = "Please wait...";
            button.IsEnabled = false;

            await RefreshSoapToDos();

            button.IsEnabled = true;
            button.Content = "Refresh the list";
        }

        async void ButtonAddSoapToDo_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.Content = "Please wait...";
            button.IsEnabled = false;

            ToDoItem todo = new ToDoItem()
            {
                Description = SoapToDoTextBox.Text,
                Id = Guid.NewGuid(),
                OwnerId = _ownerId
            };

            Service1Client soapClient =
                new Service1Client(
                    new BasicHttpBinding(),
                    new EndpointAddress(
                        new Uri("http://cshtml5-soap-sample.azurewebsites.net/Service1.svc")));

            await soapClient.AddOrUpdateToDoAsync(todo);

            await RefreshSoapToDos();

            button.IsEnabled = true;
            button.Content = "Create";
        }

        async void ButtonDeleteSoapToDo_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.Content = "Please wait...";
            button.IsEnabled = false;

            try
            {
                ToDoItem todo = (ToDoItem)((Button)sender).DataContext;

                Service1Client soapClient =
                    new Service1Client(
                        new BasicHttpBinding(),
                        new EndpointAddress(
                            new Uri("http://cshtml5-soap-sample.azurewebsites.net/Service1.svc")));

                await soapClient.DeleteToDoAsync(todo);

                await RefreshSoapToDos();
            }
            catch (FaultException ex)
            {
                // With "Fault Exceptions" the server can pass to the client some deliberate information such as "Item not found":
                MessageBox.Show(ex.Message);
            }

            button.IsEnabled = true;
            button.Content = "Delete";
        }

    }
}





