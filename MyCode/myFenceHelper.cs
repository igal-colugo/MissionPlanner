using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.MyCode
{
    internal class myFenceHelper
    {
        public static void LoadFenceAndUploadToPlane(string file)
        {
            //read form 
            List<Locationwp> cmds = WaypointFile.ReadWaypointFile(file);
            UploadToPlane(cmds);
        }

        private static void UploadToPlane(List<Locationwp> cmds)
        {
            Task.Run(async () =>
            {
                await mav_mission.upload(MainV2.comPort, MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, MAVLink.MAV_MISSION_TYPE.FENCE,
                    cmds,
                    (percent, status) => { }).ConfigureAwait(false);                
            }).GetAwaiter().GetResult();
        }
    }
}
