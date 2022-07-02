using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Harmony;
using UnityEngine;

namespace TrainTimetable_MOD
{
    public class Harmony_Patch
    {
        public static ConfigData Config;

        public Harmony_Patch()
        {
            try
            {
                var harmony = HarmonyInstance.Create("Nonnonstop.TrainTimetable");
                var assembly = Assembly.GetExecutingAssembly();
                var configPath = Path.Combine(
                    Path.GetDirectoryName(assembly.Location),
                    "config.xml");
                Config = LoadConfig(configPath);
                harmony.PatchAll(assembly);
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/BaseMods/error_TrainTimetable.txt", ex.Message);
            }
        }

        [HarmonyPatch(typeof(HellTrain), "get_reduceTime")]
        private class HellTrain_GetReduceTime_Patch
        {
            public static bool Prefix(ref float __result)
            {
                __result = Config.Interval;
                return false;
            }
        }

        private ConfigData LoadConfig(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(ConfigData));
                return (ConfigData)serializer.Deserialize(fs);
            }
        }

        [XmlRoot("config")]
        public class ConfigData
        {
            [XmlElement("interval", typeof(float))]
            public float Interval;
        }
    }
}
