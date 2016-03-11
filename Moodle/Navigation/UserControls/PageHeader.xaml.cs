using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MoodleManager.Navigation.UserControls
{
    public sealed partial class PageHeader : UserControl
    {

        public static event EventHandler buttonAddEvent;
        public static event EventHandler buttonSignOutEvent;
        public static event EventHandler buttonRefreshEvent;
        public static event EventHandler selectedItemChangeEvent;

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(PageHeader), new PropertyMetadata(null));
        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty WideLayoutThresholdProperty = DependencyProperty.Register("WideLayoutThreshold", typeof(double), typeof(PageHeader), new PropertyMetadata(600));
        public double WideLayoutThreshold
        {
            get { return (double)GetValue(WideLayoutThresholdProperty); }
            set
            {
                SetValue(WideLayoutThresholdProperty, value);
                WideLayoutTrigger.MinWindowWidth = value;
            }
        }

        //public static readonly DependencyProperty signOutButtonProperty = DependencyProperty.Register("signOutButtonProperty", typeof(Delegate), typeof(PageHeader), new PropertyMetadata(null));
        //public object signOutButton
        //{
        //    get { return GetValue(signOutButtonProperty); }
        //    set { SetValue(signOutButtonProperty, value); }
        //}




        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(PageHeader), new PropertyMetadata(false));
        //private ICommand _goBackCommand;

        //public ICommand GoBackCommand
        //{
        //    get
        //    {
        //        if (_goBackCommand == null)
        //        {
        //            // TODO: handle relay command
        //        }
        //        return _goBackCommand;
        //    }
        //}

        public PageHeader()
        {
            this.InitializeComponent();

            //if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            //{
            //    //Remove the backbutton because physical buttons are present
            //    backButton.Visibility = Visibility.Collapsed;
            //}
        }

        private void navPaneToggle_Click(object sender, RoutedEventArgs e)
        {
            Navigation.AppShell.NavPane.IsPaneOpen = !Navigation.AppShell.NavPane.IsPaneOpen;
        }


        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (buttonAddEvent != null)
                buttonAddEvent(this, EventArgs.Empty);
            else
                Debug.WriteLine("MyEvent is null");
        }

     

        private void signOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (buttonSignOutEvent != null)
            {
               NotificationHelper.logOut();
                buttonSignOutEvent(this,EventArgs.Empty);

              
            }
         //   AppShell.ShellFrame.Navigate(typeof(SignInPage), "SignOut");
        }


        public class MyEventArgs : EventArgs
        {
            public int MyEventInt { get; set; }

            public MyEventArgs(int index)
            {
                this.MyEventInt = index;
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppShell.RootFrame == null)
                return;
            // Check to see if this is the top-most page on the app back stack.
            if (AppShell.RootFrame.CanGoBack)
            {
                PageStackEntry pageStack = AppShell.RootFrame.BackStack.Last();
                if (pageStack.SourcePageType == typeof(MainPage))
                    IsVisible = true;
                else IsVisible = false;
                int index = AppShell.navList.FindIndex(x => x.DestPage == pageStack.SourcePageType);
                selectedItemChangeEvent(this, new MyEventArgs(index));
                //    Debug.WriteLine("Check backstack: " + AppShell.RootFrame.BackStack.First().Parameter.GetType());    // If not, set the event to handled and go back to the previous page in the app.
                AppShell.RootFrame.GoBack();
            }
        }

       
        private void AppBarRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (buttonRefreshEvent != null)
            {
                buttonRefreshEvent(this, EventArgs.Empty);
            }
            }
    }
}
