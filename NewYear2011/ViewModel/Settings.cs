using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using NewYear2011.Helper;

namespace NewYear2011.ViewModel
{
    public class Settings
    {
        public double TreeScale { get; set; }
        public double TreeX { get; set; }
        public double TreeY { get; set; }

        public Boolean RunOnStartup { get; set; }
        public Boolean PlaySound { get; set; }

        public List<TreeGadgetSettings> gadgetInstances { get; set; }

        public Settings()
        {
            gadgetInstances = new List<TreeGadgetSettings>();
            TreeScale = 1;
            PlaySound = true;
        }
        public static Settings curSettings;
        public static void Save()
        {
            string dataPath = MainHelper.GetApplicationDir() + "\\settings.dat";
            if (curSettings != null)
            {

                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                using (FileStream fs = new FileStream(dataPath, FileMode.Create))
                {
                    ser.Serialize(fs, curSettings);
                    fs.Close();
                }

            }
        }
        public static void Load(Boolean LoadDefaultData = false)
        {
            string datafname = "\\settings.dat";
            if (LoadDefaultData)
                datafname = "\\default.dat";
            string dataPath = MainHelper.GetApplicationDir() + datafname;
            if (File.Exists(dataPath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                using (FileStream fs = new FileStream(dataPath, FileMode.Open))
                {
                    curSettings = ser.Deserialize(fs) as Settings;
                    fs.Close();
                }
            }
        }
    }

    [Serializable]
    public class TreeGadgetSettings
    {

        public TreeGadgetSettings() { }

        public string imgPath { get; set; }
        public double Scale { get; set; }

        public double TranslateX { get; set; }
        public double TranslateY { get; set; }
    }
}
