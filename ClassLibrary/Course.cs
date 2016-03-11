using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace MoodleManager
{

    [DataContract]
    public class Course : INotifyPropertyChanged
    {
        
        [DataMember]  public Instances InstanceActivity { get; set; }
        private String name;
        [DataMember] public String Name {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }

        [DataMember]  public String Href { get; set; }
        [DataMember]  public String Src { get; set; }
        [DataMember]  public int Id { get; set; }

        public Course(int Id, String Name, String Href, String Src)
        {
            this.InstanceActivity = new Instances();
            this.Name = Name;
            this.Href = Href;
            this.Src = Src;
            this.Id = Id;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public sealed override string ToString()
        {
            return Id.ToString() +  " - " + Name + " -- " + Href; 
        }

        public async Task getInstances(CookieContainer cookie)
        {
            HtmlDocument document = await NotificationHelper.GetPageSource(Href, cookie);
            IEnumerable<HtmlNode> findclasses = document.DocumentNode.Descendants("div").Where(d =>
            d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("activityinstance")
            );


            foreach (HtmlNode i in findclasses)
            {
                String Name = i.InnerText;
                if (Name.Contains("News forum"))
                    continue;
                String Href = i.Descendants("a").ElementAt(0).Attributes["href"].Value;
                String Scr = i.Descendants("img").ElementAt(0).Attributes["src"].Value;
                Instance ins = new Instance(Id, Name, Href, Scr,false);
                if (InstanceActivity == null)
                    InstanceActivity = new Instances();
                InstanceActivity.Addinstance(ins);
            }
        }


        //public IEnumerable<String> getLines(IEnumerable<String> ContentLines)
        //{
        //    ContentLines = ContentLines.Concat(new[] { Name });
        //    ContentLines = ContentLines.Concat(new[] { Href });
        //    ContentLines = ContentLines.Concat(new[] { Src });
        //    ContentLines = ContentLines.Concat(new[] { Id.ToString() });
        //    return ContentLines;
        //    //   await Windows.Storage.FileIO.WriteTextAsync(file, "Test line...");
        //}


        public int Count()
        {
            return InstanceActivity.Count();
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
                        if (insts.instances[i].CompareTo(InstanceActivity.instances[j]))
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
                        if (InstanceActivity.instances[i].CompareTo(insts.instances[j]))
                            check = true;


                    }
                    if (!check)
                        NewInsts.Addinstance(InstanceActivity.instances[i]);

                }

            }
            else return null;
            return NewInsts;
        }



    }
}