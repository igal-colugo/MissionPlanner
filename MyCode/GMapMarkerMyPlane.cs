
using System;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;

namespace MissionPlanner.MyCode
{
    [Serializable]
    public class GMapMarkerMyPlane : GMapMarkerBase
    {
        private PointLatLng hompPos;
        private Bitmap icon;// = global::MissionPlanner.Maps.Resources.boat;//global::MissionPlanner.Maps.Resources.planeicon;
        private System.Drawing.Size SizeSt = //need to get icon size
            new System.Drawing.Size(global::MissionPlanner.Maps.Resources.boat.Width,
                global::MissionPlanner.Maps.Resources.boat.Height);
        static SolidBrush shadow = new SolidBrush(Color.FromArgb(50, Color.Black));
       
        float cog = -1;
        float heading = 0;
        float nav_bearing = -1;
        float radius = -1;
        float target = -1;
        int which = 0;

        public GMapMarkerMyPlane(Bitmap myIcon, int which, PointLatLng p, PointLatLng h, float heading, float cog, float nav_bearing, float target,
            float radius)
            : base(p)
        {
            this.hompPos = h;
            this.icon = myIcon;
            Initilaize(which, heading, cog, nav_bearing, target, radius);
        }

        private void Initilaize(int which, float heading, float cog, float nav_bearing, float target,
            float radius)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            this.nav_bearing = nav_bearing;
            this.radius = radius;
            this.which = which;
            Size = icon.Size;
        }





        public float Cog { get => cog; set => cog = value; }
        public float Heading { get => heading; set => heading = value; }
        public float Nav_bearing { get => nav_bearing; set => nav_bearing = value; }
        public float Radius { get => radius; set => radius = value; }
        public float Target { get => target; set => target = value; }

        public override void OnRender(IGraphics g)
        {
            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.RotateTransform(-Overlay.Control.Bearing);

            // anti NaN
            try
            {
                if (DisplayHeading)
                    g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f,
                        (float)Math.Cos((heading - 90) * MathHelper.deg2rad) * length,
                        (float)Math.Sin((heading - 90) * MathHelper.deg2rad) * length);
            }
            catch
            {
            }

            // line to home...
            try
            {
                GPoint p1 = Overlay.Control.FromLatLngToLocal(Position);
                GPoint p2 = Overlay.Control.FromLatLngToLocal(hompPos);
                int x1, y1, x2, y2;
                x1 = (int)p1.X;
                y1 = (int)p1.Y;
                x2 = (int)p2.X;
                y2 = (int)p2.Y;
                // g.DrawLine(new Pen(Color.Aqua, 2), x1, y1, x2, y2);
                g.DrawLine(new Pen(Color.Aqua, 2), 0, 0, x2-x1, y2-y1);
            
            
        }
            catch
            {
            }



            if (DisplayNavBearing)
                g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f,
                    (float)Math.Cos((nav_bearing - 90) * MathHelper.deg2rad) * length,
                    (float)Math.Sin((nav_bearing - 90) * MathHelper.deg2rad) * length);
            if (DisplayCOG)
                g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f,
                    (float)Math.Cos((cog - 90) * MathHelper.deg2rad) * length,
                    (float)Math.Sin((cog - 90) * MathHelper.deg2rad) * length);
            if (DisplayTarget)
                g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f,
                    (float)Math.Cos((target - 90) * MathHelper.deg2rad) * length,
                    (float)Math.Sin((target - 90) * MathHelper.deg2rad) * length);
            // anti NaN
            try
            {
                if (DisplayRadius)
                {
                    float desired_lead_dist = 100;

                    double width =
                        (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                             Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0)) * 1000.0);
                    double m2pixelwidth = Overlay.Control.Width / width;

                    float alpha = (float)(((desired_lead_dist * (float)m2pixelwidth) / radius) * MathHelper.rad2deg);

                    var scaledradius = radius * (float)m2pixelwidth;

                    if (radius < -1 && alpha < -1)
                    {
                        // fixme 

                        float p1 = (float)Math.Cos((cog) * MathHelper.deg2rad) * scaledradius + scaledradius;

                        float p2 = (float)Math.Sin((cog) * MathHelper.deg2rad) * scaledradius + scaledradius;

                        g.DrawArc(new Pen(Color.HotPink, 2), p1, p2, Math.Abs(scaledradius) * 2,
                            Math.Abs(scaledradius) * 2, cog, alpha);
                    }

                    else if (radius > 1 && alpha > 1)
                    {
                        // correct

                        float p1 = (float)Math.Cos((cog - 180) * MathHelper.deg2rad) * scaledradius + scaledradius;

                        float p2 = (float)Math.Sin((cog - 180) * MathHelper.deg2rad) * scaledradius + scaledradius;

                        g.DrawArc(new Pen(Color.HotPink, 2), -p1, -p2, scaledradius * 2, scaledradius * 2, cog - 180,
                            alpha);
                    }
                }
            }
            catch
            {
            }

            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            g.DrawImageUnscaled(icon, Size.Width / -2,
                Size.Height / -2);

            
            g.Transform = temp;
        }
    }
}