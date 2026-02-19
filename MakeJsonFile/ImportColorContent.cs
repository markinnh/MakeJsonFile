using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MakeJsonFile
{
    public class ImportColorContent
    {
        public static readonly Regex csvRegex = new System.Text.RegularExpressions.Regex(@",(?=(?:[^""]*""[^""]*"")*[^""]*$)");

        public string Name { get; set; }
        public string ColorRGB { get; set; }
        public string FilamentType { get; set; }
        public string VendorName { get; set; }
        public ImportColorContent(string name, string colorRGB, string filamentType, string vendorName)
        {
            Name = name;
            ColorRGB = colorRGB;
            FilamentType = filamentType;
            VendorName = vendorName;
        }
        public static List<ImportColorContent> ImportContents(string inpath) 
        {
            List<ImportColorContent> list = new List<ImportColorContent>();
            using var reader = new StreamReader(inpath);
            reader.ReadLine(); // skip header
           
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    var values = csvRegex.Split(line);
                    list.Add(new ImportColorContent(values[0], values[1], values[2], values[3]));
                }
            }
            return list;
        }
    }
}
