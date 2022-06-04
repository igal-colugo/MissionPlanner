using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.MyCode
{
    internal class GMapMarkerMyPointTo : GMapMarker
    {
        private readonly Bitmap icon = global::MissionPlanner.Properties.Resources.pointTo;


        public GMapMarkerMyPointTo(PointLatLng p)
            : base(p)
        {
            Size = icon.Size;            
        }

        public override void OnRender(IGraphics g)
        {
            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.DrawImage(icon, -20, -20, 40, 40);

            g.Transform = temp;
        }
    }

}
