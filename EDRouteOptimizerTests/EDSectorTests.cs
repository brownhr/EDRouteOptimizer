using Microsoft.VisualStudio.TestTools.UnitTesting;
using EDRouteOptimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EDRouteOptimizer.Tests
{
    [TestClass()]
    public class EDSectorTests
    {
        [TestMethod()]
        public void ParseSectorCSVTest()
        {
            EDSector.ParseSectorCSV();
        }

        [DataTestMethod]

        [DataRow("Graea Hypue")]
        [DataRow("Eol Prou")]
        public void EDSectorTest(string inputSectorName)
        {
            EDSector sector = new EDSector(inputSectorName);

            Assert.IsNotNull(sector);
            Assert.AreEqual(inputSectorName, sector.SectorName);
        }
    }
}