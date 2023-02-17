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

        [TestMethod()]
        [DynamicData(nameof(ShiftSubsectorByOffsetData))]
        public void ShiftSubsectorByOffsetTest(EDSubsector inputSubsector, int[] offsetArray, EDSubsector? expectedSubsector)
        {
            EDSubsector? actualSubsector = inputSubsector.ShiftSubsectorByOffset(offsetArray);

            if (expectedSubsector == null)
            {
                Assert.IsNull(actualSubsector);
            }
            else
            {
                Assert.IsNotNull(actualSubsector);
                Assert.AreEqual(expectedSubsector, actualSubsector,
                    message: $"Actual: {actualSubsector}; Expected: {expectedSubsector}");
            }

        }

        public static IEnumerable<object[]> ShiftSubsectorByOffsetData =>
            new List<object[]>
            {
                new object[]
                {
                    new EDSubsector(EDSector.GetSector("Graea Hypue"),
                                    EDBoxel.GetBoxel("AA-A g0")),
                    new int[] {0, 3, 0 },
                    new EDSubsector(EDSector.GetSector("Puekee"),
                                    EDBoxel.GetBoxel("YE-A g0"))
                },
                new object[]
                {
                    new EDSubsector(EDSector.GetSector("Graea Hypue"),
                                    EDBoxel.GetBoxel("AA-A h0")),
                    new int[] {0, 10, 0 },
                    null
                },
                new object[]
                {
                    new EDSubsector(EDSector.GetSector("Byeia Aerb"),
                                    EDBoxel.GetBoxel("AA-A b0")),
                    new int[] {45, 9, 14 },
                    new EDSubsector(EDSector.GetSector("Byeia Aerb"),
                                    EDBoxel.GetBoxel("FC-D b13"))
                }
            };

        [TestMethod()]
        [DynamicData(nameof(GetSubsectorData))]
        public void GetSubsectorTest(string input, EDSubsector expectedSubsector)
        {
            EDSubsector actualSubsector = EDSubsector.GetSubsector(input);
            Assert.AreEqual(expectedSubsector, actualSubsector);

        }

        private static IEnumerable<object[]> GetSubsectorData =>
            new List<object[]>
            {
                new object[]
                {
                    "Graea Hypue RT-Y d2",
                    new EDSubsector(EDSector.GetSector("Graea Hypue"),
                                    EDBoxel.GetBoxel("RT-Y d2"))
                }
            };

        [TestMethod()]
        public void GetSubsectorFailureTest()
        {
            string failure = "Efinowke";
            Assert.ThrowsException<ArgumentException>(() => { EDSubsector.GetSubsector(failure); });

        }
    }
}