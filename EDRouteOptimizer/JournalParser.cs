﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Helpers;
using System.Threading.Tasks;
using System.Dynamic;

namespace EDRouteOptimizer
{
    public class JournalParser
    {
        private static string journal = @"%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous";
        private static string JournalDirectory = Environment.ExpandEnvironmentVariables(journal);

        private static string filter = "Journal*.log";

        private static List<string> journalFiles = System.IO.Directory.GetFiles(JournalDirectory, filter).ToList();

        public List<dynamic> journalEntries = new List<dynamic>();

        private DateTime dateTimeCutoff = new DateTime();
        public List<FSSAllBodiesFoundEvent> FSSMappedSystems = new List<FSSAllBodiesFoundEvent>();


        public JournalParser(DateTime cutoff)
        {
            dateTimeCutoff = cutoff;
        }
        public void ParseJournals()
        {





            foreach (string file in journalFiles)
            {
                DateTime modifiedTime = File.GetLastWriteTime(file);
                if (modifiedTime < dateTimeCutoff) { continue; }
                JsonTextReader jsonTextReader = new JsonTextReader(new StreamReader(file)) { SupportMultipleContent = true };

                JsonSerializer jsonSerializer = new JsonSerializer();

                while (jsonTextReader.Read())
                {
                    var dynamicObj = jsonSerializer.Deserialize<dynamic>(jsonTextReader);
                    string eventType = dynamicObj["event"];
                    if (eventType != "FSSAllBodiesFound") { continue; }
                    string systemName = dynamicObj["SystemName"];
                    ulong systemAddress = dynamicObj["SystemAddress"];
                    int count = dynamicObj["Count"];
                    string timestamp = dynamicObj["timestamp"];

                    FSSMappedSystems.Add(new FSSAllBodiesFoundEvent() { @event = eventType, SystemName = systemName, SystemAddress = systemAddress, Count = count , timestamp = timestamp});
                }
            }




        }

        public static FSSAllBodiesFoundEvent ParseDynamicFSSEvent(string jsonString)
        {
            var dynamicObj = JsonConvert.DeserializeObject<dynamic>(jsonString)!;

            string eventType = dynamicObj["event"];
            string systemName = dynamicObj["SystemName"];
            ulong systemAddress = dynamicObj["SystemAddress"];
            int count = dynamicObj["Count"];

            return new FSSAllBodiesFoundEvent() { @event = eventType, SystemName = systemName, SystemAddress = systemAddress, Count = count };

        }

    }
}
