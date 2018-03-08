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
            //Dictionary<string, double> NearestNeighbours = new Dictionary<string, double>();
            List<LineDistance> Neighbours = new List<LineDistance>();

            int TestKey = 0; // Substituir pela classe depois

            foreach (var TestLine in TestingSet)
            {
                foreach (var TrainLine in TrainingSet)
                {
                    double EuclideanDistance = GetEuclideanDistance(TrainLine, TestLine);
                    Neighbours.Add(new LineDistance(TestKey.ToString(), EuclideanDistance));
                    TestKey++;
                }

                Neighbours = Neighbours.OrderBy(item => item.Distance).Take(K).ToList();

                // Extrair para outra classe, chosen label é a classe do item em questao.
                string ChosenLabel = "";
                int LastCount = 0;
                foreach (var Item in Neighbours)
                {
                    int CurrentCount = Neighbours.Count(i => i.Label == Item.Label); //For no neighbors de novo
                    if (CurrentCount <= LastCount) { continue; }

                    ChosenLabel = Item.Label;
                    LastCount = CurrentCount;
                }
            }
        }

        private double GetEuclideanDistance(List<string> TrainingSet, List<string> TestingSet) 
        {
            if (TrainingSet.Count != TestingSet.Count)
            {
                Console.WriteLine("ERROR: Sets do not have same size.");
                //Environment.Exit(-1);
            }
            double distance = 0;
            for (int i = 0; i < TestingSet.Count; i++)
            {
                double Testing = 0, Training = 0;
                if (!double.TryParse(TestingSet[i], out Testing) || !double.TryParse(TrainingSet[i], out Training)) 
                {
                    Console.WriteLine("ERROR: Unable to cast string to double.");
                    //Environment.Exit(-1);
                }

                distance += Math.Pow((Testing - Training), 2);
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
            List<List<string>> FullDataset = new List<List<string>>();
            string FileContents = FileSystem.GetFileContents(FilePath);
            List<string> SplitLines = FileContents.Split(Environment.NewLine.ToCharArray()).ToList();
            SplitLines.RemoveAll(item => item == "");

            for (int i = 0; i < SplitLines.Count(); i++)
            {
                List<string> NewLine = new List<string>();
                NewLine.AddRange(SplitLines[i].Split(','));
                FullDataset.Add(NewLine);
            }

            return FullDataset;
        }
    }
}
