﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EDRouteOptimizer
{

    [JsonObject(MemberSerialization.OptIn)]
    public class EDRoute
    {
        [JsonProperty]
        public List<EDSystem> RouteWaypoints { get; set; }
        [JsonProperty]
        public int CurrentDestination { get; set; }
        [JsonProperty]
        public bool AutoSetNextDestination { get; set; }

        public static EDRoute ParseJson(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string json = reader.ReadToEnd();
                EDRoute route = JsonConvert.DeserializeObject<EDRoute>(json);
                return route;
            }
        }


        public void WriteJson(string outFilePath)
        {

            using (StreamWriter writer = File.CreateText(outFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();

                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, this);

            }

        }


        public void SortByArray(int[] sequence)
        {
            List<EDSystem> newOrder = new List<EDSystem>();

            foreach (int item in sequence)
            {
                newOrder.Add(RouteWaypoints[item]);

            }


            RouteWaypoints = newOrder;


        }




    }



    [Serializable]
    public class RouteJsonCoords
    {
        [JsonProperty]
        public double x { get; set; }
        [JsonProperty]
        public double y { get; set; }
        [JsonProperty]
        public double z { get; set; }

        public RouteJsonCoords(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return $"({x:F2}, {y:F2}, {z:F2})";
        }
    }


}
