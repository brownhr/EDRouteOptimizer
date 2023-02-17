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

        //public EDSector GetNeighboringSectors(string direction)
        //{



        //}

        



        // TODO: Calculate Manhattan distance between subsectors

        // TODO: Method for determining orthogonal neighbors (and orth. + diag. neighbors?)


    }
}
