using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace EDRouteOptimizer
{
    public class EDSector
    {
        private static readonly string SectorCSVFilePath = @"sector_dataframe.csv";
        public static readonly List<ParseableSector> SectorList = ParseSectorCSV();


        public static readonly int SectorLYDist = 1280;
        public readonly string SectorName;


        public readonly double MinXCoord;
        public readonly double MinYCoord;
        public readonly double MinZCoord;
        public readonly double MaxXCoord;
        public readonly double MaxYCoord;
        public readonly double MaxZCoord;

        public readonly int ID64X;
        public readonly int ID64Y;
        public readonly int ID64Z;

        public EDSector(string SectorName)
        {
            this.SectorName = SectorName;
            bool success = false;

            foreach (ParseableSector sector in SectorList)
            {

                if (this.SectorName == sector.SectorName)
                {
                    success = true;
                    ID64X = sector.ID64X;
                    ID64Y = sector.ID64Y;
                    ID64Z = sector.ID64Z;

                    MaxXCoord = sector.MaxXCoord;
                    MaxYCoord = sector.MaxYCoord;
                    MaxZCoord = sector.MaxZCoord;
                }


            }
            if (success)
            {
                MinXCoord = MaxXCoord - SectorLYDist;
                MinYCoord = MaxYCoord - SectorLYDist;
                MinZCoord = MaxZCoord - SectorLYDist;
            }
            else
            {
                throw new Exception(message: "Invalid Sector; Sector name must be fully procgen.");
            }
        }


        public override string ToString()
        {
            return SectorName;
        }



        public static List<ParseableSector> ParseSectorCSV()
        {
            using (StreamReader? reader = new StreamReader(SectorCSVFilePath))
            using (CsvReader? csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                //csv.Context.RegisterClassMap<EDSectorMap>();
                var records = csv.GetRecords<ParseableSector>();

                return records.ToList();
            }
        }
    }

    public class ParseableSector
    {

        public string SectorName { get; set; }
        public int ID64X { get; set; }
        public int ID64Y { get; set; }
        public int ID64Z { get; set; }
        public int MaxYCoord { get; set; }
        public int MaxZCoord { get; set; }
        public int MaxXCoord { get; set; }


    }

    internal class EDSectorMap : ClassMap<EDSector>
    {
        public EDSectorMap()
        {
            Map(p => p.SectorName).Index(0);
            Map(p => p.ID64X).Index(1);
            Map(p => p.ID64Y).Index(2);
            Map(p => p.ID64Z).Index(3);

            Map(p => p.MaxXCoord).Index(4);
            Map(p => p.MaxYCoord).Index(5);
            Map(p => p.MaxZCoord).Index(6);
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
            double cx = ((ID64X - 39) * EDSector.SectorLYDist) - 65;
            double cy = ((ID64Y - 32) * EDSector.SectorLYDist) - 25;
            double cz = ((ID64Z - 19) * EDSector.SectorLYDist) + 215;

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


    public class EDSubsector
    {

    }
}
