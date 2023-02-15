﻿using System;
using System.Text.RegularExpressions;
using System.Configuration;

namespace EDRouteOptimizer

{

    public class EDSystem
    {



    }

    public class EDSubsector
    {

    }

    public class EDSector
    {


    }

    public class EDBoxel
    {
        private static readonly char[] ValidMassCodes = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        private static readonly char[] alphaUppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private static readonly string boxelRegexPattern =
            @"(?<boxelCode>[A-Z]{2}-[A-Z]) (?<massCode>[a-h])(?<massNum>\d+)";


        private static readonly int MaxNum = (int)Math.Pow(26, 3);
        private static readonly int MaxIndex = int.MaxValue;

        public readonly string BoxelCode;
        public readonly char MassCode;

        public readonly int MassNum;
        public readonly int MassNum2;


        private readonly int BoxelIndex;
        private readonly char[] BoxelChar;

        private readonly int Remainder;
        private readonly int N2;

        public BoxelCoord Coordinates;

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
            BoxelIndex = BoxelCharToIndex(BoxelChar) + massNum * MaxNum;

            Remainder = BoxelIndex % MaxNum;
            N2 = BoxelIndex / MaxNum;

            Coordinates = IndexToBoxelCoord(BoxelIndex);


        }

        public static EDBoxel ParseBoxelFromString(string boxelString)
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
            bool indexInValidRange = BoxelIndex <= MaxIndex;

            int numBoxelsInMassCode = GetNumBoxelsInMasscode(MassCode);
            int maxCoordinateAlongAxis = (int)Math.Cbrt(numBoxelsInMassCode) - 1;

            bool coordinatesInValidRange =
                Coordinates.x <= maxCoordinateAlongAxis &&
                Coordinates.y <= maxCoordinateAlongAxis &&
                Coordinates.z <= maxCoordinateAlongAxis;

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

        public override bool Equals(object? obj)
        {
            return Equals(obj as EDBoxel);
        }

        public bool Equals(EDBoxel? obj)
        {
            return obj != null &&
                //obj.GetType() == GetType() &&
                obj.BoxelCode == BoxelCode &&
                obj.MassCode == MassCode &&
                obj.MassNum == MassNum;
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
    public class BoxelCoord
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        public BoxelCoord(int X, int Y, int Z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public bool Equals(BoxelCoord other)
        {
            if (other == null) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            if (other.x == x && other.y == y && other.z == z)
            {
                return base.Equals((BoxelCoord)other);
            }
            else
            {
                return false;
            }
        }


        public int ToBoxelIndex()
        {
            return x + 128 * y + (int)Math.Pow(128, 2) * z;
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