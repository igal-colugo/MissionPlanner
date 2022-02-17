using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Controls;
using MissionPlanner.Maps;
using MissionPlanner.MyCode;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    public class MyMarkersLayer
    {
        /// <summary>
        /// Store points of interest
        /// </summary>
        static ObservableCollection<PointLatLngAlt> myMarkers = new ObservableCollection<PointLatLngAlt>();

        private static EventHandler _POIModified;

        public static event EventHandler POIModified
        {
            add
            {
                _POIModified += value;
                try
                {
                   // if (File.Exists(filename))
                    //    LoadFile(filename);
                }
                catch
                {
                }
            }
            remove { _POIModified -= value; }
        }

        private static string filename = Path.Combine(MySettings.myBasePath, "targets") + "\\targets.txt";
        private static bool loading;

        static MyMarkersLayer()
        {
            myMarkers.CollectionChanged += POIs_CollectionChanged;
        }

        private static void POIs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (loading)
                    return;
            //    SaveFile(filename);
            }
            catch { }
        }

        public static void POIAdd(PointLatLngAlt Point, string imageIndex)
        {
            // local copy
            PointLatLngAlt pnt = Point;

            pnt.Tag = pnt.ToString();
            pnt.Tag2 = imageIndex+"\n";

            MyMarkersLayer.myMarkers.Add(pnt);

            if (_POIModified != null && !loading)
                _POIModified(null, null);
        }


        public static void POIDelete(GMyMarkerGoogle Point)
        {
            if (Point == null)
                return;

            for (int a = 0; a < MyMarkersLayer.myMarkers.Count; a++)
            {
                if (MyMarkersLayer.myMarkers[a].Point() == Point.Position)
                {
                    MyMarkersLayer.myMarkers.RemoveAt(a);
                    if (_POIModified != null)
                        _POIModified(null, null);
                    return;
                }
            }
        }

        public static void POIEdit(GMyMarkerGoogle Point, string imageIndex)
        {
            if (Point == null)
                return;

       
            for (int a = 0; a < MyMarkersLayer.myMarkers.Count; a++)
            {
                if (MyMarkersLayer.myMarkers[a].Point() == Point.Position)
                {
                    MyMarkersLayer.myMarkers[a].Tag = imageIndex + "\n" + Point.Position.ToString();
                    MyMarkersLayer.myMarkers[a].Tag2 = imageIndex + "\n";
                    if (_POIModified != null)
                        _POIModified(null, null);
                    return;
                }
            }
        }

        public static void POIMove(GMyMarkerGoogle marker, double newLat, double newLng)
        {

           
            for (int a = 0; a < MyMarkersLayer.myMarkers.Count; a++)
            {
                if (MyMarkersLayer.myMarkers[a].Point() == marker.Position)
                {
                    MyMarkersLayer.myMarkers[a].Lat = newLat;
                    MyMarkersLayer.myMarkers[a].Lng = newLng;

                    MyMarkersLayer.myMarkers[a].Tag = "rrr" + "\n" + marker.Position.ToString();
                    MyMarkersLayer.myMarkers[a].Tag2 = "1" + "\n";
                    if (_POIModified != null)
                        _POIModified(null, null);
                    return;
                }
            }

/*
                for (int a = 0; a < poioverlay.Markers.Count; a++)
            {
                if (myMarkers[a] == marker)
                {
                    myMarkers[a].Lat = newLat;
                    myMarkers[a].Lng = newLng;
                    myMarkers[a].Tag = myMarkers[a].Tag.Substring(0, myMarkers[a].Tag.IndexOf('\n')) + "\n" + marker.Position.ToString();
                    break;
                }
            }
*/
            if (_POIModified != null)
                _POIModified(null, null);
        }

        public static void POISave()
        {
            SaveFile(filename);                
        }

        private static void SaveFile(string fileName)
        {
            using (Stream file = File.Open(fileName, FileMode.Create))
            {
                foreach (var item in MyMarkersLayer.myMarkers)
                {
                    string line = item.Lat.ToString(CultureInfo.InvariantCulture) + "\t" +
                                  item.Lng.ToString(CultureInfo.InvariantCulture) + "\t" + item.Tag2.Substring(0, item.Tag2.IndexOf('\n')) + "\r\n";
                    byte[] buffer = ASCIIEncoding.ASCII.GetBytes(line);
                    file.Write(buffer, 0, buffer.Length);
                }
            }
        }



        public static void LoadFile()
        {
            loading = true;
            using (Stream file = File.Open(filename, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    while (!sr.EndOfStream)
                    {
                        string[] items = sr.ReadLine().Split('\t');

                        if (items.Count() < 3)
                            continue;

                        POIAdd(new PointLatLngAlt(double.Parse(items[0], CultureInfo.InvariantCulture)
                            , double.Parse(items[1], CultureInfo.InvariantCulture)), items[2]);
                    }
                }
            }
            loading = false;
            // redraw now
            if (_POIModified != null)
                _POIModified(null, null);
        }

        public static void UpdateOverlay(GMap.NET.WindowsForms.GMapOverlay poioverlay, ImageList ilMyImages)
        {
            if (poioverlay == null)
                return;

            poioverlay.Clear();

            foreach (var pnt in myMarkers)
            {
                poioverlay.Markers.Add(new GMyMarkerGoogle(pnt, ilMyImages, int.Parse(pnt.Tag2))
                {
                    ToolTipMode = MarkerTooltipMode.OnMouseOver,
                    ToolTipText = pnt.Tag + " Type(" + pnt.Tag2 +")"
                });
            }
        }

        internal static void ClearAll()
        {
            myMarkers.Clear();
            // redraw now
            if (_POIModified != null)
                _POIModified(null, null);
        }
    }
}