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

            // TODO: Calculate Manhattan distance between subsectors

            // TODO: Method for determining orthogonal neighbors (and orth. + diag. neighbors?)
    }
}
