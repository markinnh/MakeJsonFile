using System;
using System.Collections.Generic;
using System.Text;

namespace MakeJsonFile
{
    internal class JsonUpdater
    {
        public static void UpdateJsonFile(string filePath, string newContent)
        {
            try
            {
                // Write the new content to the specified file path
                System.IO.File.WriteAllText(filePath, newContent);
                Console.WriteLine($"Successfully updated the JSON file at: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating the JSON file: {ex.Message}");
            }
        }
    }
}
