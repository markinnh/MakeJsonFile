// See https://aka.ms/new-console-template for more information
using MakeJsonFile;
using System.Reflection;

// TODO: update the manifest, to signal to the filament app that there are changes to the content, so that it will reimport the json file
var executingPath = Assembly.GetExecutingAssembly().Location;
var projectDirectory = executingPath.Substring(0, executingPath.IndexOf("\\bin"));  //this is a bit hacky but it works, we want to get the path to the project directory, which is the parent of the bin directory where the executable is located
var colorsOutpath = Path.Combine(projectDirectory, "SharedFilamentData", "Colors Master.json");
var manifestPath = Path.Combine(projectDirectory, "SharedFilamentData", "Manifest.json");
var materialOutpath = Path.Combine(projectDirectory, "SharedFilamentData", "material.json");
var vendorOutpath = Path.Combine(projectDirectory, "SharedFilamentData", "Vendors.json");
var colorsInpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Colors Master.csv");
var materialInpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Material Master.csv");
var inpathVendors = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Filament\\Exchange\\Vendors", "Vendors.csv");
var manifest = Manifest.GetManifest(manifestPath);
var colorsUpdated = false;
var webManifest = await WebManifest.GetWebManifestAsync();
bool updateVendors = false;
bool materialUpdated = false;
if (CheckIfContentsNeedsUpdating(colorsInpath, manifest.ColorsLastUpdated))
{
    Console.WriteLine("Colors need updating, updating the json file.");
    var json = System.Text.Json.JsonSerializer.Serialize(ImportColorContent.ImportContents(colorsInpath), new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(colorsOutpath, json);
    colorsUpdated = true;
}
if (CheckIfVendorsNeedsUpdating(inpathVendors, webManifest.VendorsLastUpdated)) { 
    Console.WriteLine("Vendors need updating, updating the json file.");
    var vendors = ImportVendorContent.ImportContents(inpathVendors);

    var json = System.Text.Json.JsonSerializer.Serialize(vendors, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(vendorOutpath, json);
    updateVendors = true;
}

if (CheckIfContentsNeedsUpdating(materialInpath, webManifest.MaterialLastUpdated))
{
    Console.WriteLine("Material needs updating, updating the material json file.");
    var materials = ImportMaterialContent.ImportContents(materialInpath);
    var json = System.Text.Json.JsonSerializer.Serialize(materials, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(materialOutpath, json);
    materialUpdated = true;
}

if (colorsUpdated || updateVendors || materialUpdated)
    Manifest.UpdateManifest(Path.Combine(projectDirectory, "SharedFilamentData", "Manifest.json"), colorUpdated: colorsUpdated, vendorsUpdated: updateVendors, materialUpdated: materialUpdated);

static bool CheckIfContentsNeedsUpdating(string path, DateOnly lastUpdated)
{
    DateOnly fileLastUpdated = DateOnly.FromDateTime(System.IO.File.GetLastWriteTime(path));
    return fileLastUpdated > lastUpdated;
}

static bool CheckIfVendorsNeedsUpdating(string inpath, DateOnly lastUpdated)
{
    DateOnly fileLastUpdated = DateOnly.FromDateTime(System.IO.File.GetLastWriteTime(inpath));
    //result = fileLastUpdated > lastUpdated;
    return fileLastUpdated > lastUpdated;
}