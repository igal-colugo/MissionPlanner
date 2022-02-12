using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.MyCode
{
    
    public static class MySettings
    {
        public static readonly string myBasePath =
        Path.Combine(System.IO.Directory.GetParent(System.IO.Directory.GetParent(System.IO.Directory.GetParent(Settings.GetRunningDirectory().ToString()).ToString()).ToString()).ToString(), "Resources");
    }
}
