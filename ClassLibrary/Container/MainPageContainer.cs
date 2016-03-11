
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleManager.Container
{
   public class MainPageContainer
    {
        public CourseManager Cm;
        public Instances insts;
        public Instances upcoming;
        public MainPageContainer(CourseManager Cm, Instances insts, Instances upcoming)
        {
            this.Cm = Cm;
            this.insts = insts;
            this.upcoming = upcoming;
        }
    }
}
