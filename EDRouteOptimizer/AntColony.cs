using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDRouteOptimizer
{
    internal class AntColony
    {
        //double[][] distanceMatrix { get; set; }
        private double initialTrail = 1.0;

        private double pheromoneImportanceAlpha = 1.25;
        private double distancePriorityBeta = 5.0;

        private double pheromoneEvaporationPercent = 0.75;
        private double pheromoneDepositionAmount = 100;

        private double antContriubution = 0.8;
        private double randomContribution = 0.01;

        private static Random random = new Random();

        private int maxIterations = 1000;

        public int numPoints;
        public int numAnts;

        public double[,] distanceMatrix;
        public double[,] trails;

        private List<Ant> ants = new List<Ant>();
        private double[] pheromones;

        private int currentIndex;

        public int[] bestTrailOrder;
        public double bestTrailLength;


        // Initialize Ant Colony
        public AntColony(double[,] distMat)
        {
            distanceMatrix = distMat;
            numPoints = distanceMatrix.GetLength(0);
            numAnts = (int)(numPoints * antContriubution);

            trails = new double[numPoints, numPoints];

            pheromones = new double[numPoints];

            for (int i = 0; i < numAnts; i++)
            {
                ants.Add(new Ant(numPoints));
            }
        }


        private static double[,] GenerateInitialSolution(int m, int n)
        {
            double[,] array = new double[m, n];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    array[i, j] = (double)random.Next(100) + 1;
                }
            }
            return array;
        }

        public int[] GenerateTrailSolution()
        {
            InitializeAnts();
            ClearTrails();
            for (int i = 0; i < maxIterations; i++)
            {
                MoveAnts();
                UpdateTrails();
                UpdateBest();
            }
            Console.WriteLine("Best trail length: " + bestTrailLength.ToString("F3"));
            return (int[])bestTrailOrder.Clone();
        }

        private void InitializeAnts()
        {
            foreach (Ant ant in ants)
            {
                ant.ClearTrail();
                ant.VisitPoint(-1, random.Next(numPoints));
            }
            currentIndex = 0;
        }

        private void MoveAnts()
        {
            for (int i = currentIndex; i < numPoints - 1; i++)
            {
                foreach (Ant ant in ants)
                {
                    ant.VisitPoint(currentIndex, SelectNextPoint(ant));
                    currentIndex++;
                }
            }
        }

        private int SelectNextPoint(Ant ant)
        {
            int t = random.Next(numPoints - currentIndex);
            if (random.NextDouble() < randomContribution)
            {
                int? pointIndex = Enumerable.Range(0, numPoints)
                    .Where(x => x == t && !ant.IsVisited(x))
                    .First();

                if (pointIndex != null)
                {
                    return pointIndex.Value;
                }
            }

            CalculateProbabilities(ant);
            double r = random.NextDouble();
            double total = 0;
            for (int i = 0; i < numPoints; i++)
            {
                total += pheromones[i];
                if (total >= r)
                {
                    return i;
                }
            }
            throw new IndexOutOfRangeException("No valid remaining points");
        }

        public void CalculateProbabilities(Ant ant)
        {
            int i = ant.trail[currentIndex];

            double pheromone = 0.0;

            for (int k = 0; k < numPoints; ++k)
            {
                if (ant.IsVisited(k)) { continue; }
                pheromone +=
                    Math.Pow(trails[i, k], pheromoneImportanceAlpha) *
                    Math.Pow(1.0 / distanceMatrix[i, k], distancePriorityBeta);
            }

            for (int j = 0; j < numPoints; ++j)
            {
                if (ant.isVisited[j]) { pheromones[j] = 0.0; }
                else
                {
                    double numerator =
                        Math.Pow(trails[i, j], pheromoneImportanceAlpha) *
                        Math.Pow(1.0 / distanceMatrix[i, j], distancePriorityBeta);
                    pheromones[j] = numerator / pheromone;
                }
            }
        }

        public void UpdateTrails()
        {
            EvaporatePheromones();
            DepositPheromones();
        }

        public void EvaporatePheromones()
        {
            for (int i = 0; i < numPoints; ++i)
            {
                for (int j = 0; j < numPoints; j++)
                {
                    trails[i, j] *= pheromoneEvaporationPercent;
                }
            }
        }

        public void DepositPheromones()
        {
            foreach (Ant ant in ants)
            {
                double contribution = pheromoneDepositionAmount / ant.TrailLength(distanceMatrix);
                for (int i = 0; i < numPoints - 1; i++)
                {
                    trails[ant.trail[i], ant.trail[i + 1]] += contribution;
                }
                trails[ant.trail[numPoints - 1], ant.trail[0]] += contribution;
            }
        }

        private void UpdateBest()
        {
            if (bestTrailOrder == null)
            {
                bestTrailOrder = ants[0].trail;
                bestTrailLength = ants[0].TrailLength(distanceMatrix);
            }
            foreach (Ant ant in ants)
            {
                if (ant.TrailLength(distanceMatrix) > bestTrailLength) { continue; }
                bestTrailLength = ant.TrailLength(distanceMatrix);
                bestTrailOrder = (int[])ant.trail.Clone();
            }
        }

        private void ClearTrails()
        {
            for (int i = 0; i < numPoints; i++)
            {
                for (int j = 0; j < numPoints; j++)
                {
                    trails[i, j] = initialTrail;
                }
            }
        }


    }


    public class Ant
    {
        internal int trailSize;

        internal int[] trail;

        internal bool[] isVisited;

        public Ant(int numPoints)
        {
            trailSize = numPoints;
            trail = new int[numPoints];
            isVisited = new bool[numPoints];
        }


        public void VisitPoint(int currentIndex, int point)
        {
            trail[currentIndex + 1] = point;
            isVisited[point] = true;
        }

        public bool IsVisited(int point)
        {
            return isVisited[point];
        }

        public double TrailLength(double[,] distanceMatrix)
        {
            double length = 0;
            for (int i = 0; i < trailSize - 1; i++)
            {
                length += distanceMatrix[i, i + 1];
            }
            return length;
        }

        public void ClearTrail()
        {
            for (int i = 0; i < isVisited.Length; i++)
            {
                isVisited[i] = false;
            }
        }


    }
}
