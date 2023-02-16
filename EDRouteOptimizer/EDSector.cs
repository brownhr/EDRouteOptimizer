using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace EDRouteOptimizer
{
    public class EDSector : ParseableSector
    {
        private static readonly string SectorCSVFilePath = @"sector_dataframe.csv";
        private static readonly Dictionary<string, ParseableSector> SectorDict = ParseSectorCSV();


        public EDSector(string sectorName)
        {

            if (SectorDict.TryGetValue(sectorName, out ParseableSector? s))
            {
                SectorName = s.SectorName;
                MaxXCoord = s.MaxXCoord;
                MaxYCoord = s.MaxYCoord;
                MaxZCoord = s.MaxZCoord;

                ID64X = s.ID64X;
                ID64Y = s.ID64Y;
                ID64Z = s.ID64Z;
            }
            else
            {
                throw new KeyNotFoundException(nameof(sectorName));
            }
        }

        public static Dictionary<string, ParseableSector> ParseSectorCSV()
        {

            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            };

            using (StreamReader reader = new StreamReader(SectorCSVFilePath))
            using (CsvReader csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<ParseableSectorMap>();
                var records = csv.GetRecords<ParseableSector>();
                return records.ToList().ToDictionary(x => x.SectorName, x => x);
            }
        }



        public static EDSector GetSectorFromCoords(GalacticCoordinates coords)
        {
            foreach (ParseableSector sector in SectorDict.Values)
            {
                if (sector.CoordInSector(coords))
                {
                    return new EDSector(sector.SectorName);
                }
            }
            throw new ArgumentOutOfRangeException(nameof(coords), message: $"Coordinates {coords.ToString()} not found within any sector");
        }





        public override string ToString()
        {
            return SectorName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SectorName);
        }

        //public static bool operator ==(EDSector? s1, EDSector? s2)
        //{

        //    return s1.Equals(s2);
        //}
        //public static bool operator !=(EDSector? s1, EDSector? s2)
        //{

        //    return !s1.Equals(s2);
        //}





        public override bool Equals(object obj)
        {
            EDSector? item = obj as EDSector;

            return item != null && SectorName == item.SectorName;
        }
    }

    public class ParseableSector
    {

        public string SectorName { get; set; }
        public int ID64X { get; set; }
        public int ID64Y { get; set; }
        public int ID64Z { get; set; }
        public double MaxYCoord { get; set; }
        public double MaxZCoord { get; set; }
        public double MaxXCoord { get; set; }


        public override int GetHashCode()
        {
            return HashCode.Combine(SectorName);
        }

        public bool CoordInSector(GalacticCoordinates coords)
        {

            bool _x = coords.x <= MaxXCoord && coords.x >= MaxXCoord - 1280;
            bool _y = coords.y <= MaxYCoord && coords.y >= MaxYCoord - 1280;
            bool _z = coords.z <= MaxZCoord && coords.z >= MaxZCoord - 1280;

            return _x && _y && _z;
        }
    }

    internal class ParseableSectorMap : ClassMap<ParseableSector>
    {
        public ParseableSectorMap()
        {
            Map(p => p.SectorName).Name("SectorName");
            Map(p => p.ID64X).Name("ID64X");
            Map(p => p.ID64Y).Name("ID64Y");
            Map(p => p.ID64Z).Name("ID64Z");

            Map(p => p.MaxXCoord).Name("MaxXCoord");
            Map(p => p.MaxYCoord).Name("MaxYCoord");
            Map(p => p.MaxZCoord).Name("MaxZCoord");
        }
    }

    internal class IDCollection
    {
        private readonly int ID64X;
        private readonly int ID64Y;
        private readonly int ID64Z;
        public IDCollection(int ID64X, int ID64Y, int ID64Z)
        {
            this.ID64X = ID64X;
            this.ID64Y = ID64Y;
            this.ID64Z = ID64Z;
        }
        public SectorCoordinateCollection TranslateIDToCoordinate()
        {
            double cx = ((ID64X - 39) * 1280) - 65;
            double cy = ((ID64Y - 32) * 1280) - 25;
            double cz = ((ID64Z - 19) * 1280) + 215;

            return new SectorCoordinateCollection(cx, cy, cz);

        }
    }

    internal class SectorCoordinateCollection
    {
        private readonly double coordinateX;
        private readonly double coordinateY;
        private readonly double coordinateZ;
        public SectorCoordinateCollection(double coordinateX, double coordinateY, double coordinateZ)
        {
            this.coordinateX = coordinateX;
            this.coordinateY = coordinateY;
            this.coordinateZ = coordinateZ;
        }

        public IDCollection TranslateCoordinatesToID()
        {
            int idx = (int)((coordinateX + 65) / 1280) + 39;
            int idy = (int)((coordinateY + 25) / 1280) + 32;
            int idz = (int)((coordinateZ - 215) / 1280) + 19;
            return new IDCollection(idx, idy, idz);
        }
    }
}
