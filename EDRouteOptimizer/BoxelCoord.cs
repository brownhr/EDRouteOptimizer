namespace EDRouteOptimizer
{
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

        public BoxelCoord(int[] array)
        {
            if (array.Length != 3) throw new IndexOutOfRangeException(nameof(array));
            x = array[0];
            y = array[1];
            z = array[2];
        }

        public static BoxelCoord operator +(BoxelCoord lhs, int[] rhs)
        {
            int[] bCoordArray = lhs.ToArray();
            int[] result = new int[3];

            result = Enumerable.Zip(bCoordArray, rhs, (x, y) => x + y).ToArray();
            return new BoxelCoord(result);
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

        public int[] ToArray()
        {
            return new int[] { x, y, z };
        }

    }

    public class SectorCoordinates
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;


        public SectorCoordinates(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public SectorCoordinates(int[] array)
        {
            if (array.Length != 3) throw new IndexOutOfRangeException(nameof(array));

            this.x = array[0];
            this.y = array[1];
            this.z = array[2];
        }

        public static SectorCoordinates operator +(SectorCoordinates sCoordA, int[] offset)
        {
            int[] coordArray = sCoordA.ToArray();
            int[] result = new int[3];
            result = Enumerable.Zip(coordArray, offset, (x, y) => x + y).ToArray();

            return new SectorCoordinates(result);
        }


        public int[] ToArray()
        {
            return new int[] { x, y, z };
        }
    }

    public class GalacticCoordinates
    {
        public readonly double x;
        public readonly double y;
        public readonly double z;

        public GalacticCoordinates(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double[] ToArray()
        {
            return new double[3] { x, y, z };
        }


        public GalacticCoordinates(double[] coordArray)
        {
            if (coordArray.Length != 3)
            {
                throw new IndexOutOfRangeException(nameof(coordArray));
            }

            x = coordArray[0];
            y = coordArray[1];
            z = coordArray[2];
        }

        public override string ToString()
        {
            return $"({x:F3}, {y:F3}, {z:F3})";
        }


    }
}
