using BackgroundNotification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();
        }



        private async void triggerButton_Toggled(object sender, RoutedEventArgs e)
        {
            if (triggerButton.IsOn)
            {
                await BackgroundHelper.Register<BackgroundNotifier>(new TimeTrigger(NotificationHelper.TIMENOTIFICATION, false),
                                                               new SystemCondition[] { new SystemCondition(SystemConditionType.InternetAvailable),
                                                                new SystemCondition(SystemConditionType.UserPresent) });


                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["Trigger"] = "ON";
                NotifyTime.Visibility = Visibility.Visible;
            }
            else {
                var result = await BackgroundHelper.Unregister<BackgroundNotifier>();
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["Trigger"] = "OFF";
                NotifyTime.Visibility = Visibility.Collapsed;
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["Trigger"] == null)
            {
                triggerButton.IsOn = false;
                NotifyTime.Visibility = Visibility.Collapsed;
            }
            else if (localSettings.Values["Trigger"].ToString() == "ON")
            {
                triggerButton.IsOn = true;
                NotifyTime.Visibility = Visibility.Visible;
            }
            else
            {
                triggerButton.IsOn = false;
                NotifyTime.Visibility = Visibility.Collapsed;
            }

            
            if (NotificationHelper.TIMENOTIFICATION == 30)
                NotifyTime.SelectedIndex = 0;
            else if (NotificationHelper.TIMENOTIFICATION == 60)
                NotifyTime.SelectedIndex = 1;
            else if (NotificationHelper.TIMENOTIFICATION == 120)
                NotifyTime.SelectedIndex = 2;
            else if (NotificationHelper.TIMENOTIFICATION == 360)
                NotifyTime.SelectedIndex = 3;
            else if (NotificationHelper.TIMENOTIFICATION == 720)
                NotifyTime.SelectedIndex = 4;
            else
                NotifyTime.SelectedIndex = 5;

            if (NotificationHelper.LIMITDAYS == 7)
                LimitDay.SelectedIndex = 0;
            else if (NotificationHelper.LIMITDAYS == 14)
                LimitDay.SelectedIndex = 1;
            else if (NotificationHelper.LIMITDAYS == 21)
                LimitDay.SelectedIndex = 2;
            else LimitDay.SelectedIndex = 3;
        }

        private void NotifyTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NotificationHelper.TIMENOTIFICATION = (uint)(int) NotifyTime.SelectedValue;
        }

        private void LimitDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)LimitDay.SelectedValue == "1 week")
                NotificationHelper.LIMITDAYS = 7;
            else if ((string)LimitDay.SelectedValue == "2 weeks")
                NotificationHelper.LIMITDAYS = 14;
            else if ((string)LimitDay.SelectedValue == "3 weeks")
                NotificationHelper.LIMITDAYS = 21;
            else 
                NotificationHelper.LIMITDAYS = 28;
        }
    }
}
