using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace EDRouteOptimizer
{
    public class AntColony
    {
        //double[][] distanceMatrix { get; set; }
        public double initialTrail = 1.0;

        public double pheromoneImportanceAlpha = 1.25;
        public double distancePriorityBeta = -2.5;
        public int numBestTrailsFound = 0;

        public double pheromoneEvaporationPercent = 0.75;
        public double pheromoneDepositionAmount = 1e5;


        public double maxiumumPreferredDistance = 45;

        public double antCountMultiplier = 0.6;

        // TODO: Refactor constructor to fix numberAnts
        public int? numberAnts = null;
        public double randomContribution = 0.125;
        private static int? DEBUG_SEED = null;
        private static Random random = InitRandom();

        private static Random InitRandom()
        {
            if (DEBUG_SEED != null)
            {
                return new Random(DEBUG_SEED.Value);
            }
            else
            {
                return new Random();
            }
        }
        public int maxIterations = 4096;

        public int numPoints;
        private int numAnts;

        public double[,] distanceMatrix;
        public double[,] pheromoneMatrix;

        public List<Ant> ants = new List<Ant>();
        public double[] pheromones;

        public int currentIndex;

        public int[] bestTrailOrder;
        public double bestTrailLength;
        public double[] trailDistances;

        public ProgressBarOptions options = new ProgressBarOptions { ProgressBarOnBottom = true, ProgressCharacter = '#' };

        // Initialize Ant Colony
        public AntColony(double[,] distMat)
        {
            distanceMatrix = distMat;
            numPoints = distanceMatrix.GetLength(0);
            numAnts = numberAnts == null ? (int)(numPoints * antCountMultiplier) : numberAnts.Value;

            pheromoneMatrix = new double[numPoints, numPoints];

            pheromones = new double[numPoints];

            for (int i = 0; i < numAnts; i++)
            {
                ants.Add(new Ant(numPoints));
            }
        }



        public static double[,] GenerateInitialSolution(int m, int n)
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
            ClearPheromoneMatrix();
            using (ShellProgressBar.ProgressBar pbar = new ShellProgressBar.ProgressBar(maxIterations, "Iterating ants: ", options))
            {
                for (int i = 0; i < maxIterations; i++)
                {
                    MoveAnts();
                    UpdateTrails();
                    UpdateBest();
                    pbar.Tick();
                }
            }
            Console.WriteLine("Best trail length: " + bestTrailLength.ToString("F3"));
            return (int[])bestTrailOrder.Clone();
        }

        public void InitializeAnts()
        {
            foreach (Ant ant in ants)
            {
                ant.ClearTrail();
                ant.trail[0] = 0;
                ant.isVisited[0] = true;
                //ant.isVisited[^1] = true;

                //ant.VisitPoint(0, random.Next(numPoints));
                //ant.isVisited[0] = true;
            }
            currentIndex = 0;
        }

        public void MoveAnts()
        {
            for (int i = currentIndex; i < numPoints - 1; i++)
            {
                foreach (Ant ant in ants)
                {
                    ant.VisitPoint(currentIndex, SelectNextPoint(ant));
                }
                currentIndex++;

            }
        }

        public int SelectNextPoint(Ant ant)
        {


            // Select next point randomly
            int t = random.Next(numPoints - currentIndex);
            double randomMoveProbability = random.NextDouble();
            if (randomMoveProbability < randomContribution)
            {
                int? pointIndex = Enumerable.Range(0, numPoints)
                    .Where(x => x == t && !ant.isVisited[x])
                    .Cast<int?>()
                    .FirstOrDefault();

                if (pointIndex != null)
                {
                    return pointIndex.Value;
                }
            }

            CalculateProbabilities(ant);
            int weightedIndex = Enumerable.Range(0, numPoints).Where(x => !ant.isVisited[x]).RandomElementByWeight(e => pheromones[e]);
            return weightedIndex;




            // select next point based on pheromone weight


            //for (int i = 0; i < numPoints; i++)
            //{
            //    total += pheromones[i];
            //    if (total >= r)
            //    {
            //        return i;
            //    }
            //}
            throw new IndexOutOfRangeException("No valid remaining points");
        }

        public void CalculateProbabilities(Ant ant)
        {
            int i = ant.trail[currentIndex];

            double pheromone = 0.0;

            for (int k = 0; k < numPoints; ++k)
            {
                if (ant.isVisited[k]) { continue; }
                double alpha = Math.Pow(pheromoneMatrix[i, k], pheromoneImportanceAlpha);
                double beta = Math.Pow(distanceMatrix[i, k] / maxiumumPreferredDistance, distancePriorityBeta);
                pheromone += alpha * beta;
            }



            for (int j = 0; j < numPoints; ++j)
            {
                if (ant.isVisited[j]) { pheromones[j] = 0.0; }
                else
                {
                    double alpha = Math.Pow(pheromoneMatrix[i, j], pheromoneImportanceAlpha);
                    double beta = Math.Pow(distanceMatrix[i, j] / maxiumumPreferredDistance, distancePriorityBeta);
                    double numerator = alpha * beta;
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
                    pheromoneMatrix[i, j] *= pheromoneEvaporationPercent;
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
                    pheromoneMatrix[ant.trail[i], ant.trail[i + 1]] += contribution;
                }
                pheromoneMatrix[ant.trail[numPoints - 1], ant.trail[0]] += contribution;
            }
        }

        public double[] TrailDistances(double[,] distanceMatrix)
        {
            double[] dists = new double[bestTrailOrder.Length];
            for (int i = 0; i < bestTrailOrder.Length - 1; i++)
            {
                dists[i] = distanceMatrix[i, i + 1];
            }
            return dists;
        }

        public void UpdateBest()
        {
            if (bestTrailOrder == null)
            {
                bestTrailOrder = ants[0].trail;
                bestTrailLength = ants[0].TrailLength(distanceMatrix);
            }
            foreach (Ant ant in ants)
            {
                double dist = ant.TrailLength(distanceMatrix);
                if (dist < bestTrailLength)
                {

                    numBestTrailsFound++;
                    bestTrailLength = dist;
                    trailDistances = TrailDistances(distanceMatrix);
                    ant.trail.CopyTo(bestTrailOrder, 0);
                    Console.WriteLine($"New best trail of distance {bestTrailLength:F1} found!");
                }
            }
        }

        public void ClearPheromoneMatrix()
        {
            for (int i = 0; i < numPoints; i++)
            {
                for (int j = 0; j < numPoints; j++)
                {
                    pheromoneMatrix[i, j] = initialTrail;
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
        public double TrailLength(double[,] distanceMatrix)
        {
            double total = 0;
            for (int i = 0; i < trail.Length - 1; i++)
            {
                total += distanceMatrix[trail[i], trail[i + 1]];
            }
            return total;
        }



        public void ClearTrail()
        {
            for (int i = 0; i < isVisited.Length; i++)
            {
                isVisited[i] = false;
            }
        }


    }

    public static class IEnumerableExtensions
    {

        private static Random random = new Random();
        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, double> weightSelector)
        {
            double totalWeight = sequence.Sum(weightSelector);
            double itemWeightIndex = random.NextDouble() * totalWeight;

            double currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += itemWeightIndex;

                if (currentWeightIndex > itemWeightIndex)
                {
                    return item.Value;
                }
            }
            return default(T);

        }
    }
}
