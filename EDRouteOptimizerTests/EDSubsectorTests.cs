using EDRouteOptimizer;
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
                new object[] { EDSector.GetSector("Byeia Aerb"), EDSector.GetSector("Chroarsts"), 1},
                new object[] { EDSector.GetSector("Byua Aerb"), EDSector.GetSector("Ruekaei"), 2},
                new object[] { EDSector.GetSector("Byua Aerb"), EDSector.GetSector("Blau Ais"), 3}
            };

        [TestMethod()]
        [DynamicData(nameof(GetNeighboringSectorsData))]
        public void GetNeighboringSectorsTest(string inputSectorString, string[] expectedNeighborStrings)
        {
            EDSector inputSector = EDSector.GetSector(inputSectorString);
            int expectedCountNull = expectedNeighborStrings.Count(s => s == null);


            List<EDSector?> actualSectors = inputSector.GetNeighboringSectors();
            int countNull = 0;

            List<string> strings = new List<string>();
            foreach (EDSector? actualSector in actualSectors)
            {
                if (actualSector == null)
                {
                    countNull++;
                }
                else
                {
                    string name = actualSector.SectorName;
                    strings.Add(name);
                }


            }
            foreach (string s in expectedNeighborStrings)
            {
                if (s != null)
                {
                    Assert.IsTrue(
                        Array.Exists(
                            strings.ToArray(), e => e == s), 
                        message: $"Actual neighbors does not contain sector {s}"                        
                        );

                }
            }

            foreach (string s in strings)
            {
                if (s != null)
                {
                    Assert.IsTrue(Array.Exists(expectedNeighborStrings, e => e == s),
                        message: $"Expected neighbors does not contain sector {s}");

                }
            }


            Assert.AreEqual(expectedCountNull, countNull, message: $"Difference in null elements between expected and actual neighbors");


        }

        private static IEnumerable<object[]> GetNeighboringSectorsData =>
            new List<object[]>
            {
                new object[] {"Eishoqs",
                    new string?[26] {
                        "Eolls Flye", "Lyamboi", "Boerns",
                        "Eock Flye", "Boets", "Dryeia Flye",
                        "Boewls", "Eolls Flyiae", "Dryua Flyiae",

                        "Iowhophs", "Eishorks", "Whanuae",
                        "Iowhaib",              "Whani",
                        "Iowhaiscs", "Eishorps", "Dryo Fleau",

                        "Byae Aihn", "Ooshairld", "Dryo Scroe",
                        "Sloeths", "Eowyg Scroe", "Dryeae Scroe",
                        "Byae Airk", "Eocs Scrio", "Synambae"
                    }
                }

            };
    }
}