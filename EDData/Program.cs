using EDData;

public class Program
{
    public static string outputFilePath = @"C:\Users\brownhr\source\repos\EDRouteOptimizer\EDRouteOptimizer\sector_json.json";
    public static List<EDAstroSectorCSV> sectors = new List<EDAstroSectorCSV>();
    public static void Main()
    {

        string tempFile = Path.GetTempFileName();

        var tasks = new List<Task>();

        Task download = EDAstroDownloader.DownloadFile(EDAstroDownloader.SystemListFilename, tempFile);
        Task.WaitAll(download);

        sectors = EDAstroDownloader.ParseDownloadedCsv(tempFile);
        ManualAddSector();

        sectors.Sort((a, b) =>
        {
            int result = a.SectorName.CompareTo(b.SectorName);
            return result;
        });
        EDAstroDownloader.WriteSectorToJson(outputFilePath, sectors);


    }

    public static void ManualAddSector()
    {
        EDAstroSectorCSV Eaezi = new EDAstroSectorCSV(
            sectorName: "Eaezi",
            maxXCoord: 8895,
            maxYCoord: 2535,
            maxZCoord: 41175,
            ID64X: 45,
            ID64Y: 33,
            ID64Z: 50
            );
        sectors.Add(Eaezi);

    }


}