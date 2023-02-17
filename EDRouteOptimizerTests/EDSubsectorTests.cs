﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                new object[] { EDSector.GetSector("Byeia Aerb"), EDSector.GetSector("Chroarsts"), 1},
                new object[] { EDSector.GetSector("Byua Aerb"), EDSector.GetSector("Ruekaei"), 2},
                new object[] { EDSector.GetSector("Byua Aerb"), EDSector.GetSector("Blau Ais"), 3}
            };

    }
}