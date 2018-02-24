using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizer
{
    class Program
    {
        static void Main(string[] args)
        {
            string FilePath = FileSystem.GetAllFilesInFolder(@"../../Raw Data/Wine")[0];
            List<string> FileLines = GetFileLines(FilePath);
            ClearEmptyValues(ref FileLines);
            ClearOutliers(ref FileLines);

            Console.ReadLine();
        }

        /** Vai dar ruim caso tenha letra em uma linha
         *  TODO: Limpar outliers e melhorar o metodo para funcionar por coluna
         */
        private static List<double> ClearOutliers(ref List<string> FileLines)
        {
            List<double> ConvertedList = new List<double>();
            foreach(string Item in FileLines)
            {
                ConvertedList.AddRange(Item.Split(',').Select(item => double.Parse(item)).ToList());
            }

            return ConvertedList;
        }

        private static void ClearEmptyValues(ref List<string> FileLines)
        {
            for (int i = 0; i < FileLines.Count(); i++)
                if (FileLines[i].Split(',').Contains("")) { FileLines.RemoveAt(i); }
        }

        private static List<string> GetFileLines(string FilePath)
        {
            string FileContents = FileSystem.GetFileContents(FilePath);
            List<string> FileLines = FileContents.Split(Environment.NewLine.ToCharArray()).ToList();
            FileLines.RemoveAll(item => item == "");

            return FileLines;
        }
    }
}
