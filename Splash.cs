using MissionPlanner.MyCode;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MissionPlanner
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();

            string strVersion = typeof(Splash).GetType().Assembly.GetName().Version.ToString();

      //      TXT_version.Text = "Version: " + Application.ProductVersion; // +" Build " + strVersion;

            Console.WriteLine(strVersion);
            this.BackgroundImage = Image.FromFile(Path.Combine(MySettings.myBasePath, "general\\splashscreen.png"));
            
           

            Console.WriteLine("Splash .ctor");
        }
    }
}