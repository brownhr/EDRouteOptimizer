using Microsoft.VisualStudio.TestTools.UnitTesting;
using EDRouteOptimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDRouteOptimizer.Tests
{
    [TestClass()]
    public class GalacticCoordinatesTests
    {
        [TestMethod()]
        public void GalacticCoordinatesTest()
        {
            double[] coords = new double[3] { 0, 0, 0 };
            GalacticCoordinates gCoords = new GalacticCoordinates(coords);
        }

        [TestMethod]
        public void GalacticCoordinatesOutOfRangeTest()
        {
            double[] coords = new double[4] { 0, 1, 2, 3 };

            Assert.ThrowsException<IndexOutOfRangeException>(() => { new GalacticCoordinates(coords); });
        }
    }
}