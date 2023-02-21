using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace EDRouteOptimizer

{

    public class EDBoxel
    {
        private static readonly char[] ValidMassCodes = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        private static readonly char[] alphaUppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private static readonly string boxelRegexPattern =
            @"(?<boxelCode>[A-Z]{2}-[A-Z]) (?<massCode>[a-h])(?<massNum>\d+)";

        //private EDBoxel? _parentBoxel;
        //private List<EDBoxel>? _children = new List<EDBoxel>();


        private static readonly int MaxNum = (int)Math.Pow(26, 3);
        private static readonly int MaxIndex = Int32.MaxValue;

        public readonly string BoxelCode;
        public readonly char MassCode;

        public readonly int MassNum;
        public readonly int MassNum2;


        private readonly int BoxelCubeNumber;
        private readonly char[] BoxelChar;


        public BoxelCoord BoxelCoords;
        public GalacticCoordinates GalacticCoords;

        // TODO: Implement binary boxel tree ID
        private Int32 BoxelID;


        public EDBoxel(string boxelCode, char massCode, int massNum)
        {
            BoxelCode = boxelCode;
            MassCode = massCode;
            MassNum = massNum;

            if (!IsValidMasscode(MassCode))
            {
                throw new ArgumentOutOfRangeException(nameof(MassCode), message: "Invalid Masscode");
            }

            BoxelChar = GetBoxelChar(BoxelCode);
            BoxelCubeNumber = BoxelCharToIndex(BoxelChar) + massNum * MaxNum;
            BoxelCoords = IndexToBoxelCoord(BoxelCubeNumber);
        }

        public static EDBoxel GetBoxel(string boxelString)
        {
            Regex boxelRX = new Regex(boxelRegexPattern);

            Match match = boxelRX.Match(boxelString);

            if (!match.Success)
            {
                throw new ArgumentException(message: "Invalid Boxel Code");
            }
            else
            {
                string boxelCode = match.Groups["boxelCode"].Value;
                string massCode = match.Groups["massCode"].Value;
                string massNum = match.Groups["massNum"].Value;

                EDBoxel result = new EDBoxel(boxelCode, char.Parse(massCode), int.Parse(massNum));
                return result;
            }
        }


        public static BoxelCoord IndexToBoxelCoord(int index)
        {
            (int coord_z, int remainder) = Util.Divmod(index, (int)Math.Pow(128, 2));
            (int coord_y, int coord_x) = Util.Divmod(remainder, 128);
            BoxelCoord result = new BoxelCoord(coord_x, coord_y, coord_z);
            return result;
        }

        public static bool IsValidMasscode(char massCode)
        {
            return Array.Exists(ValidMassCodes, x => x == massCode);
        }

        public bool IsValidBoxel()
        {
            bool indexInValidRange = BoxelCubeNumber <= MaxIndex;

            int numBoxelsInMassCode = GetNumBoxelsInMasscode(MassCode);
            int maxCoordinateAlongAxis = (int)Math.Cbrt(numBoxelsInMassCode) - 1;

            bool coordinatesInValidRange =
                BoxelCoords.x <= maxCoordinateAlongAxis &&
                BoxelCoords.y <= maxCoordinateAlongAxis &&
                BoxelCoords.z <= maxCoordinateAlongAxis;

            bool isValidChar = true;

            foreach (char c in BoxelChar)
            {
                isValidChar = isValidChar && Array.Exists(alphaUppercase, x => x == c);
            }
            return isValidChar && indexInValidRange && coordinatesInValidRange;

        }


        public static int GetNumBoxelsInMasscode(char massCode)
        {
            int numBoxels = (int)Math.Pow(8, 8 - GetCharPositionInAlphabet(massCode));
            return numBoxels;
        }

        public static double GetBoxelEdgeLength(char massCode)
        {
            if (!IsValidMasscode(massCode))
            {
                throw new ArgumentOutOfRangeException(nameof(massCode), "Invalid masscode");
            }
            int mcIndex = GetCharPositionInAlphabet(massCode) - 1;
            return Math.Pow(2, mcIndex) * 10;
        }

        private static int GetCharPositionInAlphabet(char massCode)
        {

            return char.ToUpper(massCode) - 64;
        }

        public char[] GetBoxelChar(string boxelCode)
        {
            List<char> bc = new List<char>();
            for (int i = 0; i < boxelCode.Length; i++)
            {
                if (boxelCode[i] != '-')
                {
                    bc.Add(boxelCode[i]);
                }
            }
            char[] rev = bc.ToArray();
            Array.Reverse(rev);
            return rev;
        }


        public static int BoxelCharToIndex(char[] boxelChar)
        {
            Array.Reverse(boxelChar);
            int sum = 0;
            for (int i = 0; i < boxelChar.Length; i++)
            {
                sum += (GetCharPositionInAlphabet(boxelChar[i]) - 1) * (int)Math.Pow(26, i);
            }
            return sum;
        }

        public static int[] DecomposeBase26(int n)
        {
            int[] result = new int[3];
            int remainder;
            for (int i = 0; i < 3; i++)
            {
                remainder = n % 26;
                n /= 26;
                result[i] = remainder;
            }
            return result;
        }


        public List<EDBoxel>? GetChildBoxels()
        {
            if (MassCode == 'a') return null;

            char childMassCode = (char)(MassCode - 1);

            int[] dX = { 0, 1 };
            int[] dY = { 0, 1 };
            int[] dZ = { 0, 1 };
            var cartesianProduct =
                (from x in dX
                 from y in dY
                 from z in dZ
                 select new int[] { x, y, z });
            int[] coordArray = BoxelCoords.ToArray();

            int[][] children = new int[8][];
            int[][] prod = cartesianProduct.ToArray();

            for (int i = 0; i < prod.Length; i++)
            {
                children[i] = Enumerable.Zip(coordArray, prod[i], (x, y) => (2 * x) + y).ToArray();
            }
            EDBoxel[] childBoxels = new EDBoxel[children.Length];
            for (int i = 0; i < children.Length; i++)
            {
                EDBoxel box = GetBoxelFromBoxelCoordinates(
                    coordinates: new BoxelCoord(
                        children[i][0],
                        children[i][1],
                        children[i][2]),
                    massCode: childMassCode);
                childBoxels[i] = box;
            }
            return childBoxels.ToList();

        }

        public EDBoxel? GetParentBoxel()
        {
            if (MassCode == 'h') return null;

            char parentMassCode = (char)(MassCode + 1);

            int[] coordArray = BoxelCoords.ToArray();

            int[] parentArray = coordArray.Select(c => c / 2).ToArray();

            return GetBoxelFromBoxelCoordinates(new BoxelCoord(parentArray[0], parentArray[1], parentArray[2]), parentMassCode);
        }


        public static EDBoxel GetBoxelFromBoxelCoordinates(BoxelCoord coordinates, char massCode)
        {
            int index = CoordToBoxelIndex(coordinates);
            int massNum = index / MaxNum;
            int remainder = index % MaxNum;
            int[] intArray = DecomposeBase26(remainder);
            char[] charArray = IntToBoxelCharArray(intArray);
            string boxelCode = CharArrayToBoxelString(charArray);

            return new EDBoxel(boxelCode, massCode, massNum);
        }

        public static char[] IntToBoxelCharArray(int[] intArray)
        {
            char[] charArray = new char[intArray.Length];
            for (int i = 0; i < intArray.Length; i++)
            {
                charArray[i] = (char)('A' + intArray[i]);
            }
            return charArray;

        }

        public static string CharArrayToBoxelString(char[] charArray)
        {
            return new string(charArray[..2]) + "-" + new string(charArray[2..]);
        }

        public static int CoordToBoxelIndex(BoxelCoord coordinates)
        {
            int _z = coordinates.z * 128 * 128;
            int _y = coordinates.y * 128;
            int _x = coordinates.x;

            return _x + _y + _z;
        }




        public override bool Equals(object? obj)
        {
            if (obj is EDBoxel)
            {
                var that = obj as EDBoxel;

                return this.BoxelCode.Equals(that.BoxelCode) &&
                    this.MassCode.Equals(that.MassCode) &&
                    this.MassNum.Equals(that.MassNum);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BoxelCode, MassCode, MassNum);
        }

        public override string? ToString()
        {
            return $"{BoxelCode} {MassCode}{MassNum}";
        }




    }


    public static class Util
    {
        public static (int, int) Divmod(int num, int baseNum)
        {
            int div = num / baseNum;
            int rem = num % baseNum;
            return (div, rem);
        }
    }


}