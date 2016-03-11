using MoodleManager.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class SignInPage : Page
    {

        public SignInPage()
        {
            this.InitializeComponent();
        }

        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            errorMessage.Visibility = Visibility.Collapsed;
            string user =  username.Text;
            string pass =  password.Password;
            username.Visibility = Visibility.Collapsed;
            password.Visibility = Visibility.Collapsed;
            signInButton.Visibility = Visibility.Collapsed;
            remember.Visibility = Visibility.Collapsed;

            logging.IsVisible = true;
           // errorMessage.Text = "wait...!";
           
            var result =  await NotificationHelper.Connect(user, pass);
            //    var result = "not logged in";
           // await Task.Delay(1000);
            if (result.Contains("not logged in"))
            {
                username.Visibility = Visibility.Visible;
                password.Visibility = Visibility.Visible;
                signInButton.Visibility = Visibility.Visible;
                remember.Visibility = Visibility.Visible;
               
                logging.IsVisible = false;
             
                errorMessage.Visibility = Visibility.Visible;
                errorMessage.Text = "Wrong password or username!";
            }
            else
            {
                
                errorMessage.Text = "";
                errorMessage.Visibility = Visibility.Collapsed;
                username.Visibility = Visibility.Visible;
                password.Visibility = Visibility.Visible;
                signInButton.Visibility = Visibility.Visible;
                remember.Visibility = Visibility.Visible;
                logging.IsVisible = false;
                if (remember.IsChecked.Value)
                {
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    localSettings.Values["Username"] = user;
                    localSettings.Values["Password"] = pass;
                    localSettings.Values["Remember"] = "Checked";
                }
                Frame.Navigate(typeof(AppShell));

               // NotificationHelper.logOut();
               
                //var req1 = (HttpWebRequest)System.Net.WebRequest.Create("http://courses.ctdb.hcmus.edu.vn/login/index.php");
                //// System.Net.ServicePointManager.Expect100Continue = false;
                //req1.CookieContainer = null;
                //var resp = req1.GetResponseAsync();
                //StreamReader sr = new StreamReader(resp.Result.GetResponseStream());
                //var pageSource = sr.ReadToEnd();
                //Debug.WriteLine(pageSource);
            }

         //   errorMessage.Visibility = Visibility.Collapsed;
        }

        private void username_LostFocus(object sender, RoutedEventArgs e)
        {

        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var check = localSettings.Values["Remember"];
            if (e != null && e.Parameter.ToString() == "SignOut")
            {

                if (check != null && check.ToString() == "Checked")
                {
                    remember.IsChecked = true;
                    username.Text = localSettings.Values["Username"].ToString();
                    password.Password = localSettings.Values["Password"].ToString();
                }
                return;
            }
            else if (check == null)
                return;
            username.Visibility = Visibility.Collapsed;
            password.Visibility = Visibility.Collapsed;
            signInButton.Visibility = Visibility.Collapsed;
            remember.Visibility = Visibility.Collapsed;
            logging.IsVisible = true;
            errorMessage.Visibility = Visibility.Collapsed;
            if (check!= null  &&   check.ToString() == "Checked")
            {
               // errorMessage.Text = "Logging in...";
                Debug.WriteLine("Check login: " + check.ToString());
                await NotificationHelper.Connect(localSettings.Values["Username"].ToString(), localSettings.Values["Password"].ToString());
                Frame.Navigate(typeof(AppShell));
                logging.IsVisible = false;
            }
        }
    }
}
