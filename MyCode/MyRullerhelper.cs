using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.MyCode
{
    enum RullerMode { rmNone, rmSet, rmFirst, rmSecond };
    class MyRullerhelper
    {
        private GMapOverlay _rullerOverlay;

        private RullerMode _mode;
        public RullerMode Mode { get{ return _mode; } 
            private set { if (value == _mode) return;
                _mode = value;
                if (_mode == RullerMode.rmNone) {
                    //clear all
                    _rullerOverlay.Markers.Clear();
                }
            } 
        }

        private PointLatLngAlt _p1;
        private PointLatLngAlt _p2;

        public MyRullerhelper(GMapOverlay rullerOverlay)
        {
            this._rullerOverlay = rullerOverlay;
            Mode = RullerMode.rmNone;
        }



        internal void mouseClickOnMap(GMap.NET.PointLatLng mouseDownLoc)
        {
            switch (Mode)
            {
                case RullerMode.rmNone:
                    break;
                case RullerMode.rmSet:
                    Mode = RullerMode.rmFirst;
                    _p1 = new PointLatLngAlt(mouseDownLoc);
                    _rullerOverlay.Markers.Add(new GMarkerGoogle(mouseDownLoc, GMarkerGoogleType.white_small));
                    break;
                case RullerMode.rmFirst:
                    Mode = RullerMode.rmSecond;
                    _p2 = new PointLatLngAlt(mouseDownLoc);
                    _rullerOverlay.Markers.Add(new GMarkerGoogle(mouseDownLoc, GMarkerGoogleType.white_small));
                    break;
                case RullerMode.rmSecond:
                    Mode = RullerMode.rmNone;
                    break;
                default:
                    break;
            }
           
        }


        internal string doMeasure()
        {

            double dist = _p1.GetDistance(_p2);
            string sDist = (dist > 1000) ? string.Format("{0:0.0} Km", dist / 1000) : string.Format("{0:0} m", dist);
            return sDist;
        }
    }
}

