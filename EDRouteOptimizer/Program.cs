// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;


namespace EDRouteOptimizer
{
    public class Program
    {

        //public static string currentSystem = "Eaezi GW-W c1-0";
        //public static EDSystem homeSystem = new EDSystem(systemName: "Byeia Aerb HL-Y e0",
        //                                                 coords: new RouteJsonCoords(x: 813.5,
        //                                                                             y: 1547.09375,
        //                                                                             z: 13291.96875));

        public static EDSystem homeSystem;
        public static string sector;
        public static string currentSystem;

        public static EDRoute route;
        public static Dictionary<string, EDSystem> SystemDict = new Dictionary<string, EDSystem>();


        public static string inputFSSDataPath = @"C:\Users\brownhr\Documents\fss.log";
        public static string inputFilePath = @"C:\Users\brownhr\Documents\EDJP\DATA\F4352458\ScheauBlaoLX-U_f2-.route";

        public static double initialDistance;
        public static double finalDistance;

        public static int[] optimizedSequence;
        public static double[] distances;

        public static List<FSSAllBodiesFoundEvent> mappedSystems;
       

        public static void Main()
        {
            

            DateTime cutoffDate = DateTime.ParseExact(s: "2023-02-26", format: "yyyy-MM-dd", CultureInfo.InvariantCulture);

            EDEventParser.SetCutoff(cutoffDate);

            EDEventParser.ParseAllJournals();


            EDEventParser.ParseJournalEvents();
            mappedSystems = EDEventParser.FSSAllBodiesFoundEvents;


            FSDJumpEvent mostRecentJump = EDEventParser.FSDJumpEvents[^1];

            string sysName = mostRecentJump.StarSystem;

            EDSubsector subsector = EDSubsector.GetSubsector(sysName);
            sector = subsector.Sector.SectorName;

            route = EDRoute.ParseJson(inputFilePath);
            CreateSystemDict();
            homeSystem = LookupSystem(mostRecentJump.StarSystem);


            SetupRoute();

            currentSystem = homeSystem.SystemName;




            //Thread.Sleep(1000);

            //journalParser = new JournalParser(cutoff: cutoffDate);
            //journalParser.ParseJournals();

            // TODO: Read most recent FSD Jump
            Console.WriteLine(homeSystem.SystemName);

            double[,] distanceMatrix = route.GenerateDistanceMatrix();

            NearestNeighborOptimizer optimizer
                = new NearestNeighborOptimizer(distanceMatrix);


            RunOptimizer(optimizer);


            route.VerifyRoute(optimizedSequence);


            ReportDistances(distances, count: 10, reverse: true, message: "Largest distances:");
            ReportDistances(distances, count: 10, reverse: false, message: "Shortest distances:");
            route.SortByArray(optimizedSequence);


            //ReportDistances(distances, count: 10, reverse: false, sort: false, message: "Final route distances:");
            PrintRouteHead(10);

            route.CurrentDestination = 0;
            string outfile = @$"C:\Users\brownhr\Documents\EDJP\DATA\F4352458\{sector}_Optimized.route";

            route.WriteJson(outfile);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static EDSystem LookupSystem(string systemName)
        {
            if (SystemDict.TryGetValue(systemName, out EDSystem system))
            {
                return system;
            }
            else
            {
                throw new KeyNotFoundException(message: $"System {systemName} not found");
            }
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

        public static void PrintRouteHead(int count)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('[');
            var head = route.RouteWaypoints.Take(count).ToList();
            List<string> strings = Enumerable.Zip(distances, head, (a, b) => $"{b.SystemName}: {a:F2}").ToList();
            sb.AppendJoin(",\n", strings);
            sb.Append(']');
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


        public static void FilterMappedSystems(EDRoute route, List<FSSAllBodiesFoundEvent> events, string sector)
        {
            List<string> mappedSystems = new List<string>();
            Regex ByeiaAerbRegex = new Regex(sector);

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
            route.RouteWaypoints = route.RouteWaypoints.Where(e => e != null).ToList();
            Console.WriteLine(countRemainingSystems + " systems remaining.");

        }
        public static void SetupRoute()
        {

            //homeSystem = SystemDict.TryGetValue(currentSystem, out EDSystem value) == true ? value : homeSystem;



            FilterMappedSystems(route, mappedSystems, sector);
            route.RouteWaypoints = route.RouteWaypoints.DistinctBy(x => x.SystemName).ToList();

            route.RouteWaypoints.Insert(0, homeSystem);
            route.ShuffleSansFirst();
        }
    }

}