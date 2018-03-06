using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizer
{
    class KNN
    {
        public void GetSetsForColumn(string FilePath, float TrainingPercentage, int ColumnIndex, out List<double> TrainingSet, out List<double> TestingSet)
        {
            TrainingSet = new List<double>();
            TestingSet = new List<double>();

            List<List<string>> FullDataset = PrepareDataset(FilePath);
            int TrainingIndex = (int) Math.Floor(FullDataset[ColumnIndex].Count() * TrainingPercentage);

            for ( int i = 0; i < FullDataset[ColumnIndex].Count() - 1; i++)
            {
                if (i < TrainingIndex)
                    TrainingSet.Add(Convert.ToDouble(FullDataset[ColumnIndex][i]));
                else
                    TestingSet.Add(Convert.ToDouble(FullDataset[ColumnIndex][i]));
            }
        }

        private List<List<string>> PrepareDataset(string FilePath)
        {
            List<List<string>> FullDataset = new List<List<string>>();
            string FileContents = FileSystem.GetFileContents(FilePath);
            List<string> SplitLines = FileContents.Split(Environment.NewLine.ToCharArray()).ToList();

            for (int i = 0; i < SplitLines.Count(); i++)
                if (SplitLines[i].Split(',').Contains("")) { SplitLines.RemoveAt(i); }

            foreach (var Line in SplitLines)
            {
                string[] SplitColumns = Line.Split(',');
                for (int i = 0; i < SplitColumns.Count(); i++)
                {
                    if (FullDataset.Count() < SplitColumns.Count())
                        FullDataset.Add(new List<string>() { SplitColumns[i] });
                    else
                        FullDataset[i].Add(SplitColumns[i]);
                }
            }

            return FullDataset;
        }
    }
}
