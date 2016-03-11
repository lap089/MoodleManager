using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ClassLibrary.UserControls
{
    public sealed partial class MyCustomLoginProcess : UserControl
    {

        public MyCustomLoginProcess()
        {
            this.InitializeComponent();
            Storyboard1.Begin();
     
        }


        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value);
                Storyboard1.Stop();
                if(value)  Storyboard1.Begin();
                Debug.WriteLine("Check Visibility login: " + value.ToString());
            }
        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisibleProperty", typeof(bool), typeof(MyCustomLoginProcess), new PropertyMetadata(false));


    }
}
