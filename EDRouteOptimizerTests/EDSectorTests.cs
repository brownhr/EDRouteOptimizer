using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void InitializeSectorTest(string inputSectorName)
        {
            EDSector sector = new EDSector(inputSectorName);
            Assert.AreEqual(inputSectorName, sector.SectorName);
        }

        [TestMethod]
        public void EDSectorKeyNotFoundException()
        {
            string notFound = "BBBBBBB";
            Assert.ThrowsException<KeyNotFoundException>(() => new EDSector(notFound));
        }

        [TestMethod()]
        [DynamicData(nameof(GetSectorFromCoordsData))]
        public void GetSectorFromCoordsTest(GalacticCoordinates coords, EDSector expectedSector)
        {
            EDSector actualSector = EDSector.GetSectorFromCoords(coords);

            Assert.AreEqual(expectedSector.SectorName, actualSector.SectorName);
        }

        private static IEnumerable<object[]> GetSectorFromCoordsData =>
            new List<object[]>
            {
                new object[] {new GalacticCoordinates(-1260.94,1257.75,44783.66),
                              new EDSector("Aaefong")},
                new object[] {new GalacticCoordinates(844.25, 1444.03, 13312.22),
                              new EDSector("Byeia Aerb")},
                new object[] {new GalacticCoordinates(24870.18, 449.93, 15071.09), 
                              new EDSector("Blaea Aip")}

            };

    }
}