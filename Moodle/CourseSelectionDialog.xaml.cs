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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MoodleManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public enum SelectionResult
    {
        OK,
        Cancel,
        Nothing
    }

    public sealed partial class CourseSelectionDialog : ContentDialog
    {
        

        public SelectionResult result;
        public CourseManager CourseViewModel = new CourseManager();
        public CourseSelectionDialog()
        {
            this.InitializeComponent();
            this.Opened += CourseSelectionDialog_Opened;
          
        }

        private async void CourseSelectionDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            await CourseViewModel.getCoursesFromRemote();
            Windows.Storage.StorageFolder storageFolder =
             Windows.Storage.ApplicationData.Current.LocalFolder;
            var checkExist = storageFolder.TryGetItemAsync(NotificationHelper.COURSEFILE);
            if (checkExist != null) {
                CourseManager existingCourses = new CourseManager();
                 await  existingCourses.getCoursesFromLocal();
               // await existingCourses.getCoursesFromRemote();
                foreach(Course course in existingCourses.courses)
                {
                    if (CourseViewModel.courses.Contains(course))
                    {
                        var item = selectioncourse.Items[CourseViewModel.getIndexCourse(course)];
                        selectioncourse.SelectedItems.Add(item);
                    }
                }
            }
         
        }

        private void courseselection_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            CourseManager Selectedcourses = new CourseManager();
            int count = 0;
            foreach (Course course in selectioncourse.SelectedItems)
            {
                course.Id = count;
                Selectedcourses.Addcourse(course);
                ++count;
            }
            Selectedcourses.SaveCourseToFile();
           this.result =  SelectionResult.OK;
        }

        private void courseselection_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.result = SelectionResult.Cancel;
        }

        private void contentDialog_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        //    selectioncourse.Height = Window.Current.Bounds.Height * 0.6;
        //    Debug.WriteLine(Window.Current.Bounds.Height);
        }
    }
}
