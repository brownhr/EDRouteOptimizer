using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using Newtonsoft.Json;

namespace EDRouteOptimizer
{


    [JsonObject(MemberSerialization.OptIn)]
    public class EDSector
    {
        private static string JsonFilePath = "sector_json.json";
        private static readonly Dictionary<string, EDSector> EDSectorDictionary = ReadJson();

        [JsonProperty]
        public string SectorName { get; set; }
        [JsonProperty]
        public int ID64X { get; set; }
        [JsonProperty]
        public int ID64Y { get; set; }
        [JsonProperty]
        public int ID64Z { get; set; }
        [JsonProperty]
        public double MaxXCoord { get; set; }
        [JsonProperty]
        public double MaxYCoord { get; set; }
        [JsonProperty]
        public double MaxZCoord { get; set; }


        public GalacticCoordinates MinimumCoordinates;

        public GalacticCoordinates GetMinimumCoordinates()
        {
            return new GalacticCoordinates(MaxXCoord - 1280, MaxYCoord - 1280, MaxZCoord - 1280);
        }



        public static Dictionary<string, EDSector> ReadJson()
        {
            using (StreamReader reader = new StreamReader(JsonFilePath))
            {
                string json = reader.ReadToEnd();
                List<EDSector> items = JsonConvert.DeserializeObject<List<EDSector>>(json);

                if (items == null)
                {
                    throw new NullReferenceException(nameof(items));
                }

                return items.ToDictionary(x => x.SectorName, x => x);

            }
        }

        public static EDSector GetSector(string sectorName)
        {
            if (EDSectorDictionary.TryGetValue(sectorName, out EDSector? sector)) { return sector; }

            else
            {
                throw new KeyNotFoundException(message: $"Invalid sector: {nameof(sectorName)}");
            }


        }

        public static EDSector GetSectorFromCoords(GalacticCoordinates coords)
        {
            foreach (EDSector sector in EDSectorDictionary.Values)
            {
                if (sector.CoordInSector(coords))
                {
                    return sector;
                }
            }
            throw new ArgumentOutOfRangeException(nameof(coords), message: $"Coordinates {coords} not found within any sector");
        }

        public static EDSector? GetSectorFromIDs(int IDX, int IDY, int IDZ)
        {
            try
            {
                EDSector? sector =
                    EDSectorDictionary.First(
                        x =>
                        x.Value.ID64X == IDX &&
                        x.Value.ID64Y == IDY &&
                        x.Value.ID64Z == IDZ
                        ).Value;
                return sector;
            }
            catch (InvalidOperationException e)
            {
                return null;
            }
        }

        public static EDSector? GetSectorFromIDs(int[] IDArray)
        {
            if (IDArray.Length != 3)
            {
                throw new IndexOutOfRangeException(nameof(IDArray));
            }

            return GetSectorFromIDs(IDArray[0], IDArray[1], IDArray[2]);
        }


        public bool CoordInSector(GalacticCoordinates coords)
        {
            bool _x = coords.x <= MaxXCoord && coords.x >= MaxXCoord - 1280;
            bool _y = coords.y <= MaxYCoord && coords.y >= MaxYCoord - 1280;
            bool _z = coords.z <= MaxZCoord && coords.z >= MaxZCoord - 1280;

            return _x && _y && _z;
        }
        public List<EDSector?> GetNeighboringSectors()
        {
            int[] offsetX = { -1, 0, 1 };
            int[] offsetY = { -1, 0, 1 };
            int[] offsetZ = { -1, 0, 1 };

            int[][] cartesianProduct = (
                from x in offsetX
                from y in offsetY
                from z in offsetZ
                select new int[] { x, y, z }).ToArray();

            List<EDSector?> result = new List<EDSector?>();

            foreach (int[] off in cartesianProduct)
            {
                int[] offsetCoords = Enumerable.Zip(IDArray(), off, (x, y) => x + y).ToArray();
                EDSector? sector = EDSector.GetSectorFromIDs(offsetCoords);
                if (sector.SectorName != SectorName)
                {
                    result.Add(sector);

                };
            }
            return result;

        }

        public string PrintIDs()
        {
            return string.Join(", ", IDArray());
        }

        public override string ToString()
        {
            return SectorName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SectorName);
        }

        public override bool Equals(object? obj)
        {
            if (obj is EDSector)
            {
                var that = obj as EDSector;
                return this.SectorName.Equals(that.SectorName);
            }
            return false;
        }

        public int[] IDArray()
        {
            return new int[3] { ID64X, ID64Y, ID64Z };
        }
    }
}