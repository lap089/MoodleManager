
using MoodleManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleManager.Container
{
  public class InstancePageContainer
    {
        public InstancePageContainer(CourseManager cm, Instances newInst,Instances upcoming,int index)
        {
            this.courseManager = cm;
            this.clickedIndex = index;
            this.newInst = newInst;
            this.upcoming = upcoming;
        }
        public Instances newInst;
        public Instances upcoming;
        public CourseManager courseManager;
        public int clickedIndex;
    }
}
