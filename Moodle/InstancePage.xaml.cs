
using MoodleManager.Container;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MoodleManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InstancePage : Page
    {
        public Instances InstanceModel;
        public CourseManager Cm;
        public Instances newInsts;
        public Instances upcoming;
        public InstancePage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            {
                // TODO: Go back to the previous page
                
                Frame.Navigate(typeof(MainPage), new MainPageContainer(Cm,newInsts,upcoming));
            };
        }



     

      

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
          InstancePageContainer insContainer = (InstancePageContainer) e.Parameter;
            Cm = insContainer.courseManager;
            newInsts = insContainer.newInst;
            upcoming = insContainer.upcoming;
            Course course = Cm.courses.ElementAt(insContainer.clickedIndex);
         //   if(course.instanceActivity.Count() == 0)
         //   course.getInstances(MainPage.cookieContainer);
            NumofCourse.Text = "Number of instances: " + course.InstanceActivity.Count().ToString();
            InstanceModel = course.InstanceActivity;
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }
    }
}
