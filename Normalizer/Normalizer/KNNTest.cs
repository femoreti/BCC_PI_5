using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizer
{
    class KNNTest
    {
        public List<List<string>> LoadCSVData(string path)
        {

            Console.WriteLine("Starting KNN from " + path);

            List<List<string>> FullDataset = new List<List<string>>();
            string FileContents = FileSystem.GetFileContents(path);
            List<string> SplitLines = FileContents.Split(Environment.NewLine.ToCharArray()).ToList();
            SplitLines.RemoveAll(item => item == "");

            for (int i = 0; i < SplitLines.Count(); i++)
            {
                List<string> NewLine = new List<string>();
                NewLine.AddRange(SplitLines[i].Split(','));
                FullDataset.Add(NewLine);
            }
            var rnd = new Random();

            FullDataset = FullDataset.OrderBy(n => rnd.Next()).ToList();
            return FullDataset;
        }

        public void PrepareTrainingAndTesting(string path, float split, out List<List<string>> TestList, out List<List<string>> TrainingList)
        {
            TestList = new List<List<string>>();
            TrainingList = new List<List<string>>();

            List<List<string>> DataSet = LoadCSVData(path);

            int trainingIndex = (int)Math.Floor(DataSet.Count * split);

            for (int x = 0; x < DataSet.Count; x++)
            {
                if (x < trainingIndex)
                {
                    TrainingList.Add(DataSet[x]);
                }
                else
                {
                    TestList.Add(DataSet[x]);
                }
            }

            Console.WriteLine("Finish prepare");
        }

        double GetEuclideanDistance(List<string> linha1, List<string> linha2)
        {
            double distance = 0;
            for (int i = 0; i < linha1.Count-1; i++)
            {
                double Testing = 0, Training = 0;
                if (!double.TryParse(linha1[i], out Training) || !double.TryParse(linha2[i], out Testing))
                {
                    Console.WriteLine("ERROR: Unable to cast string to double.");
                }
                distance += Math.Pow((Training - Testing), 2);
            }

            return Math.Sqrt(distance);
        }

        public List<LineDistance> GetNeighbors(List<List<string>> TrainingLines, List<string> TestLines, int k)
        {
            List<LineDistance> Distance = new List<LineDistance>();
            for (int x = 0; x < TrainingLines.Count; x++)
            {
                double dist = GetEuclideanDistance(TestLines, TrainingLines[x]);
                Distance.Add(new LineDistance(TrainingLines[x].Last(), dist));
            }

            List<LineDistance> neighbors = Distance.OrderBy(item => item.Distance).Take(k).ToList();

            return neighbors;
        }

        public void getResponses(List<LineDistance> neighbors)
        {
            Dictionary<string, int> classVotes = new Dictionary<string, int>();
            for (int x = 0; x < neighbors.Count; x++)
            {
                string response = neighbors[x].Label;
                if(classVotes.ContainsKey(response))
                {
                    classVotes[response]++;
                }
                else
                {
                    classVotes.Add(response, 1);
                }
            }

            string test = classVotes.OrderBy(n => n.Value).ToList().Last().Key;
        }
    }
}
