using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDRouteOptimizer
{

    [JsonObject(MemberSerialization.OptIn)]
    public class FSSAllBodiesFoundEvent
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
}
