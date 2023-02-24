using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using Newtonsoft.Json;

namespace EDData
{
    internal class EDAstroDownloader
    {
        internal static string SystemListFilename = @"https://edastro.com/mapcharts/files/sector-list.csv";



        internal static async Task DownloadFile(string url, string outputFile)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                await response.Content.CopyToAsync(fs);
            }
        }

        internal static List<EDAstroSectorCSV> ParseDownloadedCsv(string filePath
            )
        {
            List<EDAstroSectorCSV> result;

            result = File.ReadAllLines(filePath)
                .Skip(1)
                .Select(row => EDAstroSectorCSV.FromCsv(row))
                .ToList();
            return result;
        }


        internal static void WriteSectorToJson(string outFilePath, List<EDAstroSectorCSV> sectorList)
        {
            using (StreamWriter file = new StreamWriter(outFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, sectorList);
            }

        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class EDAstroSectorCSV
    {
        [JsonProperty]
        public string SectorName { get; set; }

        [JsonProperty]
        public double MaxXCoord { get; set; }
        [JsonProperty]
        public double MaxYCoord { get; set; }
        [JsonProperty]
        public double MaxZCoord { get; set; }



        [JsonProperty]
        public int? ID64X { get; set; }
        [JsonProperty]
        public int? ID64Y { get; set; }
        [JsonProperty]
        public int? ID64Z { get; set; }

        public EDAstroSectorCSV(string sectorName, double maxXCoord, double maxYCoord, double maxZCoord, int? ID64X, int? ID64Y, int? ID64Z)
        {
            SectorName = sectorName;
            MaxXCoord = maxXCoord;
            MaxYCoord = maxYCoord;
            MaxZCoord = maxZCoord;
            this.ID64X = ID64X;
            this.ID64Y = ID64Y;
            this.ID64Z = ID64Z;
        }

        public static EDAstroSectorCSV FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');

            string sectorName = Convert.ToString(values[0].Trim('"'));

            double MaxXCoord = Convert.ToDouble(values[8]);
            double MaxYCoord = Convert.ToDouble(values[9]);
            double MaxZCoord = Convert.ToDouble(values[10]);

            int? IDX = Func.ParseBlankInt(values[14]);
            int? IDY = Func.ParseBlankInt(values[15]);
            int? IDZ = Func.ParseBlankInt(values[16]);

            return new EDAstroSectorCSV(sectorName, MaxXCoord, MaxYCoord, MaxZCoord, IDX, IDY, IDZ);
        }


    }

    public static class Func
    {
        public static int? ParseBlankInt(this string s)
        {
            int i;
            return Int32.TryParse(s, out i) ? i : null;
        }
    }
}
