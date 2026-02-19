using System;
using System.Collections.Generic;
using System.Text;

namespace MakeJsonFile
{
    public class Manifest
    {
        public DateOnly ColorsLastUpdated { get; set; }
        public DateOnly VendorsLastUpdated { get; set; }
        public DateOnly MaterialLastUpdated { get; set; }
        public static void UpdateManifest(string filePath, bool colorUpdated, bool vendorsUpdated, bool materialUpdated)
        {
            string manifestContent = "{}";
            try
            {
                manifestContent = System.IO.File.ReadAllText(filePath);
                Manifest current = System.Text.Json.JsonSerializer.Deserialize<Manifest>(manifestContent) ?? new Manifest();
                if (colorUpdated)
                {
                    current.ColorsLastUpdated = DateOnly.FromDateTime(DateTime.Now);
                }
                if (vendorsUpdated)
                {
                    current.VendorsLastUpdated = DateOnly.FromDateTime(DateTime.Now);
                }
                if (materialUpdated)
                {
                    current.MaterialLastUpdated = DateOnly.FromDateTime(DateTime.Now);
                }
                string json = System.Text.Json.JsonSerializer.Serialize(current, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(filePath, json);
                Console.WriteLine(json);
            }
            catch (FileNotFoundException)
            {
                Manifest manifest = new Manifest() { ColorsLastUpdated = DateOnly.FromDateTime(DateTime.Today), MaterialLastUpdated = DateOnly.FromDateTime(DateTime.Today), VendorsLastUpdated = DateOnly.FromDateTime(DateTime.Today) };
                string json = System.Text.Json.JsonSerializer.Serialize(manifest, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);
                System.IO.File.WriteAllText(filePath, json);
            }
        }
        public static Manifest GetManifest(string filePath)
        {
            string manifestContent = System.IO.File.ReadAllText(filePath);
            Manifest current = System.Text.Json.JsonSerializer.Deserialize<Manifest>(manifestContent) ?? new Manifest();
            return current;
        }
    }
}
