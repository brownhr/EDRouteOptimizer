using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public double[, ] GenerateDistanceMatrix()
        {
            int length = RouteWaypoints.Count;
            double[, ] distanceMatrix = new double[length, length];

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    distanceMatrix[i, j] = CalculateEuclideanDistance(RouteWaypoints[i].Coords, RouteWaypoints[j].Coords);
                }
            }
            return distanceMatrix;
        }

        public static double CalculateEuclideanDistance(RouteJsonCoords a, RouteJsonCoords b)
        {
            return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2) + Math.Pow(a.z - b.z, 2));
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

        private static Random rng = new Random();
        public void ShuffleSansFirst()
        {
            var shuffledWaypoints = RouteWaypoints.Skip(1).OrderBy(a => rng.Next()).ToList();
            shuffledWaypoints.Insert(0, RouteWaypoints[0]);
            RouteWaypoints = shuffledWaypoints;
        }


        public void VerifyRoute(int[] sequence)
        {
            foreach (int i in Enumerable.Range(0, RouteWaypoints.Count))
            {
                if (!Array.Exists(sequence, e => e == i))
                {
                    Console.WriteLine($"Index {i} not found within {nameof(sequence)}");
                }
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RouteWaypoints);
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

        [Newtonsoft.Json.JsonConstructor]
        public RouteJsonCoords(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public RouteJsonCoords(double[] coordArray)
        {
            if (coordArray.Length != 3) { throw new ArgumentException("Coordinate array must be of length 3"); }
            this.x = coordArray[0];
            this.y = coordArray[1];
            this.z = coordArray[2];
        }

        public override string ToString()
        {
            return $"({x:F2}, {y:F2}, {z:F2})";
        }
    }


}
