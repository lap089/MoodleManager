
using HtmlAgilityPack;
using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;


namespace MoodleManager
{
    public class NotificationHelper
    {
       

        public static String PRIORITYFILE = "Priority.json";
        public static String NEWINSTANCEFILE = "NewInstances.json";
        public static String COURSEFILE = "SelectedCourses.json";
        public static uint TIMENOTIFICATION = 60;
        public static int LIMITDAYS = 14;

        public static CookieContainer cookieContainer;

        public static string formUrl = "http://courses.ctdb.hcmus.edu.vn/login/index.php";
        public async static Task<String> Connect(String username, String password)
        {

            string formParams = string.Format("username={0}&password={1}", username, password);
            HttpWebRequest req2 = (HttpWebRequest)System.Net.WebRequest.Create(formUrl);



            //System.Net.ServicePointManager.Expect100Continue = false;
            // req2.CookieContainer = new CookieContainer();//req1.CookieContainer;
            req2.ContentType = "application/x-www-form-urlencoded";
            req2.Method = "POST";
            byte[] bytes1 = Encoding.ASCII.GetBytes(formParams);

            // req2.ContentLength = bytes1.Length;

            using (var stream = await Task.Factory.FromAsync(req2.BeginGetRequestStream, req2.EndGetRequestStream, null))
            {
                stream.Write(bytes1, 0, bytes1.Length);
            }


            //var resp1 = await Task.Factory.FromAsync(req2.BeginGetResponse, req2.EndGetResponse, null);
            var resp = await req2.GetResponseAsync();

            //System.Net.WebResponse resp2 = req2.GetResponse();
            cookieContainer = req2.CookieContainer;

            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                var pageSource = sr.ReadToEnd();
                if (pageSource.Contains("not logged in"))
                    Debug.WriteLine("Not logged in");
                else Debug.WriteLine("Logged in");
                resp.Dispose();
                return pageSource;
            }


        }



        public static Instances SortByDate(Instances insts)
        {
            for (int i = 0; i < insts.instances.Count; ++i)
                for (int j = i + 1; j < insts.instances.Count; ++j)
                    if (insts.instances[i].GetTime.CompareTo(insts.instances[j].GetTime) == -1)
                    {
                        DateTime temp = insts.instances[i].GetTime;
                        var inst = insts.instances[i];
                        insts.instances[i] = insts.instances[j]; 
                        insts.instances[j] = inst;
                    }
            return insts;
        }


        public static async void logOut()
        {
            HtmlDocument document = await GetPageSource(formUrl, null);
            IEnumerable<HtmlNode> findclasses = document.DocumentNode.Descendants("div").Where(d =>
     d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("logininfo")
     );
            String Href = findclasses.ElementAt(0).Descendants("a").ElementAt(1).Attributes["href"].Value;
            var req1 = (HttpWebRequest)System.Net.WebRequest.Create(Href);
            var resp = await req1.GetResponseAsync();
            //StreamReader sr = new StreamReader(resp.GetResponseStream());
            //var pageSource = sr.ReadToEnd();
            //    Debug.WriteLine("Check logout: " + pageSource);
        }

        public static Instances ComparationProcess(CourseManager CmFromRemote, CourseManager CmFromLocal)
        {
            Instances newIns = new Instances();
            int index = 0;
            foreach (Course course in CmFromRemote.courses)
            {
                Instances tempIns = course.CompareTo(CmFromLocal.courses[index].InstanceActivity);
                if (tempIns != null)
                    newIns = ConcateIns(tempIns, newIns);
                ++index;
            }
            //    newIns.Complete();
            newIns.IsCompleted = true;
            return newIns;
        }


        public static Instances ConcateIns(Instances source, Instances dest)
        {
            foreach (Instance ins in source.instances)
            {
                dest.Addinstance(ins);
            }
            return dest;
        }


        public static void MakeTile(Instances newInsts)
        {
            var content = $@"";
            for (int i = 0; i < newInsts.instances.Count(); ++i)
            {
                if (i == 1) break;
                content = $@"<tile>
<visual><binding template=""TileMedium"" branding=""name"">
  <image placement = ""peek"" src = ""Assets/Square44x44Logo.scale-200.png""/>
      <text hint-style=""captionsubtle"">{newInsts.instances.ElementAt(i).Name}</text>
         <image  hint-crop = ""circle""  placement = ""inline"" src=""{newInsts.instances.ElementAt(i).Scr}"" hint-removeMargin=""false""/>
</binding>
<binding template=""TileWide"" branding=""name"">
  <image placement = ""peek"" src = ""Assets/Wide310x150Logo.scale-200.png""/>
      <text hint-style=""captionsubtle"">{newInsts.instances.ElementAt(i).Name}</text>
         <image   placement = ""inline"" src=""{newInsts.instances.ElementAt(i).Scr}"" hint-removeMargin=""false""/>
</binding>
               </visual>
               </tile>
                ";

                Debug.WriteLine(content.ToString());
                var xml = new XmlDocument();
                xml.LoadXml(content);
                var notification = new TileNotification(xml);
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
            }     
        }



     



        public static void UpdateToast(Instances newInts)
        {
            String content = "";
            int count = 0;
            newInts = NotificationHelper.SortByDate(newInts);
            foreach (Instance inst in newInts.instances)
            {
                ++count;
                if (count >= 4)
                {
                    content += "...    " + '\n';
                    break;
                }
                content += inst.Name + '\n';
            }
            var template = $@"
            <toast>
            <visual>
            <binding template=""ToastGeneric"">
            <image placement=""AppLogoOverride"" src=""Assets\finance.png"" />
                 <text>{" " + newInts.instances.Count()+ " new instance(s) from Moodle! :"}</text>
                <text>{content}</text>
            </binding>
            </visual>
            <audio src=""ms-winsoundevent:Notification.Looping.Alarm2"" loop=""true""  />
            </toast>
            ";

            var xml = new XmlDocument();
            xml.LoadXml(template);

            var toast = new ToastNotification(xml);
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);

          //  Debug.WriteLine("Background task toast shown: " + .ToString());
        }


        public static bool IsFileExist(String filename)
        {
            string directory = ApplicationData.Current.LocalFolder.Path;
            string filePath = Path.Combine(directory, filename);

            if (File.Exists(filePath))
                return true;
            else return false;
        }

      



        public static async Task<HtmlDocument> GetPageSource(String url, CookieContainer cookieContainer)
        {

            String DetailUrl = url;
            var req1 = (HttpWebRequest)System.Net.WebRequest.Create(DetailUrl);
            // System.Net.ServicePointManager.Expect100Continue = false;
            req1.CookieContainer = cookieContainer;
            var resp = await req1.GetResponseAsync();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            var pageSource = sr.ReadToEnd();
            //   Debug.WriteLine(pageSource);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(pageSource);
            return document;
        }

    }

}
