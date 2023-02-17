using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EDRouteOptimizer.Tests
{
    [TestClass()]
    public class EDSectorTests
    {
        //[TestMethod()]
        //public void ParseSectorCSVTest()
        //{
        //    EDSector.ParseSectorCSV();
        //}

        [DataTestMethod]
        [DataRow("Graea Hypue")]
        public void InitializeSectorTest(string inputSectorName)
        {
            EDSector sector = EDSector.GetSector(inputSectorName);
            Assert.AreEqual(inputSectorName, sector.SectorName);
        }

        [TestMethod]
        public void ReadSectorJsonTest()
        {
            Dictionary<string, EDSector> sectorList = EDSector.ReadJson();
            Console.WriteLine(sectorList.Count);

        }

        [TestMethod]
        public void EDSectorKeyNotFoundException()
        {
            string notFound = "BBBBBBB";
            Assert.ThrowsException<KeyNotFoundException>(() => EDSector.GetSector(notFound));
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
                              EDSector.GetSector("Aaefong")},
                new object[] {new GalacticCoordinates(844.25, 1444.03, 13312.22),
                              EDSector.GetSector("Byeia Aerb")},
                new object[] {new GalacticCoordinates(24870.18, 449.93, 15071.09),
                              EDSector.GetSector("Blaea Aip")}

            };

    }
}