using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EDRouteOptimizer
{
    public class EDSubsector
    {
        public readonly EDSector Sector;
        public readonly EDBoxel Boxel;


        public EDSubsector(EDSector sector, EDBoxel boxel)
        {
            Sector = sector;
            Boxel = boxel;
        }

        public EDSubsector(string parseableInputString)
        {
            EDSector sec = EDSector.GetSector(parseableInputString);
            EDBoxel box = EDBoxel.GetBoxel(parseableInputString);

            Sector = sec;
            Boxel = box;
        }

        public static int ManhattanDistanceBetweenSectors(EDSector sectorA, EDSector sectorB)
        {

            int[] sectorAIDs = new int[3] { sectorA.ID64X, sectorA.ID64Y, sectorA.ID64Z };
            int[] sectorBIDs = new int[3] { sectorB.ID64X, sectorB.ID64Y, sectorB.ID64Z };

            int[] axisDistances = Enumerable.Zip(sectorAIDs, sectorBIDs, (a, b) => (int)Math.Abs(a - b)).ToArray();

            return axisDistances.Sum();
        }


        public EDSubsector? ShiftSubsectorByOffset(int[] offsetArray)
        {
            if (offsetArray.Length != 3)
            {
                throw new IndexOutOfRangeException(nameof(offsetArray));
            }
            int maxCoordInMassCode = (int)Math.Cbrt(EDBoxel.GetNumBoxelsInMasscode(Boxel.MassCode));

            BoxelCoord bcoords = Boxel.BoxelCoords;
            int[] coordWrap = new int[3];

            // AA-A g0
            // [0, 0, 0] + [0, 3, 5] => [0, 1, 0], Sector.Y + 1
            int[] coordAdd = (bcoords + offsetArray).ToArray();

            coordWrap = coordAdd.Select(x => x % maxCoordInMassCode).ToArray();

            int[] sectorAdd = coordAdd.Select(x => x / maxCoordInMassCode).ToArray();
            SectorCoordinates sectorMove = Sector.GetSectorCoordinates() + sectorAdd;


            EDSector? newSector = EDSector.GetSectorFromIDs(sectorMove);


            EDBoxel newBoxel = EDBoxel.GetBoxelFromBoxelCoordinates(new BoxelCoord(coordWrap), Boxel.MassCode);

            if (newSector == null)
            {
                return null;
            }

            return new EDSubsector(newSector, newBoxel);

        }

        public List<EDSubsector> GetChildSubsectors(char recursiveMassCodeLimit)
        {
            if (!EDBoxel.IsValidMasscode(recursiveMassCodeLimit))
            {
                throw new ArgumentException(message: $"Invalid masscode passed to {nameof(recursiveMassCodeLimit)}");
            }

            char currentMassCode = Boxel.MassCode;
            List<EDSubsector> allChildren = new List<EDSubsector>() { this };

            while (currentMassCode > recursiveMassCodeLimit && currentMassCode > 'a')
            {
                List<EDSubsector> children = allChildren
                     .Where(e => e.Boxel.MassCode == currentMassCode)
                     .ToList()
                     .SelectMany(child => child.ChildSubsectors())
                     .ToList();
                allChildren.AddRange(children);

                currentMassCode--;
            }
            allChildren.Remove(this);
            return allChildren;
        }

        private List<EDSubsector> ChildSubsectors()
        {
            List<EDSubsector> results = new List<EDSubsector>();
            this.Boxel.GetChildBoxels()
                .ForEach(box => results.Add(new EDSubsector(this.Sector, box)));
            return results;
        }


        //public bool Equals(EDSubsector? obj)
        //{
        //    if (obj == null) return false;
        //    return (Sector == obj.Sector && Boxel == obj.Boxel);
        //}

        public static EDSubsector GetSubsector(string input)
        {
            Regex sectorRegex = new Regex(@"(?<sector>[\w ]+(?= [A-Z]{2}-[A-Z]))");
            Regex boxelRegex = new Regex(@"(?<boxel>[A-Z]{2}-[A-Z] [a-h]\d+)");

            Match sectorMatch = sectorRegex.Match(input);
            Match boxelMatch = boxelRegex.Match(input);

            if (sectorMatch.Success && boxelMatch.Success)
            {

                EDSector sector = new EDSector();
                sector = EDSector.GetSector(sectorMatch.Value);
                EDBoxel boxel = EDBoxel.GetBoxel(boxelMatch.Value);

                return new EDSubsector(sector, boxel);
            }
            else
            {
                throw new ArgumentException($"sectorMatch: {sectorMatch.Success}; boxelMatch: {boxelMatch.Success}");
            }

        }


        public override bool Equals(object? obj)
        {
            if (obj is EDSubsector)
            {
                EDSubsector? that = obj as EDSubsector;
                return this.Sector.Equals(that.Sector) &&
                    this.Boxel.Equals(that.Boxel);
            }
            return false;

        }

        public override string ToString()
        {
            return $"{Sector} {Boxel}";
        }

        public string ToString(bool includeMassNum)
        {
            return $"{Sector} {Boxel.BoxelCode} {Boxel.MassCode}";

        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Sector.SectorName,
                                    Boxel.BoxelCode,
                                    Boxel.MassCode,
                                    Boxel.MassNum);
        }



        // TODO: Method for determining orthogonal neighbors (and orth. + diag. neighbors?)

        // Coordinate Estimation

        public GalacticCoordinates GetSubsectorCoordinateOfCentroid()
        {
            double massCodeBoxelEdgeLength = EDBoxel.GetBoxelEdgeLength(Boxel.MassCode);

            int[] boxelCoordArray = Boxel.BoxelCoords.ToArray();

            double[] boxelCentroid = boxelCoordArray.Select(x => massCodeBoxelEdgeLength * (x + 0.5)).ToArray();
            double[] sectorMinCoords = Sector.GetMinimumCoordinates().ToArray();
            double[] subsectorCentroid = Enumerable.Zip(boxelCentroid, sectorMinCoords, (x, y) => x + y).ToArray();


            return new GalacticCoordinates(subsectorCentroid);
        }
    }
}
