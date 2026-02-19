// See https://aka.ms/new-console-template for more information
using MakeJsonFile;

// TODO: update the manifest, to signal to the filament app that there are changes to the content, so that it will reimport the json file
var outpath = Path.Combine("C:\\Users\\markn\\source\\repos\\MakeJsonFile\\MakeJsonFile", "SharedFilamentData", "Colors Master.json");
var manifestPath = Path.Combine("C:\\Users\\markn\\source\\repos\\MakeJsonFile\\MakeJsonFile", "SharedFilamentData", "Manifest.json");
var inpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Colors Master.csv");
var json = System.Text.Json.JsonSerializer.Serialize(ImportColorContent.ImportContents(inpath), new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
var manifest = Manifest.GetManifest(manifestPath);
var colorsUpdated = false;
//var webManifest = await WebManifest.GetWebManifestAsync();
if (!CheckIfContentsNeedsUpdating(outpath, manifest.ColorsLastUpdated))
{
    Console.WriteLine("Contents are up to date, no need to update the json file.");
    return;
}
else
{
    Console.WriteLine("Contents need updating, updating the json file.");
    File.WriteAllText(outpath, json);
    colorsUpdated = true;
}


Manifest.UpdateManifest(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FilamentSharedContent", "Manifest.json"), colorUpdated: colorsUpdated, vendorsUpdated: false, materialUpdated: false);

static bool CheckIfContentsNeedsUpdating(string path, DateOnly lastUpdated)
{
    DateOnly fileLastUpdated = DateOnly.FromDateTime(System.IO.File.GetLastWriteTime(path));
    return fileLastUpdated > lastUpdated;
}
