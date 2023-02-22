// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace EDRouteOptimizer
{
    public class Program
    {
        public static EDSystem homeSystem;
        public static EDRoute route;

        public static string inputFSSDataPath = @"C:\Users\brownhr\Documents\fss.log";
        public static string inputFilePath = @"C:\Users\brownhr\Desktop\test_json_parse.route";

        public static void Main()
        {
            SetupRoute();
           
            NearestNeighborOptimizer NNO = new NearestNeighborOptimizer()

            double[,] distanceMatrix = route.GenerateDistanceMatrix();

            AntColony antcolony = new AntColony(distanceMatrix) { numberAnts = 256 };

            int[] antColonySequence = antcolony.GenerateTrailSolution();

            foreach (int i in Enumerable.Range(0, route.RouteWaypoints.Count))
            {
                if (!Array.Exists(antColonySequence, e => e == i))
                {
                    Console.WriteLine($"Index {i} not found within {nameof(antColonySequence)}");
                }

            }

            double[] maxDists = new double[antcolony.trailDistances.Length];
            antcolony.trailDistances.CopyTo(maxDists, 0);
            Array.Sort(maxDists);
            Array.Reverse(maxDists);
            double[] topDists = maxDists.Take(20).ToArray();
            string[] tDSt = topDists.Select(x => x.ToString("F2")).ToArray();
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.AppendJoin(',', tDSt);
            sb.Append("]");

            Console.WriteLine(sb.ToString());


            double finalDistance = antcolony.bestTrailLength;

            Thread.Sleep(100);
            EDRoute routecopy = new EDRoute() { AutoSetNextDestination = true, CurrentDestination = 0, RouteWaypoints = route.RouteWaypoints };
            routecopy.SortByArray(antcolony.bestTrailOrder);
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string outfile = @$"C:\Users\brownhr\Desktop\test_json_parse{routecopy.GetHashCode()}.route";

            route.WriteJson(outfile);
        }

        public static void RunNearestNeighbor(EDRoute route)
        {



            NearestNeighbor distMat = new NearestNeighbor(route);
            distMat.FindNearestNeighbors();

            route.SortByArray(distMat.RouteIndex);

            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string outfile = @$"C:\Users\brownhr\Desktop\test_json_parse{timeStamp}.route";

            route.WriteJson(outfile);

        }

        public static void SetupRoute()
        {
            route = EDRoute.ParseJson(inputFilePath);
            homeSystem = new EDSystem(systemName: "Byeia Aerb HL-Y e0",
                                                           coords: new RouteJsonCoords(x: 813.5, y: 1547.09375, z: 13291.96875));
            List<FSSEvent> events = FSSEvent.ParseFSSJson(inputFSSDataPath);

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
            route.ShuffleSansFirst();
        }
    }
    public interface IRouteOptimizer
    {
        void SetupOptimizer();

        //void SetupDistanceMatrix();
        void RunOptimizer();
        int[] ReportResults();


    }

    public class AntColonyOptimizer : IRouteOptimizer
    {
        public AntColony antColony;
        public int[] optimizedSequence;
        private double[,] distanceMatrix;

        public AntColonyOptimizer(double[,] distanceMatrix)
        {
            this.distanceMatrix = distanceMatrix;
            SetupOptimizer();
        }

        public void SetupOptimizer()
        {
            antColony = new AntColony(distanceMatrix);
        }
        public void RunOptimizer()
        {
            antColony.GenerateTrailSolution();
            optimizedSequence = antColony.bestTrailOrder;
        }

        public int[] ReportResults()
        {
            return optimizedSequence;
        }
    }

    public class NearestNeighborOptimizer : IRouteOptimizer
    {
        public NearestNeighbor nearestNeighbor;
        public int[] optimizedSequence;
        public double[][] jaggedDistanceMatrix;

        public NearestNeighborOptimizer(double[][] distanceMatrix)
        {
            this.jaggedDistanceMatrix = distanceMatrix;
            SetupOptimizer();

        }
        public NearestNeighborOptimizer(EDRoute route)
        {
            
        }

        public void SetupOptimizer()
        {
            nearestNeighbor = new NearestNeighbor(jaggedDistanceMatrix);
        }

        public void RunOptimizer()
        {
            nearestNeighbor.FindNearestNeighbors();
            optimizedSequence = nearestNeighbor.RouteIndex;

        }

        public int[] ReportResults()
        {
            return optimizedSequence;
        }

    }
}