using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizer
{
    class KNN
    {
        public void GetNeighbours(List<List<string>> TrainingSet, List<List<string>> TestingSet, int K) 
        {
            List<LineDistance> Neighbours = new List<LineDistance>();
            List<string> labels = new List<string>();

            foreach (var TestLine in TestingSet)
            {
                for (int i = 0; i < TrainingSet.Count; i++)
                {
                    double EuclideanDistance = GetEuclideanDistance(TrainingSet[i], TestLine);
                    Neighbours.Add(new LineDistance(TrainingSet[i].Last(), EuclideanDistance));
                }

                Neighbours = Neighbours.OrderBy(item => item.Distance).Take(K).ToList();

                // Extrair para outra classe, chosen label é a classe do item em questao.
                Dictionary<string, int> ClassVotes = new Dictionary<string, int>();
                foreach (var Item in Neighbours)
                {
                    if (ClassVotes.ContainsKey(Item.Label)) { ClassVotes[Item.Label]++; }
                    else { ClassVotes.Add(Item.Label, 1); }
                }

                labels.Add(ClassVotes.OrderBy(Item => Item.Value).Last().Key);
            }

            int corrects = 0;
            for (int i = 0; i < TestingSet.Count; i++)
            {
                if (TestingSet[i].Last() == labels[i])
                    corrects++;

            }

            Console.WriteLine("% certa = " + ((float)corrects / (float)TestingSet.Count) * 100 + "%");
            Console.WriteLine("GetNeighbours");
        }

        private double GetEuclideanDistance(List<string> TrainingSet, List<string> TestingSet) 
        {
            if (TrainingSet.Count != TestingSet.Count)
            {
                Console.WriteLine("ERROR: Sets do not have same size.");
            }

            double distance = 0;
            for (int i = 0; i < TestingSet.Count - 1; i++)
            {
                double Testing = 0, Training = 0;
                if (!double.TryParse(TestingSet[i], out Testing) || !double.TryParse(TrainingSet[i], out Training)) 
                {
                    Console.WriteLine("ERROR: Unable to cast string to double.");
                }

                distance += Math.Pow((Training - Testing), 2);
            }

            return Math.Sqrt(distance);
        }

        public void GetSetsForColumn(string FilePath, float TrainingPercentage, out List<List<string>> TrainingSet, out List<List<string>> TestingSet)
        {
            TrainingSet = new List<List<string>>();
            TestingSet = new List<List<string>>();

            List<List<string>> FullDataset = PrepareDataset(FilePath);
            int TrainingIndex = (int) Math.Floor(FullDataset.Count * TrainingPercentage);

            for ( int i = 0; i < FullDataset.Count - 1; i++)
            {
                if (i < TrainingIndex)
                    TrainingSet.Add(FullDataset[i]);
                else
                    TestingSet.Add(FullDataset[i]);
            }
        }

        private List<List<string>> PrepareDataset(string FilePath)
        {
            Console.WriteLine("Starting KNN from " + FilePath);

            List<List<string>> FullDataset = new List<List<string>>();
            string FileContents = FileSystem.GetFileContents(FilePath);
            List<string> lines = FileContents.Split(Environment.NewLine.ToCharArray()).ToList();
            lines.RemoveAll(item => item == "");

            for (int i = 0; i < lines.Count(); i++)
            {
                List<string> columns = new List<string>();
                columns.AddRange(lines[i].Split(','));
                FullDataset.Add(columns);
            }
            var rnd = new Random();

            FullDataset = FullDataset.OrderBy(n => rnd.Next()).ToList();
            return FullDataset;
        }
    }
}
