using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace SRODecoderWebApp
{
    public partial class Page5_Other : Page, IDisposable
    {
        DispatcherTimer _dispatcherTimer;
        ClassToSerialize _classToSerialize;

        public Page5_Other()
        {
            this.InitializeComponent();

            // Initialize the DispatcherTimer
            _dispatcherTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 100) };
            _dispatcherTimer.Tick += DispatcherTimer_Tick;

            // Initialize the XmlSerializer demo:
            _classToSerialize = new ClassToSerialize()
            {
                TextField = "Some text",
                DateField = DateTime.Now,
                GuidField = Guid.NewGuid(),
                BooleanField = true
            };
            SerializationSourcePanel.DataContext = _classToSerialize;
        }

        void ViewHideSourceCodeForTimerDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForTimerDemo.Visibility = (SourceCodeForTimerDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForAnimationsDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForAnimationsDemo.Visibility = (SourceCodeForAnimationsDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForDragAndDropDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForDragAndDropDemo.Visibility = (SourceCodeForDragAndDropDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForRegex1Demo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForRegex1Demo.Visibility = (SourceCodeForRegex1Demo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForRegex2Demo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForRegex2Demo.Visibility = (SourceCodeForRegex2Demo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForIsolatedStorageSettingsDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForIsolatedStorageSettingsDemo.Visibility = (SourceCodeForIsolatedStorageSettingsDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForIsolatedStorageFileDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForIsolatedStorageFileDemo.Visibility = (SourceCodeForIsolatedStorageFileDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForFileInfoDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForFileInfoDemo.Visibility = (SourceCodeForFileInfoDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForStylesDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForStylesDemo.Visibility = (SourceCodeForStylesDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForXmlSerializerDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForXmlSerializerDemo.Visibility = (SourceCodeForXmlSerializerDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForLinqDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForLinqDemo.Visibility = (SourceCodeForLinqDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        void ViewHideSourceCodeForAsyncAwaitDemo_Click(object sender, RoutedEventArgs e)
        {
            SourceCodeForAsyncAwaitDemo.Visibility = (SourceCodeForAsyncAwaitDemo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        //-------------
        // Timer Demo
        //-------------

        void ButtonToStartTimer_Click(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Start();
        }

        void ButtonToStopTimer_Click(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Stop();
        }

        void DispatcherTimer_Tick(object sender, object e)
        {
            // Increment the counter by 1
            if (CounterTextBlock.Text == null || CounterTextBlock.Text == string.Empty)
                CounterTextBlock.Text = "0";
            else
                CounterTextBlock.Text = (int.Parse(CounterTextBlock.Text) + 1).ToString();
        }

        public void Dispose()
        {
            _dispatcherTimer.Stop();
        }

        //-------------
        // Animations Demo
        //-------------

        void ButtonToStartAnimationOpen_Click(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)CanvasForAnimationsDemo.Resources["AnimationToOpen"];
            storyboard.Begin();
            ButtonToStartAnimationOpen.Visibility = Visibility.Collapsed;
            ButtonToStartAnimationClose.Visibility = Visibility.Visible;
        }

        void ButtonToStartAnimationClose_Click(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)CanvasForAnimationsDemo.Resources["AnimationToClose"];
            storyboard.Begin();
            ButtonToStartAnimationOpen.Visibility = Visibility.Visible;
            ButtonToStartAnimationClose.Visibility = Visibility.Collapsed;
        }

        //-------------
        // Drag and Drop Demo
        //-------------

        bool _isPointerCaptured;
        double _pointerX;
        double _pointerY;
        double _objectLeft;
        double _objectTop;

        void DragAndDropItem_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            UIElement uielement = (UIElement)sender;
            _pointerX = e.GetCurrentPoint(null).Position.X;
            _pointerY = e.GetCurrentPoint(null).Position.Y;
            _objectLeft = Canvas.GetLeft(uielement);
            _objectTop = Canvas.GetTop(uielement);
            uielement.CapturePointer(e.Pointer);
            _isPointerCaptured = true;
        }

        void DragAndDropItem_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            UIElement uielement = (UIElement)sender;
            if (_isPointerCaptured)
            {
                // Calculate the new position of the object:
                double deltaH = e.GetCurrentPoint(null).Position.X - _pointerX;
                double deltaV = e.GetCurrentPoint(null).Position.Y - _pointerY;
                _objectLeft = deltaH + _objectLeft;
                _objectTop = deltaV + _objectTop;

                // Update the object position:
                Canvas.SetLeft(uielement, _objectLeft);
                Canvas.SetTop(uielement, _objectTop);

                // Remember the pointer position:
                _pointerX = e.GetCurrentPoint(null).Position.X;
                _pointerY = e.GetCurrentPoint(null).Position.Y;
            }
        }

        void DragAndDropItem_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            UIElement uielement = (UIElement)sender;
            _isPointerCaptured = false;
            uielement.ReleasePointerCapture(e.Pointer);
        }

        //-------------
        // Regular Expressions Demo
        //-------------

        private void ButtonCheckIfEmailAddressIsValid_Click(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(TextBoxEmailAddress.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                MessageBox.Show(TextBoxEmailAddress.Text + " is a valid email address.");
            else
                MessageBox.Show(TextBoxEmailAddress.Text + " is NOT a valid email address.");
        }

        private void ButtonReplaceDates_Click(object sender, RoutedEventArgs e)
        {
            TextBlockOutputOfRegexReplaceDemo.Text = Regex.Replace(TextBoxRegexReplaceDemo.Text, @"(\d{2})/(\d{2})/(\d{2,4})", "$3-$2-$1");
        }

        private void ApplyRegularExpression(object sender, RoutedEventArgs e)
        {
            string expression = ExpressionTextBox.Text;
            string input = InputTextBox.Text;
            Regex reg = new Regex(expression, RegexOptions.ECMAScript);
            IsMatchTextBlock.Text = reg.IsMatch(input).ToString();
            Match m = reg.Match(input);
            MatchTextBlock.Text = m.Value;
            MatchCollection col = reg.Matches(input);
            string str = "";
            bool isFirstMatch = true;
            foreach (Match match in col)
            {
                if (!isFirstMatch)
                {
                    str += ", ";
                }
                str += "{" + match.Value + "}";
            }
            MatchesTextBlock.Text = str;
            ReplaceTextBlock.Text = reg.Replace(input, ReplacementTextBox.Text);
            EscapeTextBlock.Text = Regex.Escape(expression);
            try
            {
                UnescapeTextBlock.Text = Regex.Unescape(expression);
            }
            catch
            {
                UnescapeTextBlock.Text = "Invalid expression: sequence not escaped.";
            }
            CapturedGroupsStackPanel.Children.Clear();
            int i = 0;
            foreach (Group group in m.Groups)
            {
                TextBlock t = new TextBlock();
                if (i == 0)
                {
                    t.Text = string.Format("- (no id) =     {0}", group.Value);
                }
                else
                {
                    t.Text = string.Format("- ${0}    =    {1}", i, group.Value);
                }
                t.FontSize = 12;
                CapturedGroupsStackPanel.Children.Add(t);
                ++i;
            }
        }

        //-------------
        // Storage Demo
        //-------------

        private void ButtonSaveToIsolatedStorageSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!DisplayWarningIfRunningFromLocalFileSystemOnInternetExplorer())
            {
                string key = "SampleKey";
                string value = TextBoxIsolatedStorageSettingsDemo.Text;

                IsolatedStorageSettings.ApplicationSettings[key] = value;
                MessageBox.Show("The text was successfully saved to the storage.");
            }
        }

        private void ButtonLoadFromIsolatedStorageSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!DisplayWarningIfRunningFromLocalFileSystemOnInternetExplorer())
            {
                string key = "SampleKey";

                string value;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value))
                    MessageBox.Show("The following text was read from the storage: " + value);
                else
                    MessageBox.Show("No text was found in the storage.");
            }
        }

        private void ButtonDeleteFromIsolatedStorageSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!DisplayWarningIfRunningFromLocalFileSystemOnInternetExplorer())
            {
                string key = "SampleKey";
                IsolatedStorageSettings.ApplicationSettings.Remove(key);
                MessageBox.Show("The text was successfully removed from the storage.");
            }
        }

        private void ButtonSaveToIsolatedStorageFile_Click(object sender, RoutedEventArgs e)
        {
            if (!DisplayWarningIfRunningFromLocalFileSystemOnInternetExplorer())
            {
                string fileName = "SampleFile.txt";
                string data = TextBoxFileStorageDemo.Text;

                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    IsolatedStorageFileStream fs = null;
                    using (fs = storage.CreateFile(fileName))
                    {
                        if (fs != null)
                        {
                            Encoding encoding = new UTF8Encoding();
                            byte[] bytes = encoding.GetBytes(data);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                            MessageBox.Show("A new file named SampleFile.txt was successfully saved to the storage.");
                        }
                        else
                            MessageBox.Show("Unable to save the file SampleFile.txt to the storage.");
                    }
                }
            }
        }

        private void ButtonLoadFromIsolatedStorageFile_Click(object sender, RoutedEventArgs e)
        {
            if (!DisplayWarningIfRunningFromLocalFileSystemOnInternetExplorer())
            {
                string fileName = "SampleFile.txt";

                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    if (storage.FileExists(fileName))
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile(fileName, System.IO.FileMode.Open))
                        {
                            if (fs != null)
                            {
                                using (StreamReader sr = new StreamReader(fs))
                                {
                                    string data = sr.ReadToEnd();
                                    MessageBox.Show("The following text was read from the file SampleFile.txt located in the storage: " + data);
                                }
                            }
                            else
                                MessageBox.Show("Unable to load the file SampleFile.txt from the storage.");
                        }
                    }
                    else
                        MessageBox.Show("No file named SampleFile.txt was found in the storage.");
                }
            }
        }

        private void ButtonSaveToFileInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!DisplayWarningIfRunningFromLocalFileSystemOnInternetExplorer())
            {
                string fileName = "Test.txt";
                string data = TextBoxFileStorageDemo.Text;

                FileInfo fileInfo = new FileInfo(fileName);
                using (FileStream fs = fileInfo.OpenWrite())
                {
                    Encoding encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(data);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                    MessageBox.Show("A new file named Test.txt was successfully saved to the storage.");
                }
            }
        }

        private void ButtonLoadFromFileInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!DisplayWarningIfRunningFromLocalFileSystemOnInternetExplorer())
            {
                string fileName = "Test.txt";

                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                {
                    using (FileStream fs = fileInfo.OpenRead())
                    {
                        if (fs != null)
                        {
                            using (StreamReader sr = new StreamReader(fs))
                            {
                                string data = sr.ReadToEnd();
                                MessageBox.Show("The following text was read from the file Test.txt located in the storage: " + data);
                            }
                        }
                        else
                            MessageBox.Show("Unable to load the file Test.txt from the storage.");
                    }
                }
                else
                    MessageBox.Show("No file named Test.txt was found in the storage.");
            }
        }

        private void ButtonDeleteFileInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!DisplayWarningIfRunningFromLocalFileSystemOnInternetExplorer())
            {
                string fileName = "Test.txt";

                FileInfo fileInfo = new FileInfo(fileName);
                fileInfo.Delete();
                MessageBox.Show("The file named Test.txt was successfully deleted from the local page storage.");
            }
        }

        //-------------
        // DataContractSerializer Demo
        //-------------

        public class ClassToSerialize
        {
            public string TextField { get; set; }
            public DateTime DateField { get; set; }
            public Guid GuidField { get; set; }
            public bool BooleanField { get; set; }
        }

        void ButtonSerializeDeserialize_Click(object sender, RoutedEventArgs e)
        {
            // Serialize:
            var dataContractSerializer = new DataContractSerializer(typeof(ClassToSerialize));
            var serializedXml = dataContractSerializer.SerializeToString(_classToSerialize);

            // Display the result of the serialization:
            MessageBox.Show("Result of the serialization:" + Environment.NewLine + Environment.NewLine + serializedXml);

            // Deserialize:
            dataContractSerializer = new DataContractSerializer(typeof(ClassToSerialize));
            ClassToSerialize deserializedObject = (ClassToSerialize)dataContractSerializer.DeserializeFromString(serializedXml);

            // Display the result of the deserialization:
            SerializationDestinationPanel.DataContext = deserializedObject;
        }

        //-------------
        // Linq Demo
        //-------------

        void ButtonToDemonstrateLinq_Click(object sender, RoutedEventArgs e)
        {
            var planets = Planet.GetListOfPlanets();

            var result = from p in planets
                         where p.Radius > 7000
                         orderby p.Name
                         select p.Name;

            MessageBox.Show(string.Format("List of planets that have a radius greater than 7000km sorted alphabetically: {0}", string.Join(", ", result)));
        }

        //-------------
        // Async/Await Demo
        //-------------

        async void ButtonToDemonstrateAsyncAwait_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.Visibility = Visibility.Collapsed;
            TaskBasedCounterTextBlock.Visibility = Visibility.Visible;
            TaskBasedCounterTextBlock.Text = "5 seconds left";
            await Task.Delay(1000);
            TaskBasedCounterTextBlock.Text = "4 seconds left";
            await Task.Delay(1000);
            TaskBasedCounterTextBlock.Text = "3 seconds left";
            await Task.Delay(1000);
            TaskBasedCounterTextBlock.Text = "2 seconds left";
            await Task.Delay(1000);
            TaskBasedCounterTextBlock.Text = "1 second left";
            await Task.Delay(1000);
            TaskBasedCounterTextBlock.Text = "Done!";
            button.Visibility = Visibility.Visible;
        }

        //-------------
        // Other
        //-------------

        bool DisplayWarningIfRunningFromLocalFileSystemOnInternetExplorer()
        {
            //-----------------------------------------------------------
            // When running inside Internet Explorer or Edge, the HTML5
            // Storage API is available only if the URL starts with http
            // or https. This method will display a message to the user
            // to inform her about this.
            //-----------------------------------------------------------
            if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
            {
                //Execute a piece of JavaScript code:
                if (IsRunningFromLocalFileSystemOnInternetExplorer())
                {
                    MessageBox.Show("The local storage - used to persist data - is not available on Internet Explorer or Edge when running the website from the local file system (ie. the URL starts with 'c:\' or 'file:///'). To solve the problem, please run the website from a web server instead (ie. the URL must start with 'http://' or 'https://') or test the local storage using a different browser.");
                    return true;
                }
            }
            return false;
        }

        [JSIL.Meta.JSReplacement(@"window.IE_VERSION && document.location.protocol === ""file:""")]
        bool IsRunningFromLocalFileSystemOnInternetExplorer()
        {
            return false;
        }
    }
}




