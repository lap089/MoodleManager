using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MoodleManager.Controls
{
    public class AlternatingRowListView : ListView
    {
        public static readonly DependencyProperty FirstRowBackgroundProperty =
            DependencyProperty.Register("FirstRowBackground", typeof(Brush), typeof(AlternatingRowListView), null);
        public Brush FirstRowBackground
        {
            get { return (Brush)GetValue(FirstRowBackgroundProperty); }
            set { SetValue(FirstRowBackgroundProperty, (Brush)value); }
        }

        public static readonly DependencyProperty SecondRowBackgroundProperty =
            DependencyProperty.Register("SecondRowBackground", typeof(Brush), typeof(AlternatingRowListView), null);
        public Brush SecondRowBackground
        {
            get { return (Brush)GetValue(SecondRowBackgroundProperty); }
            set { SetValue(SecondRowBackgroundProperty, (Brush)value); }
        }

        public static readonly DependencyProperty ThirdRowBackgroundProperty =
           DependencyProperty.Register("ThirdRowBackground", typeof(Brush), typeof(AlternatingRowListView), null);
        public Brush ThirdRowBackground
        {
            get { return (Brush)GetValue(ThirdRowBackgroundProperty); }
            set { SetValue(ThirdRowBackgroundProperty, (Brush)value); }
        }

        public static readonly DependencyProperty FourthRowBackgroundProperty =
          DependencyProperty.Register("FourthRowBackground", typeof(Brush), typeof(AlternatingRowListView), null);
        public Brush FourthRowBackground
        {
            get { return (Brush)GetValue(FourthRowBackgroundProperty); }
            set { SetValue(FourthRowBackgroundProperty, (Brush)value); }
        }

        public static readonly DependencyProperty FifthRowBackgroundProperty =
          DependencyProperty.Register("FifthRowBackground", typeof(Brush), typeof(AlternatingRowListView), null);
        public Brush FifthRowBackground
        {
            get { return (Brush)GetValue(FifthRowBackgroundProperty); }
            set { SetValue(FifthRowBackgroundProperty, (Brush)value); }
        }

        public static readonly DependencyProperty SixthRowBackgroundProperty =
          DependencyProperty.Register("SixthRowBackground", typeof(Brush), typeof(AlternatingRowListView), null);
        public Brush SixthRowBackground
        {
            get { return (Brush)GetValue(SixthRowBackgroundProperty); }
            set { SetValue(SixthRowBackgroundProperty, (Brush)value); }
        }

        public static readonly DependencyProperty SeventhRowBackgroundProperty =
          DependencyProperty.Register("SeventhRowBackground", typeof(Brush), typeof(AlternatingRowListView), null);
        public Brush SeventhRowBackground
        {
            get { return (Brush)GetValue(SeventhRowBackgroundProperty); }
            set { SetValue(SeventhRowBackgroundProperty, (Brush)value); }
        }

      

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var listViewItem = element as ListViewItem;
           

            if (listViewItem != null)
            {

                var index = 0;
                if (item is Course)
                {
                    index = IndexFromContainer(element);
                    Debug.WriteLine("Check type list: Course " + (index) );  
                }
                    
                else if (item is Instance) {

                    Instance ins = item as Instance;
                    index = ins.Id;
                    Debug.WriteLine("Check type list: Instance " + (index));
                }

                index += 1;


                switch (index)
                {
                    case 1:
                        listViewItem.Background = FirstRowBackground;
                        break;
                    case 2:
                        listViewItem.Background = SecondRowBackground;
                        break;
                    case 3:
                        listViewItem.Background = ThirdRowBackground;
                        break;
                    case 4:
                        listViewItem.Background = FourthRowBackground;
                        break;
                    case 5:
                        listViewItem.Background = FifthRowBackground;
                        break;
                    case 6:
                        listViewItem.Background = SixthRowBackground;
                        break;
                    case 7:
                        listViewItem.Background = SeventhRowBackground;
                        break;

                }
            }

        }
    }
}
