using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SRODecoderWebApp
{
    public partial class MainPage : Page
    {
        enum CurrentState
        {
            LargeResolution_SeeBothMenuAndPage, // This corresponds to tablets and other devices with high resolution. In this case we see both the menu and the page.
            SmallResolution_SeeMenuOnly, // This corresponds to smartphones and other devices with low resolution. In this case we see only the menu.
            SmallResolution_SeePageOnly // This corresponds to smartphones and other devices with low resolution. In this case we see only the page.
        }

        struct MenuItem
        {
            public string DisplayName { get; set; }
            public Type Type { get; set; }
        }

        CurrentState _currentState = CurrentState.LargeResolution_SeeBothMenuAndPage;

        public MainPage()
        {
            this.InitializeComponent();

            SwitchStateBasedOnDisplaySize();
            UpdateUIBasedOnCurrentState();

            // Listen to window size changes in order to switch between "SmallResolution" and "LargeResolution" states when the user resizes the browser window
            Window.Current.SizeChanged += Window_SizeChanged;

            // Populate the menu with the list of pages:
            MenuItemsContainer.ItemsSource = new List<MenuItem>() {
                new MenuItem(){ DisplayName = "SRODecoderRing", Type = typeof(Page_SRODecoderRing) },
                new MenuItem(){ DisplayName = "Controls", Type = typeof(Page1_Controls) },
                new MenuItem(){ DisplayName = "Panels", Type = typeof(Page2_Panels) },
                new MenuItem(){ DisplayName = "Binding", Type = typeof(Page3_Binding) },
                new MenuItem(){ DisplayName = "WCF", Type = typeof(Page4_WCF) },
                new MenuItem(){ DisplayName = "Other", Type = typeof(Page5_Other) },
                new MenuItem(){ DisplayName = "About", Type = typeof(Page6_About) }
            };

            MenuItemsContainer.SelectedIndex = 0;
        }

        private async void MenuItemsContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            if (listBox.SelectedItem != null)
            {
                // Create an instance of the selected page and show it:
                var menuItem = (MenuItem)listBox.SelectedItem;
                await SetCurrentPage((FrameworkElement)Activator.CreateInstance(menuItem.Type));
            }
            else
            {
                // Dispose previous page if necessary:
                if (PageContainer.Child is IDisposable)
                    ((IDisposable)PageContainer.Child).Dispose();

                // Remove page if any:
                PageContainer.Child = null;
            }
        }

        async Task SetCurrentPage(FrameworkElement content)
        {
            // Dispose previous page if necessary:
            if (PageContainer.Child is IDisposable)
                ((IDisposable)PageContainer.Child).Dispose();

            // Display the "Loading..." text:
            ButtonToGoBackInCaseOfSmallScreen.Content = "Loading...";
            LoadingMessage.Visibility = Visibility.Visible;

            // Scroll to top:
            CurrentPageScrollViewer.ScrollToVerticalOffset(0);
            CurrentPageScrollViewer.ScrollToHorizontalOffset(0);

            // Switch to page view if not already there:
            GoToStateWherePageIsVisible();

            // Launch an async operation in order to trigger a redraw so that the user can see the "Loading..." message:
            await Task.Delay(100);

            // Set the new page:
            PageContainer.Child = content;

            // Restore the text of the Back button (that we used to display the "Loading..." text - cf. above) and hide the "Loading..." text:
            ButtonToGoBackInCaseOfSmallScreen.Content = "< Back";
            LoadingMessage.Visibility = Visibility.Collapsed;
        }

        void UpdateUIBasedOnCurrentState()
        {
            switch (_currentState)
            {
                case CurrentState.LargeResolution_SeeBothMenuAndPage:
                    LeftColumn.Width = new GridLength(270);
                    TitleAndTheMenu.Visibility = Visibility.Visible;
                    ButtonToGoBackInCaseOfSmallScreen.Visibility = Visibility.Collapsed;
                    PageContainer.Visibility = Visibility.Visible;
                    CurrentPageScrollViewer.Visibility = Visibility.Visible;
                    EmptySpaceThatWillBeRemovedOnSmallScreens.Visibility = Visibility.Visible;
                    break;
                case CurrentState.SmallResolution_SeeMenuOnly:
                    MenuItemsContainer.SelectedIndex = -1;
                    LeftColumn.Width = new GridLength(270);
                    TitleAndTheMenu.Visibility = Visibility.Visible;
                    ButtonToGoBackInCaseOfSmallScreen.Visibility = Visibility.Collapsed;
                    PageContainer.Visibility = Visibility.Collapsed;
                    CurrentPageScrollViewer.Visibility = Visibility.Collapsed;
                    EmptySpaceThatWillBeRemovedOnSmallScreens.Visibility = Visibility.Collapsed;
                    break;
                case CurrentState.SmallResolution_SeePageOnly:
                    LeftColumn.Width = new GridLength(0);
                    TitleAndTheMenu.Visibility = Visibility.Collapsed;
                    ButtonToGoBackInCaseOfSmallScreen.Visibility = Visibility.Visible;
                    PageContainer.Visibility = Visibility.Visible;
                    CurrentPageScrollViewer.Visibility = Visibility.Visible;
                    EmptySpaceThatWillBeRemovedOnSmallScreens.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }

            CurrentPageScrollViewer.Width = Window.Current.Bounds.Width - LeftColumn.Width.Value;
        }

        void GoToStateWherePageIsVisible()
        {
            switch (_currentState)
            {
                case CurrentState.LargeResolution_SeeBothMenuAndPage:
                    // Nothing to do here because the page is already visible.
                    break;
                case CurrentState.SmallResolution_SeeMenuOnly:
                    // If we arrived here it means that we are running on a device
                    // with a small resolution, and we are currently seeing the menu,
                    // so we switch to the page view:
                    _currentState = CurrentState.SmallResolution_SeePageOnly;
                    UpdateUIBasedOnCurrentState();
                    break;
                case CurrentState.SmallResolution_SeePageOnly:
                    // Nothing to do here because the page is already visible.
                    break;
                default:
                    break;
            }
        }

        void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentState)
            {
                case CurrentState.LargeResolution_SeeBothMenuAndPage:
                    // Nothing to do here because the menu is already visible.
                    break;
                case CurrentState.SmallResolution_SeeMenuOnly:
                    // Nothing to do here because the menu is already visible.
                    break;
                case CurrentState.SmallResolution_SeePageOnly:
                    // If we arrived here it means that we are running on a device
                    // with a small resolution, and we are currently seeing the page,
                    // so we switch to the menu view:
                    _currentState = CurrentState.SmallResolution_SeeMenuOnly;
                    UpdateUIBasedOnCurrentState();
                    break;
                default:
                    break;
            }
        }

        void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            SwitchStateBasedOnDisplaySize();

            CurrentPageScrollViewer.Width = Window.Current.Bounds.Width - LeftColumn.Width.Value;
        }

        void SwitchStateBasedOnDisplaySize()
        {
            Rect windowBounds = Window.Current.Bounds;
            double displayWidth = windowBounds.Width;
            if (displayWidth < 10000)
            {
                if (_currentState == CurrentState.LargeResolution_SeeBothMenuAndPage)
                {
                    // Switch to "SmallResolution" state
                    if (PageContainer.Child != null)
                        _currentState = CurrentState.SmallResolution_SeePageOnly;
                    else
                        _currentState = CurrentState.SmallResolution_SeeMenuOnly;
                    UpdateUIBasedOnCurrentState();
                }
            }
            else
            {
                if (_currentState != CurrentState.LargeResolution_SeeBothMenuAndPage)
                {
                    // Switch to "LargeResolution" state
                    _currentState = CurrentState.LargeResolution_SeeBothMenuAndPage;
                    UpdateUIBasedOnCurrentState();
                }
            }
        }

        void SplashScreenContinueButton_Click(object sender, RoutedEventArgs e)
        {
            // LayoutRoot.Children.Remove(SplashScreen);
        }
    }
}
