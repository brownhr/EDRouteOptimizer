// See https://aka.ms/new-console-template for more information
using System.Globalization;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace EDRouteOptimizer
{
    public class Program
    {

        public static string currentSystem = "Byeia Aerb HL-Y e0";
        public static EDSystem homeSystem = new EDSystem(systemName: "Byeia Aerb HL-Y e0",
                                                           coords: new RouteJsonCoords(x: 813.5, y: 1547.09375, z: 13291.96875));

        public static EDRoute route;
        public static Dictionary<string, EDSystem> SystemDict = new Dictionary<string, EDSystem>();


        public static string inputFSSDataPath = @"C:\Users\brownhr\Documents\fss.log";
        public static string inputFilePath = @"C:\Users\brownhr\Desktop\routes\test_json_parse.route";

        public static double initialDistance;
        public static double finalDistance;

        public static int[] optimizedSequence;
        public static double[] distances;

        public static JournalParser journalParser;
        public static List<FSSAllBodiesFoundEvent> mappedSystems;

        public static void Main()
        {
            DateTime cutoffDate = DateTime.ParseExact(s:"2023-01-01", format: "yyyy-MM-dd", CultureInfo.InvariantCulture);
            journalParser = new JournalParser(cutoff: cutoffDate);
            journalParser.ParseJournals();

            mappedSystems = journalParser.FSSMappedSystems;

            SetupRoute();
            double[,] distanceMatrix = route.GenerateDistanceMatrix();

            NearestNeighborOptimizer optimizer
                = new NearestNeighborOptimizer(distanceMatrix);


            RunOptimizer(optimizer);


            route.VerifyRoute(optimizedSequence);


            ReportDistances(distances, count: 10, reverse: true, message: "Largest distances:");
            ReportDistances(distances, count: 10, reverse: false, message: "Shortest distances:");

            ReportDistances(distances, count: 10, reverse: false, sort: false, message: "Final route distances:");


            route.SortByArray(optimizedSequence);
            route.CurrentDestination = 0;
            string outfile = @$"C:\Users\brownhr\Desktop\routes\test_json_parse{route.GetHashCode()}.route";

            route.WriteJson(outfile);
        }

        public static void RunOptimizer(IRouteOptimizer routeOptimizer)
        {
            routeOptimizer.RunOptimizer();
            optimizedSequence = routeOptimizer.GetOptimizedRoute();
            distances = routeOptimizer.ReturnDistances();
            finalDistance = distances.Sum();
        }

        public static void ReportDistances(double[] distances, int count, bool reverse = true, bool sort = true, string? message = null)
        {
            double[] array = (double[])distances.Clone();

            if (sort) { Array.Sort(array); }
            if (reverse) { Array.Reverse(array); }

            double[] topDists = array.Take(count).ToArray();
            StringBuilder sb = new StringBuilder();
            if (message != null)
            {
                Console.WriteLine(message);
            }

            sb.Append("[");
            sb.AppendJoin(',', topDists.Select(x => x.ToString("F2")).ToArray());
            sb.Append("]");
            Console.WriteLine(sb.ToString());
        }


        public static void CreateSystemDict()
        {
            foreach (EDSystem system in route.RouteWaypoints)
            {

                string name = system.SystemName;

                if (!SystemDict.ContainsKey(name))
                {
                    SystemDict.Add(name, system);
                }
            }
        }


        public static void FilterMappedSystems(EDRoute route, List<FSSAllBodiesFoundEvent> events)
        {
            List<string> mappedSystems = new List<string>();
            Regex ByeiaAerbRegex = new Regex("Byeia Aerb");

            foreach (FSSAllBodiesFoundEvent _event in events)
            {
                Match m = ByeiaAerbRegex.Match(_event.SystemName);
                if (!m.Success) continue;
                mappedSystems.Add(_event.SystemName);
            }
            string[] mappedSystemsArray = mappedSystems.ToArray();

            List<EDSystem> remainingSystems = new List<EDSystem>();

            int countRemainingSystems = 0;

            foreach (EDSystem system in route.RouteWaypoints)
            {
                if (!Array.Exists(mappedSystemsArray, x => x == system.SystemName))
                {
                    remainingSystems.Add(system);
                    countRemainingSystems++;
                }
            }

            route.RouteWaypoints = remainingSystems;
            Console.WriteLine(countRemainingSystems + " systems remaining.");

        }
        public static void SetupRoute()
        {
            route = EDRoute.ParseJson(inputFilePath);

            CreateSystemDict();
            homeSystem = SystemDict.TryGetValue(currentSystem, out EDSystem value) == true ? value : homeSystem;

            

            FilterMappedSystems(route, mappedSystems);


            route.RouteWaypoints.Insert(0, homeSystem);
            route.ShuffleSansFirst();
        }
    }

}