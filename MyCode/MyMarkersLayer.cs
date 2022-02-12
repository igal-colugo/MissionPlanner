using GMap.NET.WindowsForms;
using MissionPlanner.Controls;
using MissionPlanner.Maps;
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
                    if (File.Exists(filename))
                        LoadFile(filename);
                }
                catch
                {
                }
            }
            remove { _POIModified -= value; }
        }

        private static string filename = Settings.GetUserDataDirectory() + "poi.txt";
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
                SaveFile(filename);
            }
            catch { }
        }

        public static void POIAdd(PointLatLngAlt Point, string tag)
        {
            // local copy
            PointLatLngAlt pnt = Point;

            pnt.Tag = tag + "\n" + pnt.ToString();

            MyMarkersLayer.myMarkers.Add(pnt);

            if (_POIModified != null && !loading)
                _POIModified(null, null);
        }

        public static void POIAdd(PointLatLngAlt Point)
        {
            if (Point == null)
                return;

            PointLatLngAlt pnt = Point;

            string output = "";

            if (DialogResult.OK != InputBox.Show("POI", "Enter ID", ref output))
                return;

            POIAdd(Point, output);
        }

        public static void POIDelete(GMapMarkerPOI Point)
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

        public static void POIEdit(GMapMarkerPOI Point)
        {
            if (Point == null)
                return;

            string output = "";

            if (DialogResult.OK != InputBox.Show("POI", "Enter ID", ref output))
                return;

            for (int a = 0; a < MyMarkersLayer.myMarkers.Count; a++)
            {
                if (MyMarkersLayer.myMarkers[a].Point() == Point.Position)
                {
                    MyMarkersLayer.myMarkers[a].Tag = output + "\n" + Point.Position.ToString();
                    if (_POIModified != null)
                        _POIModified(null, null);
                    return;
                }
            }
        }

        public static void POIMove(GMapMarkerPOI Point)
        {
            for (int a = 0; a < MyMarkersLayer.myMarkers.Count; a++)
            {
                if (myMarkers[a].Tag == Point.ToolTipText)
                {
                    myMarkers[a].Lat = Point.Position.Lat;
                    myMarkers[a].Lng = Point.Position.Lng;
                    myMarkers[a].Tag = myMarkers[a].Tag.Substring(0, myMarkers[a].Tag.IndexOf('\n')) + "\n" + Point.Position.ToString();
                    break;
                }
            }

            if (_POIModified != null)
                _POIModified(null, null);
        }

        public static void POISave()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Poi File|*.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveFile(sfd.FileName);
                }
            }
        }

        private static void SaveFile(string fileName)
        {
            using (Stream file = File.Open(fileName, FileMode.Create))
            {
                foreach (var item in MyMarkersLayer.myMarkers)
                {
                    string line = item.Lat.ToString(CultureInfo.InvariantCulture) + "\t" +
                                  item.Lng.ToString(CultureInfo.InvariantCulture) + "\t" + item.Tag.Substring(0, item.Tag.IndexOf('\n')) + "\r\n";
                    byte[] buffer = ASCIIEncoding.ASCII.GetBytes(line);
                    file.Write(buffer, 0, buffer.Length);
                }
            }
        }


        public static void POILoad()
        {
            using (OpenFileDialog sfd = new OpenFileDialog())
            {
                sfd.Filter = "Poi File|*.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    LoadFile(sfd.FileName);
                }
            }
        }

        private static void LoadFile(string fileName)
        {
            loading = true;
            using (Stream file = File.Open(fileName, FileMode.Open))
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

        public static void UpdateOverlay(GMap.NET.WindowsForms.GMapOverlay poioverlay)
        {
            if (poioverlay == null)
                return;

            poioverlay.Clear();

            foreach (var pnt in myMarkers)
            {
                poioverlay.Markers.Add(new GMapMarkerPOI(pnt)
                {
                    ToolTipMode = MarkerTooltipMode.OnMouseOver,
                    ToolTipText = pnt.Tag
                });
            }
        }
    }
}