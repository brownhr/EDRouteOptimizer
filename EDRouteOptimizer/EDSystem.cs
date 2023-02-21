using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EDRouteOptimizer
{

    [JsonObject(MemberSerialization.OptIn)]
    public class EDSystem
    {
        [JsonProperty]
        public string SystemName { get; set; }

        [JsonProperty]
        public ulong? ID64 { get; set; }

        [JsonProperty]
        public RouteJsonCoords Coords { get; set; }

        [JsonProperty]
        public string? Notes { get; set; }

        public EDSystem(string? systemName = null, RouteJsonCoords? coords = null, ulong? iD64 = null, string? notes = null)
        {
            SystemName = systemName;
            ID64 = iD64;
            Coords = coords;
            Notes = notes;
        }

        public override string ToString()
        {
            return SystemName;
        }

    }

    [JsonObject(MemberSerialization.OptIn)]
    public class EDProcgenSystem : EDSystem
    {


        public EDSubsector Subsector { get; set; }
        public int SubsectorID { get; set; }

        public EDProcgenSystem(EDSubsector subsector, int subsectorID)
        {
            Subsector = subsector;
            SubsectorID = subsectorID;

            string subsectorName = subsector.ToString(includeMassNum: false);
            int massNum = subsector.Boxel.MassNum;
            string massNumAndID = (massNum == 0) ? subsectorID.ToString() : (massNum.ToString() + '-' + subsectorID.ToString());

            SystemName = subsectorName + massNumAndID;
        }


        public static EDProcgenSystem ParseProcgenString(string procgenString)
        {
            Regex procgenSystemPattern =
                new Regex
                (
                    pattern: @"(?<Sector>[\w ]+(?= [A-Z]{2}-)) (?<Boxel>[A-Z]{2}-[A-Z]) (?<MassCode>[a-h](?=\d+))"
                );
            Regex massNumGreaterThanZeroPattern = new Regex(@"(?<=\d)-(?=\d)");

            Regex massNumRegexA = new Regex(@"(?<= [a-h])\d+");

            Regex massNumRegexB = new Regex(@"(?<=\d-)\d+");

            Match isValidProcgenSystem = procgenSystemPattern.Match(procgenString);

            if (!isValidProcgenSystem.Success)
            {
                throw new ArgumentException(message: "Invalid progcen regex search");
            }

            string sectorString = isValidProcgenSystem.Groups["Sector"].Value;
            string boxelCode = isValidProcgenSystem.Groups["Boxel"].Value;
            string massCode = isValidProcgenSystem.Groups["MassCode"].Value;

            Match massNumGTZero = massNumGreaterThanZeroPattern.Match(procgenString);

            string massNum;
            if (massNumGTZero.Success)
            {
                Match massNumMatch = massNumRegexB.Match(procgenString);
                massNum = massNumMatch.Groups[0].Value;
            }
            else
            {
                Match massNumMatch = massNumRegexA.Match(procgenString);
                massNum = massNumMatch.Groups[0].Value;

            }


            EDBoxel boxel = new EDBoxel(boxelCode, char.Parse(massCode), int.Parse(massNum));
            EDSector sector = EDSector.GetSector(sectorString);


            EDSubsector subsector = new EDSubsector(sector: sector, boxel: boxel);

            return new EDProcgenSystem(subsector, int.Parse(massNum));

        }

    }
}
