using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EDRouteOptimizer.Tests
{
    [TestClass()]
    public class EDSubsectorTests
    {
        [TestMethod()]
        [DynamicData(nameof(ManhattanDistanceBetweenSectorsData))]
        public void ManhattanDistanceBetweenSectorsTest(EDSector sectorA, EDSector sectorB, int expectedManhattanDistance)
        {
            int actualManhattanDistance = EDSubsector.ManhattanDistanceBetweenSectors(sectorA, sectorB);

            Assert.AreEqual(actualManhattanDistance, expectedManhattanDistance);

        }

        private static IEnumerable<object[]> ManhattanDistanceBetweenSectorsData =>
            new List<object[]>
            {
                new object[] { new EDSector("Byeia Aerb"), new EDSector("Chroarsts"), 1},
                new object[] { new EDSector("Byua Aerb"), new EDSector("Ruekaei"), 2},
                new object[] { new EDSector("Byua Aerb"), new EDSector("Blau Ais"), 3}
            };

    }
}