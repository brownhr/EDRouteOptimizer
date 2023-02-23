using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDRouteOptimizer;

namespace EDROForms
{
    internal class FormControl
    {
        public static EDSubsector? inputSubsector;
        public static EDSubsector? outputSubsector = null;
        public static void UpdateSubsector(string inputText)
        {
            try
            {
                EDSubsector s = EDSubsector.GetSubsector(inputText);
                inputSubsector = s;

            }
            catch (ArgumentException)
            {
                inputSubsector = null;
            }
        }

    }
}
