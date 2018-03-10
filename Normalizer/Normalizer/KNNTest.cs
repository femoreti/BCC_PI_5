using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizer
{
    class KNNTest
    {
        /// <summary>
        /// Ira carregar um CSV e transformalo em uma lista de colunas contida em uma lista de linhas
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<List<string>> LoadCSVData(string path)
        {
            Console.WriteLine("Starting KNN from " + path);

            List<List<string>> FullDataset = new List<List<string>>();
            string FileContents = FileSystem.GetFileContents(path);

            // Separa a string na quebra de linha e remove entradas vazias
            List<string> SplitLines = FileContents.Split(Environment.NewLine.ToCharArray()).ToList();
            SplitLines.RemoveAll(item => item == "");

            // Adciona as linhas para o dataset
            for (int i = 0; i < SplitLines.Count(); i++)
            {
                List<string> NewLine = new List<string>();
                NewLine.AddRange(SplitLines[i].Split(','));
                FullDataset.Add(NewLine);
            }

            // Embaralha o dataset
            var rnd = new Random();
            FullDataset = FullDataset.OrderBy(n => rnd.Next()).ToList();

            return FullDataset;
        }

        /// <summary>
        /// Ira preparar o dataset de treino e de testes de acordo com o KFold
        /// </summary>
        /// <param name="path"></param>
        /// <param name="percentage"></param>
        /// <param name="TestList"></param>
        /// <param name="TrainingList"></param>
        public int PrepareDataset(string path, int StartIndex, int KFold, ref List<List<string>> DataSet, out List<List<string>> TestList, out List<List<string>> TrainingList)
        {
            TestList = new List<List<string>>();
            TrainingList = new List<List<string>>();

            // Guarda o numero do ultimo index a ser pego
            int EndIndex = (DataSet.Count / KFold) + StartIndex;
            EndIndex = EndIndex > DataSet.Count ? DataSet.Count : EndIndex;

            // Guarda o dataset de teste.
            TestList = DataSet.GetRange(StartIndex, EndIndex - StartIndex);

            // Guarda o dataset de treino
            if (StartIndex > 0) { TrainingList = DataSet.GetRange(0, StartIndex - 1); }
            if (EndIndex < DataSet.Count) { TrainingList.AddRange(DataSet.GetRange((EndIndex + 1), (DataSet.Count - (EndIndex + 1)))); }

            return StartIndex = EndIndex + 1;
        }

        /// <summary>
        /// Calcula a distancia euclidiana entre as colunas de cada linha
        /// </summary>
        /// <param name="trainingLine"></param>
        /// <param name="testingLine"></param>
        /// <returns></returns>
        double GetEuclideanDistance(List<string> trainingLine, List<string> testingLine)
        {
            double distance = 0;
            for (int i = 0; i < trainingLine.Count-1; i++)
            {
                double Testing = 0, Training = 0;
                if (!double.TryParse(trainingLine[i], out Training) || !double.TryParse(testingLine[i], out Testing))
                {
                    Console.WriteLine("ERROR: Unable to cast string to double.");
                }
                distance += Math.Pow((Training - Testing), 2);
            }

            return Math.Sqrt(distance);
        }
        
        /// <summary>
        /// apos calcular a distancia euclidiana entre todas as linhas, escolhe as K mais proximas e considera como vizinho
        /// </summary>
        /// <param name="TrainingLines"></param>
        /// <param name="TestLines"></param>
        /// <param name="k"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Calcula a label a partir da predicao baseada nas outras labels do set
        /// </summary>
        /// <returns>The responses.</returns>
        /// <param name="neighbors">Neighbors.</param>
        public string getResponses(List<LineDistance> neighbors)
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

            string predictedKey = classVotes.OrderBy(n => n.Value).ToList().Last().Key;

            return predictedKey;
        }

        /// <summary>
        /// Calcula a precisa da predicao da label
        /// </summary>
        /// <returns>The accuracy.</returns>
        /// <param name="testSet">Test set.</param>
        /// <param name="predictions">Predictions.</param>
        public float getAccuracy(List<List<string>> testSet, List<string> predictions)
        {
            int correct = 0;

            for (int x = 0; x < testSet.Count; x++)
            {
                if (testSet[x].Last() == predictions[x])
                    correct++;
            }

            return ((float)correct / (float)testSet.Count) * 100f;
        }
    }
}
