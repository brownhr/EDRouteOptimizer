using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EDRouteOptimizer
{
    public partial class NearestNeighbor
    {
        // TODO: Refactor to multidimensional array
        public double[][] DistanceMat { get; set; }
        public int Count;
        public int[] RouteIndex;
        public bool[] IsVisited;
        public int[] DefaultSort;


        public NearestNeighbor(EDRoute route)
        {
            Count = route.RouteWaypoints.Count;
            DistanceMat = new double[Count][];
            DefaultSort = Enumerable.Range(0, Count).ToArray();

            for (int i = 0; i < Count; i++)
            {
                DistanceMat[i] = new double[Count];
                for (int j = 0; j < Count; j++)
                {
                    DistanceMat[i][j] = GetDistance(route.RouteWaypoints[i].Coords, route.RouteWaypoints[j].Coords);
                }
            }
            IsVisited = new bool[Count];
            IsVisited[0] = true;

            RouteIndex = new int[Count];

            //RouteIndex = Enumerable.Repeat<int>(0, Count).ToArray();
            RouteIndex[0] = 0;
        }

        public NearestNeighbor(double[][] jaggedDistanceMatrix)
        {
            Count = jaggedDistanceMatrix.Length;
            DistanceMat = jaggedDistanceMatrix;

            DefaultSort = Enumerable.Range(0, Count).ToArray();

            IsVisited = new bool[Count];
            IsVisited[0] = true;

            RouteIndex = new int[Count];
            RouteIndex[0] = 0;
        }

        public void FindNearestNeighbors()
        {
            int targetIndex = 0;

            for (int i = 0; i < Count - 1; i++)
            {
                const double TEMP_MIN_VALUE = 1e5d;
                double currentRowMin = TEMP_MIN_VALUE;


                double[] currentRow = DistanceMat[targetIndex];

                for (int col = 0; col < currentRow.Length; col++)
                {
                    if (col == targetIndex) { continue; }
                    if (IsVisited[col]) { continue; }

                    if (currentRow[col] < currentRowMin)
                    {
                        currentRowMin = currentRow[col];
                        targetIndex = col;
                    }
                    RouteIndex[i + 1] = targetIndex;
                }
                IsVisited[targetIndex] = true;
            }

        }



        public double[] ReportRouteDistance(int[] sequence)
        {
            double[] dists = new double[sequence.Length];
            dists[0] = 0;
            for (int i = 0; i < Count - 1; i++)
            {
                dists[i] = DistanceMat[sequence[i]][sequence[i + 1]];
            }
            return dists;
        }

        public double GetDistance(RouteJsonCoords a, RouteJsonCoords b)
        {
            return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2) + Math.Pow(a.z - b.z, 2));
        }


    }
}
