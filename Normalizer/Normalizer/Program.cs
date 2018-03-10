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
            TestKNN(@"../../Raw Data/Normalized/adult-Normalized.csv", 1);

            Console.WriteLine("\nFinish Program");
            Console.ReadLine();
        }

        public static void TestKNN(string path, int K)
        {
            List<List<string>> TrainingSet, TestingSet, DataSet;
            List<float> AccuracyRatings = new List<float>();
            int StartIndex = 0, KFold = 10;;

            KNNTest kt = new KNNTest();
            DataSet = kt.LoadCSVData(path);

            while (StartIndex < DataSet.Count)
            {
                StartIndex = kt.PrepareDataset(path, StartIndex, KFold, ref DataSet, out TestingSet, out TrainingSet);

                //Gera previsoes
                List<string> predictions = new List<string>();
                int k = K;

                for (int x = 0; x < TestingSet.Count; x++)
                {
                    List<LineDistance> neighbors = kt.GetNeighbors(TrainingSet, TestingSet[x], k);
                    string result = kt.getResponses(neighbors);
                    predictions.Add(result);
                }

                float acc = kt.getAccuracy(TestingSet, predictions);
                AccuracyRatings.Add(acc);
                Console.WriteLine("acc " + acc.ToString() + "%");   
            }

            Console.WriteLine("Accuracy after CROSS VALIDATION: " + AccuracyRatings.Average().ToString() + "%");
        }

        public static void ExecuteOutliers()
        {
            saveCsvFileWithoutOutliers(@"../../Raw Data/Abalone");
            saveCsvFileWithoutOutliers(@"../../Raw Data/Adult");
            saveCsvFileWithoutOutliers(@"../../Raw Data/Breast cancer");
            saveCsvFileWithoutOutliers(@"../../Raw Data/iris");
            saveCsvFileWithoutOutliers(@"../../Raw Data/Wine");
            saveCsvFileWithoutOutliers(@"../../Raw Data/Wine Quality");
        }

        public static void saveCsvFileWithoutOutliers(string path)
        {            
            string[] FilesPath = FileSystem.GetAllFilesInFolder(path);
            foreach (string s in FilesPath)
            {
                string[] fileName = s.Split('\\');

                List<string> FileLines = GetFileLines(s);
                ClearEmptyValues(ref FileLines);

                
                List<string> newCSV = ClearOutliers(ref FileLines, fileName[0] + " / ", fileName[1]);

                string sFile = string.Empty;
                foreach (string str in newCSV)
                {
                    sFile += str + "\n";
                }
                
                FileSystem.SaveFileContents(sFile, path + "/output/", fileName[1]);
            }

            Console.WriteLine("Completed Succesfully");
        }

        /** 
         *  Remove os outliers e retorna a estrutura montada para exportar em csv
         */
        private static List<string> ClearOutliers(ref List<string> FileLines, string DatasetFolderPath, string fileName)
        {
            List<List<string>> listOfColums = GetListOfColumns(FileLines);
            Dictionary<int, List<string>> outlierList = new Dictionary<int, List<string>>();

            for (int i = 0; i < listOfColums.Count; i++)
            {
                Console.WriteLine("Column: " + i + " of " + listOfColums.Count);

                float testString = 0;
                if (!float.TryParse(listOfColums[i][0], out testString)) //Se for string ira transformar em classe
                {
                    Dictionary<string, int> StringConverter = new Dictionary<string, int>();
                    int NewCategory = 0;

                    for (int q = 0; q < listOfColums[i].Count; q++)
                    {
                        if (StringConverter.ContainsKey(listOfColums[i][q]))
                            listOfColums[i][q] = StringConverter[listOfColums[i][q]].ToString();
                        else
                        {
                            StringConverter.Add(listOfColums[i][q], NewCategory);
                            listOfColums[i][q] = NewCategory.ToString();
                            NewCategory++;
                        }
                    }
                    continue;
                }

                List<string> tempOrderedList = new List<string>();
                tempOrderedList.AddRange(listOfColums[i]);
                tempOrderedList = tempOrderedList.OrderBy(n => decimal.Parse(n)).ToList();

                double avg = 0;
                //string q1 = tempOrderedList[(int)Math.Round((float)tempOrderedList.Count * 0.25f)].Replace('.', ',');
                //string q3 = tempOrderedList[(int)Math.Round((float)tempOrderedList.Count * 0.75f)].Replace('.', ',');
                string q1 = tempOrderedList[(int)Math.Round((float)tempOrderedList.Count * 0.25f)];
                string q3 = tempOrderedList[(int)Math.Round((float)tempOrderedList.Count * 0.75f)];

                foreach (string s in tempOrderedList)
                {
                    //string n = s.Replace('.', ',');
                    string n = s;
                    avg += double.Parse(n);
                }
                avg = Math.Round(avg / tempOrderedList.Count,2);

                float iqr = float.Parse(q3) - float.Parse(q1);
                float l_sup = (float)avg + 1.5f * iqr;
                float l_inf = (float)avg - 1.5f * iqr;
                
                //Console.WriteLine("Coluna " + i);
                //Console.WriteLine("media = " + avg);
                //Console.WriteLine("q1 = " + q1 + "\nq3 = " + q3);
                //Console.WriteLine("iqr = " + iqr.ToString());
                //Console.WriteLine("l_sup = " + l_sup + "\nl_inf = " + l_inf);

                if (q1 == q3 || Math.Abs(l_sup - l_inf) < 2f)
                    continue;
                if(iqr < 0)
                {
                    Console.WriteLine("Ops, Q3 veio maior do que Q1");
                    continue;
                }

                Console.WriteLine("Removendo outliers...");
                for (int l = 0; l < listOfColums[i].Count; l++) //para cada elemento da coluna
                {
                    //string n = listOfColums[i][l].Replace('.', ',');
                    string n = listOfColums[i][l];
                    float lineValue = float.Parse(n);
                    
                    if (lineValue > l_sup || lineValue < l_inf) //Ira remover o index
                    {
                        if (outlierList.ContainsKey(i))
                            outlierList[i].Add(n);
                        else
                            outlierList.Add(i, new List<string>() { n });

                        for (int j = 0; j < listOfColums.Count; j++) //ira remover de todas as colunas no index
                        {
                            listOfColums[j].RemoveAt(l);
                        }
                        l--;
                    }
                }

                SaveOutlierReport(ref outlierList, DatasetFolderPath + "/output/");

            }
            string[] newFileName = fileName.Split('.');
                NormalizeData(listOfColums, newFileName[0] + "-Normalized." + newFileName[1]);

            return remountLine(listOfColums);
        }

        private static List<string> remountLine(List<List<string>> listOfColums)
        {
            List<string> newCSV = new List<string>();
            for (int i = 0; i < listOfColums.Count; i++) //ira remontar as linhas sem os outliers
            {
                //Console.WriteLine("colum size = " + listOfColums[i].Count);
                for (int k = 0; k < listOfColums[i].Count; k++)
                {
                    string newLine = string.Empty;
                    if (newCSV.Count < listOfColums[i].Count)
                    {
                        //newLine = listOfColums[i][k].Replace(',','.');
                        newLine = listOfColums[i][k];
                        newCSV.Add(newLine);
                    }
                    else
                    {
                        //newLine = newCSV[k] + "," + listOfColums[i][k].Replace(',', '.');
                        newLine = newCSV[k] + "," + listOfColums[i][k];
                        newCSV[k] = newLine;
                    }
                }
            }

            return newCSV;
        }

        private static List<List<string>> GetListOfColumns(List<string> FileLines)
        {
            List<List<string>> ListOfColums = new List<List<string>>();

            foreach (string Item in FileLines)
            {
                string[] colums = Item.Split(',');

                for (int i = 0; i < colums.Length; i++)
                {
                    if (ListOfColums.Count < colums.Length)
                        ListOfColums.Add(new List<string>());

                    ListOfColums[i].Add(colums[i]);
                }
            }

            return ListOfColums;
        }

        private static void NormalizeData(List<List<string>> ListOfColumns, string fileName)
        {
            List<List<string>> NormalizedColumns = new List<List<string>>();
            for (int i = 0; i < ListOfColumns.Count; i++)
            {
                //Console.WriteLine("coluna " + i);
                
                NormalizedColumns.Add(new List<string>());

                float MinimumValue = 999999, MaximumValue = 0, Divider = 1;

                for (int n = 0; n < ListOfColumns[i].Count - 1; n++)// pega maior e menor valor
                {
                    float curr = 0;
                    if (float.TryParse(ListOfColumns[i][n], out curr)){
                        if (curr < MinimumValue)
                            MinimumValue = curr;
                        if (curr > MaximumValue)
                            MaximumValue = curr;
                    }
                }
                //if (float.TryParse(ListOfColumns[i].Max(), out MaximumValue) && float.TryParse(ListOfColumns[i].Min(), out MinimumValue))
                //{
                    Divider = Math.Abs(MaximumValue - MinimumValue);
                    //Console.WriteLine("max " + MaximumValue + " min " + MinimumValue);
                //}

                for (int j = 0; j < ListOfColumns[i].Count; j++)
                {
                    float ParseResult;
                    if (float.TryParse(ListOfColumns[i][j], out ParseResult))
                    {
                        if(i == ListOfColumns.Count - 1)
                        {
                            NormalizedColumns[i].Add(ParseResult.ToString());
                            continue;
                        }

                        float minorValue = (MinimumValue < MaximumValue) ? MinimumValue : MaximumValue;

                        float NormalizedValue = Math.Abs(ParseResult - minorValue) / Divider;
                        NormalizedColumns[i].Add(NormalizedValue.ToString());


                        //Console.WriteLine("NormalizedValue " + NormalizedValue);
                    }
                }
            }

            List<string> newCSV = remountLine(NormalizedColumns);
            string sFile = string.Empty;
            foreach (string str in newCSV)
            {
                sFile += str + "\n";
            }

            FileSystem.SaveFileContents(sFile, @"../../Raw Data/Normalized/", fileName);
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

        private static void SaveOutlierReport(ref Dictionary<int, List<string>> OutlierList, string DatasetFolderPath)
        {
            string FileReport = "";

            foreach (KeyValuePair<int, List<string>> Item in OutlierList)
            {
                FileReport += "--------" + Environment.NewLine + "Coluna " + Item.Key + Environment.NewLine;
                foreach (string Outlier in Item.Value)
                {
                    FileReport += Outlier + Environment.NewLine;
                }
            }

            FileSystem.SaveFileContents(FileReport, DatasetFolderPath, "Outlier_Report.txt");
        }
    }
}
