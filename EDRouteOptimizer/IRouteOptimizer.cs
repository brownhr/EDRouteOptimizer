using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EDRouteOptimizer
{
    public interface IRouteOptimizer
    {
        void SetupOptimizer();

        //void SetupDistanceMatrix();
        void RunOptimizer();
        int[] GetOptimizedRoute();
        double[] ReturnDistances();
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

        public double[] ReturnDistances()
        {
            return antColony.trailDistances;

        }
        public int[] GetOptimizedRoute()
        {
            return optimizedSequence;
        }
    }

    public class NearestNeighborOptimizer : IRouteOptimizer
    {
        public NearestNeighbor nearestNeighbor;
        public int[] optimizedSequence;
        public double[,] distanceMatrix;

        public NearestNeighborOptimizer(double[,] distanceMatrix)
        {
            this.distanceMatrix = distanceMatrix;
            SetupOptimizer();

        }

        public void SetupOptimizer()
        {
            nearestNeighbor = new NearestNeighbor(distanceMatrix);
        }

        public void RunOptimizer()
        {
            nearestNeighbor.FindNearestNeighbors();
            optimizedSequence = nearestNeighbor.RouteIndex;

        }


        public double[] ReturnDistances()
        {
            return nearestNeighbor.CalculateTrailDistance(optimizedSequence);
        }


        public int[] GetOptimizedRoute()
        {
            return optimizedSequence;
        }

    }
}
