using System;
using System.Collections.Generic;
using System.Text;

namespace MakeJsonFile
{
    internal class ImportMaterialContent
    {
        public required string Name { get; set; }
        public required string ScientificName { get; set; }

        internal static bool MaterialNeedsUpdating(string inpath, DateOnly lastUpdated)
        {
            try
            {
                DateOnly fileLastUpdated = DateOnly.FromDateTime(System.IO.File.GetLastWriteTime(inpath));
                return fileLastUpdated > lastUpdated;
            }
            catch (FileNotFoundException ex)
            {
                return true;
            }
        }
        internal static List<ImportMaterialContent> ImportContents(string inpath)
        {
            List<ImportMaterialContent> list = new List<ImportMaterialContent>();
            using var reader = new StreamReader(inpath);
            reader.ReadLine(); // skip header
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    var values = ImportColorContent.csvRegex.Split(line); // Assuming simple CSV without quoted fields
                    list.Add(new ImportMaterialContent { Name = values[0], ScientificName = values[1] });
                }
            }
            return list;
        }
    }
}
