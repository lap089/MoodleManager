

using MoodleManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;


namespace BackgroundNotification
{
    public sealed class BackgroundNotifier : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            if (cost == BackgroundWorkCostValue.High)
            {
                Debug.WriteLine("Background task aborted (cost is high)");
            }
            else
            {
                // handle canceled
                taskInstance.Canceled += (s, e) =>
                {
                    Debug.WriteLine("Background task canceled");
                };

                // perform task
                var deferral = taskInstance.GetDeferral();
                try
                {
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    if (localSettings.Values["Username"] == null)
                        return;
                    string user = localSettings.Values["Username"].ToString();
                    string pass = localSettings.Values["Password"].ToString();
                    await NotificationHelper.Connect(user, pass);

                    CourseManager CmFromRemote = new CourseManager();
                    CourseManager CmFromLocal = new CourseManager();
                    await CmFromRemote.LoadDataFromRemote();
                    await CmFromLocal.LoadDataFromFileAsync();
                    Instances newInsts = new Instances();
                    newInsts = NotificationHelper.ComparationProcess(CmFromRemote, CmFromLocal);
                    if (newInsts.Count() != 0)
                    {
                        //  NotificationHelper.UpdateTile(newInsts);
                        //   else
                        newInsts = NotificationHelper.SortByDate(newInsts);
                        NotificationHelper.UpdateToast(newInsts);
                        NotificationHelper.MakeTile(newInsts);
                        await newInsts.WriteFile(NotificationHelper.NEWINSTANCEFILE);
                    }

                    Debug.WriteLine("Background task complete: ");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Background task error: " + ex.Message);
                }
                finally
                {
                    deferral.Complete();
                }

            }

        }






    }
}
