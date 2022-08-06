using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

    /*
      class Program
      {

          static void Main(string[] args)
          {
          ConfigFileHelper settings = ConfigFileHelper.Load();
              Console.WriteLine("Current value of 'myInteger': " + settings.myInteger);
              Console.WriteLine("Incrementing 'myInteger'...");
              settings.myInteger++;
              Console.WriteLine("Saving settings...");
              settings.Save();
              Console.WriteLine("Done.");
              Console.ReadKey();
          }
          */

   

    class MyGeneralConfigFileHelper : MyConfigFileHelper<MyGeneralConfigFileHelper>
          {
        public float LowBattVolt { get; set; } = 20.0f;
        public float CritBattVolt { get; set; } = 18.5f;
        public bool BalloonMode { get; set; } = false;
        public bool ShortTO { get; set; } = false;
        public bool EnableWarns { get; set; } = false;    
        public int MinSpd { get; set; } = 10;
        public int CruiseSpd { get; set; } = 20;
        public int MaxSpd { get; set; } = 30;
        public int AltIncrement { get; set; } = 10;
        public int RElTOAlt { get; set; } = 25;
        public int AfterToWpAlt{ get; set; } = 47;
        public int DistToAfterToWp { get;  set; } = 455;
        public int ScndWpAlt { get; set; } = 88;
        public int DistToScndWp { get; set; } = 145;
        public bool DisableVideo { get; set; } = false;
}
   //   }
      
    public class MyConfigFileHelper<T> where T : new()
    {
        public const string DEFAULT_FILENAME = "my.settings.json";

        public void Save(string fileName = DEFAULT_FILENAME)
        {
            string userDocsFile;
            if (foundFile(out userDocsFile, fileName))
                File.WriteAllText(userDocsFile, (new JavaScriptSerializer()).Serialize(this));
        }

        public static void Save(T pSettings, string fileName = DEFAULT_FILENAME)
        {
            File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(pSettings));
        }

        public static T Load(string fileName = DEFAULT_FILENAME)
        {
            T t = new T();

            string userDocsFile;// = Settings.GetUserDataDirectory() + fileName;
            if (foundFile(out userDocsFile, fileName))
                t = (new JavaScriptSerializer()).Deserialize<T>(File.ReadAllText(userDocsFile));
           
            return t;
        }

        private static bool foundFile(out string finalName, string initialFileName)
        {
            finalName = initialFileName;
            if (File.Exists(finalName))
                return true;

            finalName = initialFileName;
            if (File.Exists(initialFileName))
                return true;
            //we couldnt find the file...
            return false;
        }
    }

    /////////////////////////////////////////
    ///
    
