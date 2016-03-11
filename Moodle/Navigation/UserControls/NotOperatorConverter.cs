using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MoodleManager.Navigation.UserControls
{
    class NotOperatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isVisible = (bool)value;
            return !isVisible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool isVisible = (bool)value;
            return !isVisible;
        }
    }
}
