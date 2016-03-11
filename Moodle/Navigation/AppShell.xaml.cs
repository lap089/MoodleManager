using MoodleManager.Navigation.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MoodleManager.Navigation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppShell : Page
    {
        public static AppShell Current;
        public static Frame RootFrame = null;
       

        public AppShell()
        {
            this.InitializeComponent();
            
            Current = this;
            RootFrame = frame;

            // Use the hardware back button instead of the back button in the header of the page.
            // The back button in the header of the page is hidden in this case, of course.
            //if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            //{
            //    Windows.Phone.UI.Input.HardwareButtons.BackPressed += (s, e) =>
            //    {
            //        if (frame.CanGoBack)
            //        {
            //            frame.GoBack();
            //            e.Handled = true;
            //        }
            //    };
            //}
            PageHeader.buttonSignOutEvent += new EventHandler(signOutButton_Click);
            PageHeader.selectedItemChangeEvent += new EventHandler(selectedItemHandler);
            NavPaneList.DataContext = this;
            NavPaneList.ItemsSource = navList;
            pageHeader.Title = "Home";
            pageHeader.IsVisible = true;
            this.frame.Navigate(navList.First().DestPage);
        }

        private void selectedItemHandler(object sender, EventArgs e)
        {
            PageHeader.MyEventArgs parameter  =  e as PageHeader.MyEventArgs;
            int index = parameter.MyEventInt;
        //    NavPaneList.SelectedIndex = index;
        }

        public void signOutButton_Click(object sender, EventArgs e)
        {
            Frame.Navigate(typeof(SignInPage),"SignOut");
        }


        public static List<NavPaneItem> navList = new List<NavPaneItem>(
            new[]
            {
                new NavPaneItem()
                {
                    Symbol = Symbol.Home,
                    Label = "Home",
                    DestPage = typeof(MainPage)
                }, 

                //new NavPaneItem()
                //{
                //    Symbol = Symbol.Add,
                //    Label = "Create event",
                //    DestPage = typeof(HomePage)
                //}, 

              
                  new NavPaneItem()
                {
                    Symbol = Symbol.Setting,
                    Label = "Setting",
                    DestPage = typeof(SettingPage)
                },

                    new NavPaneItem()
                {
                    Symbol = Symbol.ContactInfo,
                    Label = "About",
                    DestPage = typeof(AboutPage)
                }
            });

        public static SplitView NavPane
        {
            get { return Current.navPane; }
        }

    

        private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {

        }

        private void OnNavigatedToPage(object sender, NavigationEventArgs e)
        {

        }

        private void NavPaneItemInvoked(object sender, ListViewItem e)
        {
            NavPaneItem item = (NavPaneItem)((NavPaneListView)sender).ItemFromContainer(e);

            if (item != null)
            {
                if (item.DestPage != null &&
                    item.DestPage != this.frame.CurrentSourcePageType)
                {
                    if (item.DestPage == typeof(MainPage))
                        pageHeader.Title = "Home";
                    else if (item.DestPage == typeof(AboutPage))
                        pageHeader.Title = "About";
                    else if (item.DestPage == typeof(SettingPage))
                        pageHeader.Title = "Setting";

                    if (item.DestPage == typeof(MainPage))
                        pageHeader.IsVisible = true;
                    else pageHeader.IsVisible = false;
                    this.frame.Navigate(item.DestPage, item.Arguments);
                   
                }
            }
        }

        public void test(object sender, RoutedEventArgs e)
        {
        
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            frame.Navigate(typeof(SettingPage));
        }
    }
}
