using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MoodleManager
{
    [DataContract]
    public class Instance : INotifyPropertyChanged
    {

        public bool isImportant;

        [DataMember]
        public int Id { set; get; }
        [DataMember]
        public String Name { set; get; }
        [DataMember]
        public String Href { set; get; }
        [DataMember]
        public String Scr { set; get; }
        [DataMember]
        public bool IsImportant { set {
                isImportant = value;
                NotifyPropertyChanged();
            }
            get { return isImportant; } }
        [DataMember]
        public DateTime GetTime { set; get; }


        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public Instance(int Id, String name, String href, String scr, bool isImportant)
        {
            this.Id = Id;
            this.Name = name;
            this.Scr = scr;
            this.Href = href;
            this.IsImportant = isImportant;
            this.GetTime = DateTime.Now;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Print()
        {
            //  Console.WriteLine(Name + '\n' + Scr + '\n' + Href + '\n' + '\n');
        }

        //public IEnumerable<String> getLines(IEnumerable<String> ContentLines)
        //{

        //    ContentLines = ContentLines.Concat(new[] { Name });
        //    ContentLines = ContentLines.Concat(new[] { Href });
        //    ContentLines = ContentLines.Concat(new[] { Scr });
        //    ContentLines = ContentLines.Concat(new[] { Id.ToString() });
        //    return ContentLines;
        // //   await Windows.Storage.FileIO.WriteTextAsync(file, "Test line...");
        //}

        public bool CompareTo(Instance ins)
        {
            if (ins.Name == this.Name)
                return true;
            else return false;
        }


    }

}
