// See https://aka.ms/new-console-template for more information
namespace EDRouteOptimizer
{
    public class Program
    {
        public static void Main()
        {
            EDSector secA = EDSector.GetSector("Eishoqs");
            EDSector secB = EDSector.GetSector("Iowhophs");

            int mDist = EDSubsector.ManhattanDistanceBetweenSectors(secA, secB);

            Console.WriteLine(mDist);
            Console.WriteLine(string.Join(", ", secA.IDArray()));
            Console.WriteLine(string.Join(", ", secB.IDArray()));
        }
    }
}