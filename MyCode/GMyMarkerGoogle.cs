
using System.ComponentModel;
using System.IO;
using GMap.NET.Drawing;

namespace MissionPlanner.MyCode
{
   using System.Drawing;
   using System.Collections.Generic;

#if !PocketPC
    using GMap.NET.Drawing.Properties;
   using System;
   using System.Runtime.Serialization;
    using GMap.NET.WindowsForms;
    using GMap.NET;
    using System.Windows.Forms;
    using MissionPlanner.Utilities;
#else
   using GMap.NET.WindowsMobile.Properties;
#endif



#if !PocketPC
    [Serializable]
   public class GMyMarkerGoogle : GMapMarker, ISerializable
#else
   public class GMarkerGoogle : GMapMarker
#endif
   {
      Image Bitmap;
        private ImageList _im;

        public GMyMarkerGoogle(PointLatLng p, ImageList im, int id)
         : base(p)
      {
            _im = im;
            LoadBitmap(id);
      }

      void LoadBitmap(int id)
      {
            Bitmap = _im.Images[id]; //LoadBitmap from file here//GetIcon(Type.ToString());
            lock (Bitmap)
            {
                Size = new System.Drawing.Size(Bitmap.Width, Bitmap.Height);
                Offset = new Point(-Size.Width / 2, -Size.Height / 2);
            }
            

      }

     

      public override void OnRender(IGraphics g)
      {
          if (Math.Abs(LocalPosition.X) > 100000 || Math.Abs(LocalPosition.Y) > 100000)
                return;

#if !PocketPC
            g.DrawImage(Bitmap, LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
         
#else
         if(BitmapShadow != null)
         {
            DrawImageUnscaled(g, BitmapShadow, LocalPosition.X, LocalPosition.Y);
         }
         DrawImageUnscaled(g, Bitmap, LocalPosition.X, LocalPosition.Y);
#endif
      }

      public override void Dispose()
      {        
         base.Dispose();
      }


   }
}
