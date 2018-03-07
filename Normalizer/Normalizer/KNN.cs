using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizer
{
    class KNN
    {
        public void GetSetsForColumn(string FilePath, float TrainingPercentage, out List<List<string>> TrainingSet, out List<List<string>> TestingSet)
        {
            TrainingSet = new List<List<string>>();
            TestingSet = new List<List<string>>();

            List<List<string>> FullDataset = PrepareDataset(FilePath);
            int TrainingIndex = (int) Math.Floor(FullDataset.Count() * TrainingPercentage);

            for ( int i = 0; i < FullDataset.Count() - 1; i++)
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
