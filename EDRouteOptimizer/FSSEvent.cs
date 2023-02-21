using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDRouteOptimizer
{

    [JsonObject(MemberSerialization.OptIn)]
    internal class FSSEvent
    {
        [JsonProperty]
        public string? timestamp { get; set; }
        [JsonProperty]

        public string? @event { get; set; }

        [JsonProperty]
        public string? SystemName { get; set; }

        [JsonProperty]
        public ulong? SystemAddress { get; set; }

        [JsonProperty]
        public int? Count { get; set; }


        public static List<FSSEvent> ParseFSSJson(string filepath)
        {

            List<FSSEvent> eventList = new List<FSSEvent>();

            var jsonReader = new JsonTextReader(new StreamReader(filepath))

            {
                SupportMultipleContent = true
            };
            var jsonSerializer = new JsonSerializer();
            while (jsonReader.Read())
            {
                FSSEvent e = jsonSerializer.Deserialize<FSSEvent>(jsonReader);

                eventList.Add(e);
            }

            return eventList;
        }

    }
}
