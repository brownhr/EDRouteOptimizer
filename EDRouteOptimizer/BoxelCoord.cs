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

        public override string ToString()
        {
            return $"({x:F3}, {y:F3}, {z:F3})";
        }


    }
}
