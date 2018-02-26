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
            /*string FilePath = FileSystem.GetAllFilesInFolder(@"../../Raw Data/iris")[0];
            List<string> FileLines = GetFileLines(FilePath);
            ClearEmptyValues(ref FileLines);

            List<string> newCSV = ClearOutliers(ref FileLines);
            foreach (string s in newCSV)
                Console.WriteLine(s);*/
                
            saveCsvFileWithoutOutliers(@"../../Raw Data/Abalone");
            saveCsvFileWithoutOutliers(@"../../Raw Data/Adult");
            saveCsvFileWithoutOutliers(@"../../Raw Data/Breast cancer");
            saveCsvFileWithoutOutliers(@"../../Raw Data/iris");
            saveCsvFileWithoutOutliers(@"../../Raw Data/Wine");
            saveCsvFileWithoutOutliers(@"../../Raw Data/Wine Quality");
            Console.ReadLine();

        }

        public static void saveCsvFileWithoutOutliers(string path)
        {
            Console.WriteLine("This could take a while, please wait until complete");
            
            string[] FilesPath = FileSystem.GetAllFilesInFolder(path);
            foreach (string s in FilesPath)
            {
                string[] fileName = s.Split('\\');

                List<string> FileLines = GetFileLines(s);
                ClearEmptyValues(ref FileLines);

                List<string> newCSV = ClearOutliers(ref FileLines);

                string sFile = string.Empty;
                foreach (string str in newCSV)
                {
                    sFile += str + "\n";
                }

                FileSystem.SaveFileContents(sFile, path, fileName[1]);

                //Console.WriteLine(fileName[1]);
            }

            Console.WriteLine("complete\n");
        }

        /** 
         *  Remove os outliers e retorna a estrutura montada para exportar em csv
         */
        private static List<string> ClearOutliers(ref List<string> FileLines)
        {
            List<List<string>> listOfColums = new List<List<string>>();

            foreach (string Item in FileLines)
            {
                string[] colums = Item.Split(',');

                for (int i = 0; i < colums.Length; i++)
                {
                    if (listOfColums.Count < colums.Length)
                    {
                        listOfColums.Add(new List<string>());
                        
                        //TODO Gambiarra para ordenar q nao funcionou
                        float testString = 0;
                        if (!float.TryParse(colums[i], out testString))
                            listOfColums[i].Add(colums[i]);
                        else
                        {
                            if (colums[i].Length == 1)
                                colums[i] = "0" + colums[i];


                            listOfColums[i].Add(colums[i]);
                        }
                    }
                    else
                    {
                        //TODO Gambiarra para ordenar q nao funcionou
                        float testString = 0;
                        if (!float.TryParse(colums[i], out testString))
                            listOfColums[i].Add(colums[i]);
                        else
                        {
                            if (colums[i].Length == 1)
                                colums[i] = "0" + colums[i];
                            
                            listOfColums[i].Add(colums[i]);
                        }
                    }
                }
            }

            List<int> outlierIndex = new List<int>();
            for (int i = 0; i < listOfColums.Count; i++)
            {
                float testString = 0;
                if (!float.TryParse(listOfColums[i][0], out testString))
                    continue;

                List<string> tempOrderedList = new List<string>();
                tempOrderedList.AddRange(listOfColums[i]);
                //TODO ordenar a lista de string em ordem, nao ta funcionando ainda
                tempOrderedList = tempOrderedList.OrderBy(n => n).ToList();

                double media = 0;
                string q1 = tempOrderedList[(int)Math.Round((float)tempOrderedList.Count * 0.25f)].Replace('.', ',');
                string q3 = tempOrderedList[(int)Math.Round((float)tempOrderedList.Count * 0.75f)].Replace('.', ',');

                foreach (string s in tempOrderedList)
                {
                    string n = s.Replace('.', ',');
                    media += double.Parse(n);
                }
                media = Math.Round(media / tempOrderedList.Count,2);

                float iqr = float.Parse(q3) - float.Parse(q1);
                float l_sup = (float)media + 1.5f * iqr;
                float l_inf = (float)media - 1.5f * iqr;
                
                Console.WriteLine("Coluna " + i);
                Console.WriteLine("media = " + media);
                Console.WriteLine("q1 = " + q1 + "\nq3 = " + q3);
                Console.WriteLine("iqr = " + iqr.ToString());
                Console.WriteLine("l_sup = " + l_sup + "\nl_inf = " + l_inf);

                if (q1 == q3 || iqr < 0)
                    continue;

                Console.WriteLine("removendo outliers");

                NormalizeData(listOfColums);

                for (int l = 0; l < listOfColums[i].Count; l++) //para cada elemento da coluna
                {
                    string n = listOfColums[i][l].Replace('.', ',');
                    float lineValue = float.Parse(n);
                    
                    if (lineValue > l_sup || lineValue < l_inf) //Ira remover o index
                    {
                        for (int j = 0; j < listOfColums.Count; j++) //ira remover de todas as colunas no index
                        {
                            listOfColums[j].RemoveAt(l);
                        }
                        l--;
                    }
                }
            }

            List<string> newCSV = new List<string>();
            for (int i = 0; i < listOfColums.Count; i++) //ira remontar as linhas sem os outliers
            {
                //Console.WriteLine("colum size = " + listOfColums[i].Count);
                for (int k = 0; k < listOfColums[i].Count; k++)
                {
                    string newLine = string.Empty;
                    if (newCSV.Count < listOfColums[i].Count)
                    {
                        newLine = listOfColums[i][k];
                        newCSV.Add(newLine);
                    }
                    else
                    {
                        newLine = newCSV[k] + "," + listOfColums[i][k];
                        newCSV[k] = newLine;
                    }
                }
            }

            return newCSV;
        }

        private static void NormalizeData(List<List<string>> ListOfColumns)
        {
            List<List<string>> NormalizedColumns = new List<List<string>>();
            for (int i = 0; i < ListOfColumns.Count; i++)
            {
                int NewCategory = 0;
                NormalizedColumns.Add(new List<string>());
                Dictionary<string, int> StringConverter = new Dictionary<string, int>();

                float MinimumValue = 0, MaximumValue = 0, Divider = 1;
                if (float.TryParse(ListOfColumns[i].Max(), out MaximumValue) && float.TryParse(ListOfColumns[i].Min(), out MinimumValue))
                    Divider = MaximumValue - MinimumValue;

                for (int j = 0; j < ListOfColumns[i].Count; j++)
                {
                    float ParseResult;
                    if (!float.TryParse(ListOfColumns[i][j], out ParseResult))
                    {
                        if (StringConverter.ContainsKey(ListOfColumns[i][j]))
                            NormalizedColumns[i].Add(StringConverter[ListOfColumns[i][j]].ToString());
                        else
                        {
                            StringConverter.Add(ListOfColumns[i][j], NewCategory);
                            NormalizedColumns[i].Add(NewCategory.ToString());
                            NewCategory++;
                        }
                    } else
                    {
                        float NormalizedValue = (ParseResult - MinimumValue) / Divider;
                        NormalizedColumns[i].Add(NormalizedValue.ToString());
                    }
                }
            }
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

    public class CustomComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var regex = new System.Text.RegularExpressions.Regex("^(d+)");

            // run the regex on both strings
            var xRegexResult = regex.Match(x);
            var yRegexResult = regex.Match(y);

            // check if they are both numbers
            if (xRegexResult.Success && yRegexResult.Success)
            {
                return int.Parse(xRegexResult.Groups[1].Value).CompareTo(int.Parse(yRegexResult.Groups[1].Value));
            }

            // otherwise return as string comparison
            return x.CompareTo(y);
        }
    }
}
