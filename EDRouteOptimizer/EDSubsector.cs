using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDRouteOptimizer
{
    public class EDSubsector
    {
        public readonly EDSector Sector;
        public readonly EDBoxel Boxel;


        public EDSubsector(EDSector sector, EDBoxel boxel)
        {
            Sector = sector;
            Boxel = boxel;
        }



        // TODO: Calculate Manhattan distance between sectors

        public static int ManhattanDistanceBetweenSectors(EDSector sectorA, EDSector sectorB)
        {

            int[] sectorAIDs = new int[3] { sectorA.ID64X, sectorA.ID64Y, sectorA.ID64Z };
            int[] sectorBIDs = new int[3] { sectorB.ID64X, sectorB.ID64Y, sectorB.ID64Z };

            int[] axisDistances = Enumerable.Zip(sectorAIDs, sectorBIDs, (a, b) => (int)Math.Abs(a - b)).ToArray();

            return axisDistances.Sum();
        }

        
        


        public EDSector? Shift(int[] offsets)
        public static EDSubsector GetSubsector(string input)
        {
            Regex sectorRegex = new Regex(@"(?<sector>[\w ]+(?= [A-Z]{2}))");
            Regex boxelRegex = new Regex(@"(?<boxel>[A-Z]{2}-[A-Z] [a-h]\d+)");

            Match sectorMatch = sectorRegex.Match(input);
            Match boxelMatch = boxelRegex.Match(input);

            if (sectorMatch.Success && boxelMatch.Success)
            {
                EDSector sector = EDSector.GetSector(sectorMatch.Value);
                EDBoxel boxel = EDBoxel.GetBoxel(boxelMatch.Value);

                return new EDSubsector(sector, boxel);
            }
            else
            {
                throw new ArgumentException($"sectorMatch: {sectorMatch.Success}; boxelMatch: {boxelMatch.Success}");
            }

        }


        public override bool Equals(object? obj)
        {
            if (obj is EDSubsector)
            {
                EDSubsector? that = obj as EDSubsector;
                return this.Sector.Equals(that.Sector) &&
                    this.Boxel.Equals(that.Boxel);
            }
            return false;

        }

        public override string ToString()
        {
            return $"{Sector} {Boxel}";
        }




        // TODO: Calculate Manhattan distance between subsectors

        // TODO: Method for determining orthogonal neighbors (and orth. + diag. neighbors?)

        // Coordinate Estimation

        public GalacticCoordinates GetSubsectorCoordinateOfCentroid()
        {
            double massCodeBoxelEdgeLength = EDBoxel.GetBoxelEdgeLength(Boxel.MassCode);

            int[] boxelCoordArray = Boxel.BoxelCoords.ToArray();

            double[] boxelCentroid = boxelCoordArray.Select(x => massCodeBoxelEdgeLength * (x + 0.5)).ToArray();
            double[] sectorMinCoords = Sector.GetMinimumCoordinates().ToArray();
            double[] subsectorCentroid = Enumerable.Zip(boxelCentroid, sectorMinCoords, (x, y) => x + y).ToArray();


            return new GalacticCoordinates(subsectorCentroid);
        }
    }
}
