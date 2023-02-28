using EDRouteOptimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfEDRO.Models
{
    public class SubSectorModel
    {
        private EDSubsector? _subsector;
        

        public SubSectorModel(string subsectorName)
        {
           _subsector = EDSubsector.TryParse(subsectorName, out EDSubsector? result) ? result : null;
        }


    }
}
