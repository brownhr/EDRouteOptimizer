using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace EDRouteOptimizer
{

    public static class EDEventParser
    {
        public static List<JObject> JournalEvents = new List<JObject>();
        public static List<FSSAllBodiesFoundEvent> FSSAllBodiesFoundEvents = new List<FSSAllBodiesFoundEvent>();
        public static List<FSDJumpEvent> FSDJumpEvents = new List<FSDJumpEvent>();

        private static string JournalDirectory =
            Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous");

        private static List<string> JournalFiles =
            Directory.GetFiles(JournalDirectory, searchPattern: "Journal*.log").ToList();

        public static DateTime _dateCutoff = new DateTime();


        public static void SetCutoff(DateTime cutoff)
        {
            _dateCutoff = cutoff;
        }


        public static void ParseAllJournals()
        {

            foreach (string file in JournalFiles)
            {
                DateTime lastModifiedTime = File.GetLastWriteTime(file);

                if (lastModifiedTime < _dateCutoff) { continue; }

                try
                {
                    ReadJournal(file);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                }


            }

        }




        public static void ReadJournal(string filename)
        {
           
            FileStream fileStream = new FileStream(filename,
                                                   FileMode.Open,
                                                   FileAccess.Read,
                                                   FileShare.ReadWrite | FileShare.Delete);

            JsonTextReader jsonReader = new JsonTextReader(new StreamReader(fileStream, Encoding.Default))
            {
                SupportMultipleContent = true
            };
            JsonSerializer jsonSerializer = new JsonSerializer();
            while (jsonReader.Read())
            {
                JObject obj = jsonSerializer.Deserialize<JObject>(jsonReader);
                JournalEvents.Add(obj);
            }
        }

        public static void ParseJournalEvents()
        {
            foreach (JObject obj in JournalEvents)
            {
                string eventType = obj.TryGetValue("event", out JToken? value) ? value.ToString() : string.Empty;

                switch (eventType)
                {
                    case "FSSAllBodiesFound":
                        FSSAllBodiesFoundEvents.Add(obj.ToObject<FSSAllBodiesFoundEvent>());
                        break;

                    case "FSDJump":
                        FSDJumpEvents.Add(obj.ToObject<FSDJumpEvent>());
                        break;

                    default:
                        continue;

                }

            }


        }



    }


    [JsonObject(MemberSerialization.OptIn)]
    public class JournalEvent
    {
        [JsonProperty]
        public string timestamp { get; set; }

        [JsonProperty]
        public string @event { get; set; }


    }



    // TODO: Refactor to use abstract class for different log events
    [JsonObject(MemberSerialization.OptIn)]
    public class FSSAllBodiesFoundEvent : JournalEvent
    {

        [JsonProperty]
        public string SystemName { get; set; }

        [JsonProperty]
        public ulong SystemAddress { get; set; }

        [JsonProperty]
        public int Count { get; set; }

        public override string ToString()
        {
            return $"{@event}: {SystemName}";
        }


        //public FSSAllBodiesFoundEvent(string systemName, ulong systemAddress, int count)
        //{
        //    this.SystemName = systemName ;
        //    this.SystemAddress = systemAddress ;
        //    this.Count = count ;
        //}

        public static List<FSSAllBodiesFoundEvent> ParseFSSJson(string filepath)
        {

            List<FSSAllBodiesFoundEvent> eventList = new List<FSSAllBodiesFoundEvent>();

            var jsonReader = new JsonTextReader(new StreamReader(filepath))

            {
                SupportMultipleContent = true
            };
            var jsonSerializer = new JsonSerializer();
            while (jsonReader.Read())
            {
                FSSAllBodiesFoundEvent e = jsonSerializer.Deserialize<FSSAllBodiesFoundEvent>(jsonReader);

                eventList.Add(e);
            }

            return eventList;
        }

    }

    [JsonObject(MemberSerialization.OptIn)]
    public class FSDJumpEvent : JournalEvent
    {
        [JsonProperty]
        public bool Taxi { get; set; }
        [JsonProperty]
        public bool Multicrew { get; set; }
        [JsonProperty]
        public string StarSystem { get; set; }
        [JsonProperty]
        public ulong SystemAddress { get; set; }
        [JsonProperty]
        public double[] StarPos { get; set; }
        [JsonProperty]
        public double JumpDist { get; set; }
        [JsonProperty]
        public double FuelUsed { get; set; }
        [JsonProperty]
        public double FuelLevel { get; set; }
    }
}
