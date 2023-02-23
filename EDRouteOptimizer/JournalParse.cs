using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EDRouteOptimizer
{
    public class JournalParse
    {
        private static string journal = @"%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous";
        private static string JournalDirectory = Environment.ExpandEnvironmentVariables(journal);

        private static string filter = "Journal*.log";

        private static List<string> journalFiles = System.IO.Directory.GetFiles(JournalDirectory, filter).ToList();

        public static void ParseJournals()
        {
            List<FSSEvent> FSSEvents = new List<FSSEvent>();

            List<dynamic> dynamics = new List<dynamic>();




            foreach (string file in journalFiles)
            {
                JsonTextReader jsonTextReader = new JsonTextReader(new StreamReader(file)) { SupportMultipleContent = true };

                JsonSerializer jsonSerializer = new JsonSerializer();

                while (jsonTextReader.Read())
                {
                    dynamics.Add(jsonSerializer.Deserialize<dynamic>(jsonTextReader));
                }
            }


        }
    }
}
