using BackgroundNotification;
using HtmlAgilityPack;
using MoodleManager.Container;
using MoodleManager.Navigation.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MoodleManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>  
    public sealed partial class MainPage : Page
    {
        public CourseManager ViewModel = new CourseManager();
        public CourseManager CmfromFile = new CourseManager();
        public Instances InstViewModel = new Instances();
        public Instances UpcomingViewModel = new Instances();
        public MainPage()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            this.InitializeComponent();

            //   NotificationHelper.UpdateTile(100);
            //  GetData();
            // var checktask = BackgroundHelper.FindRegistration<BackgroundNotifier>();
            //  if (checktask != null)
            //text.Text = checktask.ToString();
            ///   else
            //       text.Text = "No background until now!";
            // ViewModel.Connect();
            //test.DataContext = ViewModel;
            // this.DataContextChanged += MainPage_DataContextChanged;

         
           PageHeader.buttonAddEvent += new EventHandler(OnAddButtonChecked);
           PageHeader.buttonRefreshEvent += new EventHandler(OnRefreshButton);
        }

      

        private void MainPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            InstViewModel = DataContext as Instances;
        }

        public void clearAllData()
        {
            ViewModel.courses.Clear();
            CmfromFile.courses.Clear();
            InstViewModel.instances.Clear();
            UpcomingViewModel.instances.Clear();
            ViewModel.IsCompleted = false;
            CmfromFile.IsCompleted = false;
            InstViewModel.IsCompleted = false;
            UpcomingViewModel.IsCompleted = false;
        }

        public async Task GetData()
        {
      //     await NotificationHelper.Connect(NotificationHelper.user, NotificationHelper.pass);
            clearAllData();
            await CmfromFile.LoadDataFromFileAsync();
            await ViewModel.LoadDataFromRemote();
            //  await Task.Delay(TimeSpan.FromSeconds(5));
            ListCourse.ItemsSource = ViewModel.courses;
            
           InstViewModel.ComparationProcess(ViewModel, CmfromFile);
           // Newins.ItemsSource = InstViewModel.instances;

            //     InstViewModel.IsCompleted = true;
            await UpcomingViewModel.getUpcomingEvents();
            UpcomingViewModel.checkPriority();
            //      Upcoming.ItemsSource = UpcomingViewModel.instances;
       //     NotificationHelper.MakeTile(InstViewModel);
       
            // text.Text = ViewModel.courses[0].ToString();
        }



       



        private void InstanceList_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var listview = (sender as ListView).SelectedIndex;

         

            InstancePageContainer insContainer =new InstancePageContainer(ViewModel,InstViewModel,UpcomingViewModel,listview) ;
            Frame.Navigate(typeof(InstancePage),insContainer);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            PageHeader.buttonAddEvent -= new EventHandler(OnAddButtonChecked);
            PageHeader.buttonRefreshEvent -= new EventHandler(OnRefreshButton);
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
          
            try
            {
                MainPageContainer temp = (MainPageContainer)e.Parameter;
                ViewModel = temp.Cm;
                InstViewModel = temp.insts;
                UpcomingViewModel = temp.upcoming;
             //   text.Text = "No catch";

            }
            catch
            {

                //    text.Text = "catch";
                
                //try
                //{
                if (!NotificationHelper.IsFileExist(NotificationHelper.COURSEFILE))
                {
                     CourseSelectionDialog selection_dialog = new CourseSelectionDialog();
                     await selection_dialog.ShowAsync();
                    await GetData();
                    await ViewModel.SaveDataToFile();
                }
                else
                await GetData();

                //}
                //catch (Exception)
                //{
                //    text.Text = "Connection error!";
                //}

                if (!NotificationHelper.IsFileExist(ViewModel.courses.First().Name)) 
                await ViewModel.SaveDataToFile();

            }




        }

        private void InstanceList_Tapped1(object sender, TappedRoutedEventArgs e)
        {
            var listview = (sender as ListView).SelectedIndex;

            //text.Text = "Clicked " + listview;

            InstancePageContainer insContainer = new InstancePageContainer(CmfromFile, InstViewModel,UpcomingViewModel,listview);
            Frame.Navigate(typeof(InstancePage), insContainer);
        }

        
     
       
     

      

        private void Mainpage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
         //   Newins.Height = Window.Current.Bounds.Height * 0.70;
        }

      

        private async void OnAddButtonChecked(object sender, EventArgs e)
        {

            //    Window.Current.Content = Frame;
            //    Window.Current.Activate();
        //    try
            {
                CourseSelectionDialog CourseDialog = new CourseSelectionDialog();
                await CourseDialog.ShowAsync();
                if (CourseDialog.result == SelectionResult.OK)
                {
                    await GetData();
                }
            }
        //    catch (Exception ex)
            { }
         
        }

        private async void OnRefreshButton(object sender, EventArgs e)
        {
            await GetData();
        }

        private void Upcoming_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            if(flyoutBase!= null)
            flyoutBase.ShowAt(senderElement);
        }

        private async void markImportantNewInstance_Click(object sender, RoutedEventArgs e)
        {
            Instance datacontext = (Instance)(e.OriginalSource as FrameworkElement).DataContext;
            //   Debug.WriteLine("Check click: " + datacontext.Name);
            datacontext.IsImportant = !datacontext.IsImportant;
            await InstViewModel.WriteFile(NotificationHelper.PRIORITYFILE);
        }

        private async void deleteUpcoming_Click(object sender, RoutedEventArgs e)
        {
            Instance datacontext = (Instance)(e.OriginalSource as FrameworkElement).DataContext;
            int index = UpcomingViewModel.findIndex(datacontext);
            UpcomingViewModel.instances.RemoveAt(index);
            // Debug.WriteLine("Check delete : " + datacontext.Id);
            await UpcomingViewModel.WriteFile(NotificationHelper.PRIORITYFILE);
        }

        private async void deleteNewInstance_Click(object sender, RoutedEventArgs e)
        {
            Instance datacontext = (Instance)(e.OriginalSource as FrameworkElement).DataContext;
            int index = InstViewModel.findIndex(datacontext);
            InstViewModel.instances.RemoveAt(index);
            await InstViewModel.WriteFile(NotificationHelper.PRIORITYFILE);
        }

        private async void markImportantUpcoming_Click(object sender, RoutedEventArgs e)
        {
            Instance datacontext = (Instance)(e.OriginalSource as FrameworkElement).DataContext;
            //   Debug.WriteLine("Check click: " + datacontext.Name);
            datacontext.IsImportant = !datacontext.IsImportant;
            await UpcomingViewModel.WriteFile(NotificationHelper.PRIORITYFILE);
        }
    }
}
