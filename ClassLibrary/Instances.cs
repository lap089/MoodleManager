using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MoodleManager
{
    
   public class Instances : INotifyPropertyChanged
    {

      
        public bool iscompleted;
      
        public bool IsCompleted
        {
            get { return iscompleted; }
            set
            {
                iscompleted = value;
                NotifyPropertyChanged();
            }
        }
      
        public ObservableCollection<Instance> instances = new ObservableCollection<Instance>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
     
     
        public void Addinstance(Instance instance)
            {
                instances.Add(instance);
            }

            public int Count()
            {
                return instances.Count;
            }

            public void PrintData()
            {
                foreach (Instance ins in instances)
                {
                    ins.Print();
                }
            }

        public void Complete()       /// Trigger stop loading control
        {
            IsCompleted = true;
        }

        public async Task WriteFile(String filename)
        {
            var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Instance>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(filename, CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, instances);
            }
        }


        public async Task<ObservableCollection<Instance>> RefineInstance()
        {
            Instances instFromLocal = new Instances();
            await instFromLocal.ReadFile(NotificationHelper.NEWINSTANCEFILE);
            Instances instFromRemote = (Instances)this.MemberwiseClone();
            if (instFromLocal.instances.Count == 0)
                return instances;

                        foreach (Instance instRemote in instFromRemote.instances)
                        {
                            foreach (Instance instLocal in instFromLocal.instances)
                            {
                                if (instLocal.Name == instRemote.Name)
                                {
                                    instRemote.GetTime = instLocal.GetTime;
                                    break;
                                }
                            }
                        }
                       
            return instFromRemote.instances;
        }


        public async Task ReadFile(String filename)
        {
            if (!NotificationHelper.IsFileExist(filename)) return;

            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Instance>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename))
            {
                instances = (ObservableCollection<Instance>)jsonSerializer.ReadObject(stream);
            }

            if (filename == NotificationHelper.NEWINSTANCEFILE)
            {
                // Remove old items based on limit day:
                foreach (Instance inst in instances)
                {
                    DateTime now = DateTime.Now;
                    if (inst.GetTime.Subtract(now).Days > NotificationHelper.LIMITDAYS)
                    {
                        instances.Remove(inst);
                    }
                }
            }
        }

       


        public async Task getUpcomingEvents()
        {
            
            //await NotificationHelper.Connect(NotificationHelper.user, NotificationHelper.pass);
            int count = 0;
            CourseManager Cm = new CourseManager();
           await Cm.getCoursesFromLocal();
            foreach (Course course in Cm.courses)
            {            
                HtmlDocument document = await NotificationHelper.GetPageSource(course.Href, NotificationHelper.cookieContainer);
                IEnumerable<HtmlNode> findclasses = document.DocumentNode.Descendants("div").Where(d =>
           d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("event")
           );
                
                //  Debug.WriteLine();
                foreach (HtmlNode i in findclasses)
                {
                    String Name = i.InnerText;
                    String Href = i.Descendants("a").ElementAt(0).Attributes["href"].Value;
                    String Scr = i.Descendants("img").ElementAt(0).Attributes["src"].Value;
                    Debug.WriteLine("Check getevent: " + Scr);
                    Instance inst = new Instance(count, Name, Href, Scr, false);
                    instances.Add(inst);
                    IsCompleted = true;
                }
                ++count;
              
            }
            IsCompleted = true;

        }


        public void ComparationProcess(CourseManager CmFromRemote, CourseManager CmFromLocal)
        {
            int index = 0;
            foreach (Course course in CmFromRemote.courses)
            {
                Instances tempIns = course.CompareTo(CmFromLocal.courses[index].InstanceActivity);
                if (tempIns != null)
                {
                
                    foreach (Instance ins in tempIns.instances.Reverse())
                    {
                        instances.Add(ins);
                     //   Debug.WriteLine("Check instances " + ins.Name);
                    }
                }
                ++index;
            }
            //    newIns.Complete();
           IsCompleted = true;
        }


        public Instances CompareTo(Instances insts)
            {
                Instances NewInsts = new Instances();

                if (insts.Count() > Count())
                {
                    for (int i = 0; i < insts.Count(); ++i)
                    {
                        bool check = false;
                        for (int j = 0; j < Count(); ++j)
                        {
                            if (insts.instances[i].CompareTo(instances[j]))
                                check = true;


                        }
                        if (!check)
                            NewInsts.Addinstance(insts.instances[i]);

                    }
                }

                else if (insts.Count() < Count())
                {

                    for (int i = 0; i < Count(); ++i)
                    {
                        bool check = false;
                        for (int j = 0; j < insts.Count(); ++j)
                        {
                            if (insts.instances[i].CompareTo(instances[j]))
                                check = true;


                        }
                        if (!check)
                            NewInsts.Addinstance(instances[i]);

                    }

                }
                else return null;
           
                return NewInsts;
            }

        public int findIndex(Instance inst)
        {
            int count = 0;
            foreach (Instance i in instances)
            {
                if (i.Name == inst.Name)
                    return count;
                ++count;
            }
            return -1;
        }


        public async void checkPriority()
        {
            Instances insts = new Instances();
            await insts.ReadFile(NotificationHelper.PRIORITYFILE);
            foreach (Instance i in instances)
                foreach (Instance j in insts.instances)
                    if (i.Name == j.Name)
                        i.IsImportant = j.IsImportant;
            
        }
    }
}
