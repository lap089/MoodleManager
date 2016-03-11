using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MoodleManager
{
    
    public class CourseManager : INotifyPropertyChanged
    {
       
        public bool Iscompleted;
      
        public bool IsCompleted { get { return Iscompleted; } set {
                Iscompleted = value;
                NotifyPropertyChanged(); } }
      
       
       
        public ObservableCollection<Course> courses = new ObservableCollection<Course>();


        public override string ToString()
        {
            return courses.ElementAt(0).Href + '\n' + courses.ElementAt(1).Href;
        }
        public CourseManager()
        {
          
        }
       
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public async void SaveCourseToFile()
        {
            var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Course>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(NotificationHelper.COURSEFILE, CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, courses);
            }

        }

        public int getIndexCourse(Course course)
        {
            int index = 0;
            foreach (Course cour in courses)
            {
                if (cour.Name.CompareTo(course.Name) == 0)
                    return index;
                ++index;
            }
            return -1;
        }

        public async Task getCoursesFromLocal()
        {
            if (!NotificationHelper.IsFileExist(NotificationHelper.COURSEFILE)) return;

                var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Course>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(NotificationHelper.COURSEFILE))
            {
                courses = (ObservableCollection<Course>)jsonSerializer.ReadObject(stream);
            }
        }

        public async Task getCoursesFromRemote()
        {
            //await NotificationHelper.Connect(NotificationHelper.user, NotificationHelper.pass);
            String categoryUrl = "http://courses.ctdb.hcmus.edu.vn/course/index.php?categoryid=9";
            var req1 = (HttpWebRequest)System.Net.WebRequest.Create(categoryUrl);
            //     System.Net.ServicePointManager.Expect100Continue = false;
            
            req1.CookieContainer = NotificationHelper.cookieContainer;
            var resp = await req1.GetResponseAsync();
            //System.Net.WebResponse resp2 = req2.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            var pageSource = sr.ReadToEnd();

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(pageSource);

            IEnumerable<HtmlNode> findclasses = document.DocumentNode.Descendants("div").Where(d =>
     d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("coursename")
 );
            //  Console.WriteLine(findclasses.Count());
            int count = 0;
            foreach (HtmlNode i in findclasses)
            {
                
                String Href = i.Descendants("a").ElementAt(0).Attributes["href"].Value;
                String Name = i.InnerText;

                    ++count;
                    Course course = new Course(count, Name, Href, "");
                    Addcourse(course);
                
            }
            IsCompleted = true;
        }

        public void Addcourse(Course course)
        { courses.Add(course); }


        //Remove odd instances to save file
        public async Task<ObservableCollection<Course>> RefineInstance()
        {
            CourseManager CmFromLocal = new CourseManager();
            await CmFromLocal.LoadDataFromFileAsync();
            CourseManager CmFromRemote =  (CourseManager) this.MemberwiseClone();
            if (CmFromLocal.courses.Count == 0)
                return courses;
            
            foreach (Course Cremote in CmFromRemote.courses)
            {
                foreach (Course Clocal in CmFromLocal.courses)
                {
                    if (Cremote.Name == Clocal.Name)
                    {
                        foreach (Instance instRemote in Cremote.InstanceActivity.instances)
                        {
                            foreach (Instance instLocal in Clocal.InstanceActivity.instances)
                            {
                                if (instLocal.Name == instRemote.Name)
                                {
                                    instRemote.GetTime = instLocal.GetTime;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }

            }
            return CmFromRemote.courses;
        }



        public async Task SaveDataToFile()
        {
            // await NotificationHelper.Connect(NotificationHelper.user, NotificationHelper.pass);
            CourseManager CmRefined = new CourseManager();
            CmRefined.courses =  await RefineInstance();
           foreach( Course course in CmRefined.courses)
            {
          //      if(course.InstanceActivity.instances.Count() == 0)
          //     course.getInstances(NotificationHelper.cookieContainer);
               await course.InstanceActivity.WriteFile(course.Name+".json");           
            }
        }



        public async Task LoadDataFromRemote()
        {
            await getCoursesFromLocal();
            foreach (Course course in courses)
            {
              await course.getInstances(NotificationHelper.cookieContainer);
            }

            IsCompleted = true;
        }


      


        public async Task LoadDataFromFileAsync()
        {
          await  getCoursesFromLocal();
           
            foreach (Course course in courses)
            {
                if(NotificationHelper.IsFileExist(course.Name + ".json"))
                {
                    await course.InstanceActivity.ReadFile(course.Name + ".json");
                }
            }
        }


        public int Count(int i)
        {
            return courses.ElementAt(i).InstanceActivity.Count();
        }


      

    


}
}
