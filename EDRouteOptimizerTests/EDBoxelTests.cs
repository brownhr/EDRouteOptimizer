using EDRouteOptimizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EDRouteOptimizer.Tests
{
    [TestClass()]
    public class EDBoxelTests
    {
        [DataTestMethod]
        [DataRow('a', true)]
        [DataRow('h', true)]
        [DataRow('i', false)]
        [DataRow('A', false)]
        [DataRow('2', false)]
        public void IsValidMasscodeTest(char inputMC, bool expectedOutput)
        {
            bool actualOutput = EDBoxel.IsValidMasscode(inputMC);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [DataTestMethod]
        [DataRow('a', 10d)]
        [DataRow('b', 20d)]
        [DataRow('h', 1280d)]
        public void GetBoxelEdgeLengthTest(char inputMC, double expectedOutput)
        {
            double actualOutput = EDBoxel.GetBoxelEdgeLength(inputMC);
            Assert.AreEqual(expectedOutput, actualOutput, delta: 1e-6);
        }


        [DataTestMethod]
        [DataRow("AA-A", new char[3] { 'A', 'A', 'A' })]
        [DataRow("CL-Y", new char[3] { 'Y', 'L', 'C' })]
        [DataRow("RT-Y", new char[3] { 'Y', 'T', 'R' })]
        public void GetBoxelCharTest(string boxelCode, char[] expectedBoxelChar)
        {
            EDBoxel box = new EDBoxel(boxelCode: boxelCode, massCode: 'd', massNum: 0);
            char[] actualBoxelChar = box.GetBoxelChar(boxelCode);
            CollectionAssert.AreEqual(actualBoxelChar, expectedBoxelChar);
        }


        [DataTestMethod]
        [DataRow(0, new int[] { 0, 0, 0 })]
        [DataRow(1, new int[] { 1, 0, 0 })]
        [DataRow(1024, new int[] { 10, 13, 1 })]
        [DataRow(65535, new int[] { 15, 24, 18 })]
        public void DecomposeBase26Test(int n, int[] expectedOutput)
        {
            int[] actualOutput = EDBoxel.DecomposeBase26(n);
            CollectionAssert.AreEqual(actualOutput, expectedOutput);
        }

        [DataTestMethod]
        [DataRow(new char[] { 'A', 'A', 'A' }, 0)]
        [DataRow(new char[] { 'A', 'A', 'B' }, 1)]
        [DataRow(new char[] { 'Y', 'L', 'C' }, 16512)]
        [DataRow(new char[] { 'Y', 'L', 'D' }, 16513)]
        public void BoxelCharToIntTest(char[] boxelChar, int expectedOutput)
        {
            int actualOutput = EDBoxel.BoxelCharToIndex(boxelChar);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [DataTestMethod]
        [DataRow(new int[] { 0, 0, 0 }, new char[] { 'A', 'A', 'A' })]
        [DataRow(new int[] { 25, 25, 25 }, new char[] { 'Z', 'Z', 'Z' })]
        public void IntToBoxelCharArrayTest(int[] ints, char[] expectedOutput)
        {
            char[] actualOutput = EDBoxel.IntToBoxelCharArray(ints);

            CollectionAssert.AreEqual(actualOutput, expectedOutput);


        }

        [DataTestMethod()]
        [DataRow(0, 0, 0, 0)]
        [DataRow(3, 1, 0, 131)]
        [DataRow(0, 1, 1, 16512)]

        public void CoordToBoxelIndexTest(int x, int y, int z, int expectedIndex)
        {
            BoxelCoord coord = new BoxelCoord(x, y, z);

            int actualIndex = EDBoxel.CoordToBoxelIndex(coord);

            Assert.AreEqual(expectedIndex, actualIndex);

        }

        [DataTestMethod()]
        [DataRow(new char[] { 'A', 'A', 'A' }, "AA-A")]
        [DataRow(new char[] { 'C', 'L', 'Y' }, "CL-Y")]
        public void CharArrayToBoxelStringTest(char[] charArray, string expectedString)
        {
            string actualString = EDBoxel.CharArrayToBoxelString(charArray);
            Assert.AreEqual(expectedString, actualString);

        }

        private static IEnumerable<object[]> GetBoxelFromCoordinatesData =>
            new List<object[]>
            {
                new object[] {new BoxelCoord(0, 0, 0), 'd', new EDBoxel("AA-A", 'd', 0)},
                new object[] {new BoxelCoord(1, 1, 1), 'g', new EDBoxel("DL-Y", 'g', 0)},
                new object[] {new BoxelCoord(28, 22, 24), 'c', new EDBoxel("CX-N", 'c', 22)}
            };

        [TestMethod()]
        [DynamicData(nameof(GetBoxelFromCoordinatesData))]
        public void GetBoxelFromCoordinatesTest(BoxelCoord coord, char massCode, EDBoxel expectedBoxel)
        {
            EDBoxel actualBoxel = EDBoxel.GetBoxelFromCoordinates(coord, massCode);
            Assert.AreEqual(expectedBoxel, actualBoxel);
        }

        [TestMethod]
        [DynamicData(nameof(GetChildBoxelData))]

        public void GetChildBoxelsTest(string parentBoxels, List<string>? expectedChildBoxelStrings)
        {
            EDBoxel parentBoxel = EDBoxel.ParseBoxelFromString(parentBoxels);
            List<EDBoxel>? actualChildBoxels = parentBoxel.GetChildBoxels();

            List<EDBoxel>? expectedChildren = new List<EDBoxel>();
            if (expectedChildBoxelStrings == null)
            {
                Assert.IsNull(actualChildBoxels);
            }
            if (expectedChildBoxelStrings != null)
            {
                foreach (string s in expectedChildBoxelStrings)
                {
                    expectedChildren.Add(EDBoxel.ParseBoxelFromString(s));
                }



                foreach (EDBoxel e in actualChildBoxels)
                {
                    CollectionAssert.Contains(expectedChildren, e);
                }
            }

        }

        private static IEnumerable<object[]> GetChildBoxelData =>
            new List<object[]>
            {
                new object[] {       "AA-A h0",
                new List<string> {      "AA-A g0", "BA-A g0",
                                     "YE-A g0", "ZE-A g0",
                                     "EG-Y g0", "FG-Y g0",
                                     "CL-Y g0", "DL-Y g0"}},
                new object[] {"AA-A a0", null}
            };



        [TestMethod()]
        [DynamicData(nameof(GetParentBoxelData))]
        public void GetParentBoxelTest(EDBoxel childBoxel, EDBoxel? expectedParentBoxel)
        {
            EDBoxel? actualParentBoxel = childBoxel.GetParentBoxel();
            Assert.AreEqual(expectedParentBoxel, actualParentBoxel);
        }

        private static IEnumerable<object[]> GetParentBoxelData =>
            new List<object[]>
            {
                new object[] {EDBoxel.ParseBoxelFromString("CL-Y g0"), EDBoxel.ParseBoxelFromString("AA-A h0")},
                new object[] {EDBoxel.ParseBoxelFromString("AA-A h0"), null},
                new object[] {EDBoxel.ParseBoxelFromString("GX-L d7"), EDBoxel.ParseBoxelFromString("QY-S e3")}
            };
    }

    [TestClass()]
    public class CoordTests
    {
        [DataTestMethod]
        [DataRow(0, 0, 0, 0)]
        [DataRow(0, 0, 1, 16384)]
        [DataRow(12, 23, 34, 560012)]
        [DataRow(7, 8, 9, 148487)]
        public void BoxelCoordToBoxelIndexTest(int x, int y, int z, int expectedOutput)
        {
            BoxelCoord bc = new BoxelCoord(x, y, z);
            int actualOutput = bc.ToBoxelIndex();
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [DataTestMethod()]
        [DataRow(0, 0, 0, 0)]
        [DataRow(1024, 0, 8, 0)]
        [DataRow(16384, 0, 0, 1)]
        [DataRow(65535, 127, 127, 3)]
        [DataRow(28607, 63, 95, 1)]

        public void IndexToBoxelCoordTest(int index, int x_out, int y_out, int z_out)
        {
            BoxelCoord actualOutput = EDBoxel.IndexToBoxelCoord(index);
            Assert.AreEqual(x_out, actualOutput.x);
            Assert.AreEqual(y_out, actualOutput.y);
            Assert.AreEqual(z_out, actualOutput.z);

        }

    }

    [TestClass()]
    public class BoxelTests
    {

        [TestMethod()]
        [DataRow("AA-A", 'h', 0, 0, 0, 0)]
        [DataRow("YK-G", 'b', 11, 44, 8, 12)]
        [DataRow("QL-O", 'c', 6, 22, 4, 7)]
        [DataRow("TC-V", 'd', 2, 11, 2, 3)]
        [DataRow("BA-A", 'g', 0, 1, 0, 0)]
        [DataRow("ZE-A", 'g', 0, 1, 1, 0)]
        [DataRow("DL-Y", 'g', 0, 1, 1, 1)]


        public void CreateBoxelTest(string boxelCode, char massCode, int massNum, int _x, int _y, int _z)
        {
            //Thread.Sleep(3000);
            EDBoxel box = new EDBoxel(boxelCode, massCode, massNum);
            BoxelCoord coord = new BoxelCoord(X: _x, Y: _y, Z: _z);
            Assert.AreEqual(coord.x, box.Coordinates.x);
            Assert.AreEqual(coord.y, box.Coordinates.y);
            Assert.AreEqual(coord.z, box.Coordinates.z);
        }


        [DataTestMethod]
        [DataRow("AA-A h0", "AA-A", 'h', 0)]
        [DataRow("WV-A c15", "WV-A", 'c', 15)]
        [DataRow("YK-N d7", "YK-N", 'd', 7)]
        public void ParseBoxelFromStringTest(string input, string boxelCode, char massCode, int massNum)
        {
            EDBoxel expectedBoxel = new EDBoxel(boxelCode, massCode, massNum);
            EDBoxel actualBoxel = EDBoxel.ParseBoxelFromString(input);

            Assert.AreEqual(expectedBoxel, actualBoxel);
        }

        [DataTestMethod]
        [DataRow("aaa H0")]
        [DataRow("12354")]
        [DataRow("Keltim")]
        public void ParseBoxelFromStringRegexFailureTest(string input)
        {
            Assert.ThrowsException<ArgumentException>(() => EDBoxel.ParseBoxelFromString(input));

        }


        [DataTestMethod]
        [DataRow('h', 1)]
        [DataRow('g', 8)]
        [DataRow('f', 64)]
        [DataRow('e', 512)]

        public void GetNumBoxelsInMasscodeTest(char inputMasscode, int expectedBoxels)
        {
            Assert.AreEqual(EDBoxel.GetNumBoxelsInMasscode(inputMasscode), expectedBoxels);

        }


        [DataTestMethod]
        [DataRow("AA-A h0", true)]
        [DataRow("CL-Y d0", true)]
        [DataRow("ZZ-Z g4", false)]
        [DataRow("AA-A h1", false)]


        public void IsValidBoxelTest(string inputBoxelString, bool expectedOutput)
        {
            EDBoxel testBoxel = EDBoxel.ParseBoxelFromString(inputBoxelString);
            bool actualOutput = testBoxel.IsValidBoxel();

            Assert.AreEqual(expectedOutput, actualOutput);

        }

        [DataTestMethod]
        [DataRow("AA-A h0", "AA-A h0", true)]
        [DataRow("RT-Y d2", "RT-Y d2", true)]
        [DataRow("AA-A h0", "CL-Y d0", false)]
        [DataRow("AA-A h0", "AA-A d0", false)]
        [DataRow("AA-A d0", "AA-A d1", false)]
        [DataRow("BA-A d1", "AA-A d1", false)]
        public void BoxelEqualsTest(string inputStringA, string inputStringB, bool expectedOutput)
        {
            EDBoxel boxA = EDBoxel.ParseBoxelFromString(inputStringA);
            EDBoxel boxB = EDBoxel.ParseBoxelFromString(inputStringB);
            bool actualOutput = boxA.Equals(boxB);
            Assert.AreEqual(actualOutput, expectedOutput);
            Assert.AreEqual(boxB.Equals(boxA), expectedOutput);
        }
    }
}