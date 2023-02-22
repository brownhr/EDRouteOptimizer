// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

namespace EDRouteOptimizer
{
    public class Program
    {
        public static void Main()
        {
            EDSystem homeSystem = new EDSystem(systemName: "Byeia Aerb HL-Y e0",
                                                           coords: new RouteJsonCoords(x: 813.5, y: 1547.09375, z: 13291.96875));

            string inputFilePath = @"C:\Users\brownhr\Desktop\test_json_parse.route";
            EDRoute route = EDRoute.ParseJson(inputFilePath);

            string inputFSSdata = @"C:\Users\brownhr\Documents\fss.log";
            List<FSSEvent> events = FSSEvent.ParseFSSJson(inputFSSdata);

            List<string> mappedSystems = new List<string>();
            Regex BARegex = new Regex(@"Byeia Aerb");
            foreach (FSSEvent e in events)
            {
                Match m = BARegex.Match(e.SystemName);
                if (!m.Success) continue;
                mappedSystems.Add(e.SystemName);
            }

            string[] ms_array = mappedSystems.ToArray();
            List<EDSystem> remaining = new List<EDSystem>();
            int sum = 0;

            foreach (EDSystem s in route.RouteWaypoints)
            {
                if (!Array.Exists(ms_array, e => e == s.SystemName))
                {
                    remaining.Add(s);
                    sum++;
                }
            }

            Console.WriteLine(sum + " Systems remaining");
            route.RouteWaypoints = remaining;

            route.RouteWaypoints.Insert(0, homeSystem);

            double[,] distanceMatrix = route.GenerateDistanceMatrix();

            AntColony antcolony = new AntColony(distanceMatrix);

            int[] antColonySequence = antcolony.GenerateTrailSolution();
            double finalDistance = antcolony.bestTrailLength;

            Thread.Sleep(100);


        }

        public static void RunNearestNeighbor(EDRoute route)
        {

            

            DistanceMatrix distMat = new DistanceMatrix(route);
            distMat.NearestNeighbor();

            route.SortByArray(distMat.RouteIndex);

            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string outfile = @$"C:\Users\brownhr\Desktop\test_json_parse{timeStamp}.route";

            route.WriteJson(outfile);

        }
    }
}