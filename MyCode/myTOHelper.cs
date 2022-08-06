using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.MyCode
{
    internal class myTOHelper
    {
        internal static void CreateAndUploadTOPlan(float lat, float lng, float altAsl, float yaw, int tOAlt, int wpAlt1, int distToWp1, int wpAlt2, int distToWp2, bool shrtTo, ILog log)
        {
            List<Locationwp> commandlist = createPoints(lat, lng, altAsl, yaw, tOAlt, wpAlt1, distToWp1, wpAlt2, distToWp2, shrtTo);

            Task.Run(async () =>
            {
                await mav_mission.upload(MainV2.comPort, MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, MAVLink.MAV_MISSION_TYPE.MISSION,
                    commandlist,
                    (percent, status) => {}).ConfigureAwait(false);

                try
                {
                    await MainV2.comPort.getHomePositionAsync((byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent).ConfigureAwait(false);
                }
                catch (Exception ex2)
                {
                    log.Error(ex2);
                    try
                    {
                        MainV2.comPort.getWP((byte)MainV2.comPort.sysidcurrent,
                            (byte)MainV2.comPort.compidcurrent, 0);
                    }
                    catch (Exception ex3)
                    {
                        log.Error(ex3);
                    }
                }
            }).GetAwaiter().GetResult();
        }

        private static List<Locationwp> createPoints(float lat, float lng, float altAsl, float yaw, int tOAlt, int wpAlt1, int distToWp1, int wpAlt2, int distToWp2, bool shrtTo)
        {
            List<Locationwp> locationwps = new List<Locationwp>();
            Locationwp home = new Locationwp();
            home.frame = 0;
            home.id = (ushort)MAVLink.MAV_CMD.WAYPOINT;
            home.alt = altAsl;// (float)(double.Parse(items[10], CultureInfo.InvariantCulture));
            home.lat = lat;// (double.Parse(items[8], CultureInfo.InvariantCulture));
            home.lng = lng;// (double.Parse(items[9], CultureInfo.InvariantCulture));

            Locationwp TOCmd = new Locationwp();
            TOCmd.frame = 3;// (byte)int.Parse(items[2], CultureInfo.InvariantCulture);
            TOCmd.id = (ushort)MAVLink.MAV_CMD.VTOL_TAKEOFF; //(ushort)Enum.Parse(typeof(MAVLink.MAV_CMD), items[3], false);
            TOCmd.alt = tOAlt;// (float)(double.Parse(items[10], CultureInfo.InvariantCulture));
            TOCmd.lat = lat;// (double.Parse(items[8], CultureInfo.InvariantCulture));
            TOCmd.lng = lng;// (double.Parse(items[9], CultureInfo.InvariantCulture));
            

            locationwps.Add(home);
            locationwps.Add(TOCmd);

            Locationwp loitCmd = new Locationwp();
            loitCmd.frame = 3;//relative alt...
            loitCmd.id = (ushort)MAVLink.MAV_CMD.LOITER_UNLIM;
            
            

            if (shrtTo)
            {
                Locationwp transToCopter = new Locationwp();
                transToCopter.frame = 3;//relative alt...
                transToCopter.id = (ushort)MAVLink.MAV_CMD.DO_VTOL_TRANSITION;
                transToCopter.p1 = 3;//copter
                locationwps.Add(transToCopter);

                //update location of where to loiter...
                loitCmd.lat = lat;
                loitCmd.lng = lng;
                loitCmd.alt = tOAlt;
            }
            else
            {
                PointLatLngAlt frstPos = new PointLatLngAlt(lat, lng).newpos(yaw, distToWp1);
                PointLatLngAlt scndPos = new PointLatLngAlt(frstPos.Lat, frstPos.Lng).newpos(yaw, distToWp2);

                Locationwp wp1 = new Locationwp();
                wp1.frame = 3;//relative alt...
                wp1.id = (ushort)MAVLink.MAV_CMD.WAYPOINT;
                wp1.alt = wpAlt1;
                wp1.lat = frstPos.Lat;
                wp1.lng = frstPos.Lng;
                locationwps.Add(wp1);

                Locationwp wp2 = new Locationwp();
                wp2.frame = 3;//relative alt...
                wp2.id = (ushort)MAVLink.MAV_CMD.WAYPOINT;
                wp2.alt = wpAlt2;
                wp2.lat = scndPos.Lat;
                wp2.lng = scndPos.Lng;
                locationwps.Add(wp2);

                //update location of where to loiter...
                loitCmd.lat = scndPos.Lat;
                loitCmd.lng = scndPos.Lng;
                loitCmd.alt = wpAlt2;
            }

            locationwps.Add(loitCmd);
            
            return locationwps;
        }

        internal static void resetToFlightPlan(float homeAlt, double homeLat, double homeLng)
        {
            
            Locationwp home = new Locationwp();
            home.frame = 0;
            home.id = (ushort)MAVLink.MAV_CMD.WAYPOINT;
            home.alt = homeAlt;
            home.lat = homeLat;
            home.lng = homeLng;

            List<Locationwp> commandlist = new List<Locationwp>();
            commandlist.Add(home);

            Task.Run(async () =>
            {
                await mav_mission.upload(MainV2.comPort, MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, MAVLink.MAV_MISSION_TYPE.MISSION,
                    commandlist,
                    (percent, status) => { }).ConfigureAwait(false);
               
            }).GetAwaiter().GetResult();

        }
    }
}
