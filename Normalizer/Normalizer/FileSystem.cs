using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizer
{
    class FileSystem
    {
        public static string[] GetAllFilesInFolder(string Path)
        {
            if (!Directory.Exists(Path)) { Console.WriteLine("Path does not exist!"); Environment.Exit(-1); }
            return Directory.GetFiles(Path);
        }

        public static string GetFileContents(string Path)
        {
            if (!File.Exists(Path)) { Console.WriteLine("Path does not exist!"); Environment.Exit(-1); }
            return File.ReadAllText(Path);
        }

        public static void SaveFileContents(string data, string Path, string filename)
        {
            //Console.WriteLine("Salvando arquivo - " + filename);
            string outputFolder = Path;
            if (!File.Exists(outputFolder)) {
                Directory.CreateDirectory(outputFolder);
            }

            File.WriteAllText(outputFolder + filename, data);

            Console.WriteLine("Saved file " + filename);
        }
    }
}
