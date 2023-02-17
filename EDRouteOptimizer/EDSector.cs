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

        public bool CoordInSector(GalacticCoordinates coords)
        {
            bool _x = coords.x <= MaxXCoord && coords.x >= MaxXCoord - 1280;
            bool _y = coords.y <= MaxYCoord && coords.y >= MaxYCoord - 1280;
            bool _z = coords.z <= MaxZCoord && coords.z >= MaxZCoord - 1280;

            return _x && _y && _z;
        }

        public override string ToString()
        {
            return SectorName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SectorName);
        }

        public override bool Equals(object obj)
        {
            EDSector? item = obj as EDSector;

            return item != null && SectorName == item.SectorName;
        }
    }
}
