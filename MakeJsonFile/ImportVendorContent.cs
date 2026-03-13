using System;
using System.Collections.Generic;
using System.Text;

namespace MakeJsonFile
{
    internal class ImportVendorContent
    {
        public required string Name { get; set; }
        public required string Website { get; set; }
        public bool FoundOnAmazon { get; set; }

        internal static List<ImportVendorContent> ImportContents(string inpath)
        {
            List<ImportVendorContent> list = new List<ImportVendorContent>();
            using var reader = new StreamReader(inpath);
            reader.ReadLine(); // skip header
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    var values = line.Split(',');
                    list.Add(new ImportVendorContent { Name = values[0], Website = values[1], FoundOnAmazon = bool.Parse(values[2]) });
                }
            }
            return list;
        }
    }
}
