using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning_Vector_Quantization
{
    //public struct Distance
    //{
    //    public List<string> codebook;
    //    public double dist;

    //    public Distance(List<string> a, double b)
    //    {
    //        codebook = a;
    //        dist = b;
    //    }
    //}

    public struct NewDistance {
        public Neuronio neuron;
        public double distance;
        public List<string> datasetRow;

        public NewDistance(Neuronio neuron, double distance, List<string> datasetRow) 
        {
            this.neuron = neuron;
            this.distance = distance;
            this.datasetRow = datasetRow;
        }
    }

    class LVQ
    {
        public void RunLVQ(List<List<Neuronio>> neurons, List<List<string>> trainingSet, float radius, double dp, double learningRate) 
        {
            //int StartIndex = 0;
            //List<List<string>> trainingSet, testingSet;
            //while (StartIndex < dataset.Count) //REVER, acredito q esteja no local errado
            //{
            //StartIndex = GetDatasets(StartIndex, dataset, out testingSet, out trainingSet);

            // Itera no dataset pegando cada linha
                foreach (List<string> line in trainingSet)
                {
                    // Pega o neuronio com a menor distancia para a linha de entrada / BMU
                    NewDistance closest = BMU(neurons, line);

                    // Atualiza Neuronio
                    updateWeight(closest.datasetRow, closest.neuron, ref neurons, radius, dp, learningRate);

                    // Atualiza os pesos
                    // TODO: Implementar metodo de atualizacao
                }
            //}
        }

        /// <summary>
        /// Calcula a distancia entre os neuronios e encontra o que mais se aproxima
        /// </summary>
        /// <param name="neurons"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public NewDistance BMU(List<List<Neuronio>> neurons, List<string> line)
        {
            List<NewDistance> distances = new List<NewDistance>();
            // Para cada linha itera nos neuronios
            foreach (var neuronLine in neurons)
            {
                // Para cada neuronio pega a distancia entre os pesos e a linha do dataset
                foreach (var neuron in neuronLine)
                {
                    distances.Add(new NewDistance(neuron, NewGetEuclideanDistance(line, neuron), line));
                }
            }

            // Pega o neuronio com a menor distancia para a linha de entrada / BMU
            return distances.OrderBy(item => item.distance).First();
        }

        void updateWeight(List<string> DatasetRow, Neuronio winner, ref List<List<Neuronio>> neurons, float radius, double dp, double learningRate)
        {
            if (string.IsNullOrEmpty(winner.currentClass)) //se nao possuir classe, atribui classe do row
                neurons[winner.row][winner.column].currentClass = DatasetRow.Last();

            for (int i = 0; i < neurons.Count; i++) //Percorre linha da matriz
            {
                for (int j = 0; j < neurons[i].Count; j++) //Percorre coluna da matriz
                {
                    double dist = getNeuronDistance(winner, neurons[i][j]); //Calcula a distancia para o neuronio i,j

                    if (dist <= radius) //Se a distancia eh menor q o raio, deve-se atualizar os pesos
                    {
                        //if (string.IsNullOrEmpty(neurons[i][j].currentClass)) //se nao possuir classe, atribui classe do row
                        //    neurons[neurons[i][j].row][neurons[i][j].column].currentClass = DatasetRow.Last();

                        double gaussian = gaussianMath(dist, dp);
                        for (int w = 0; w < neurons[i][j].pesos.Count; w++)
                        {
                            double lastWeight = neurons[i][j].pesos[w];
                            double X = double.Parse(DatasetRow[w]);
                            double newWeight = 0;

                            if (winner.currentClass == DatasetRow.Last()) //Eh a mesma classe que a escolhida
                            {
                                newWeight = lastWeight + learningRate * gaussian * (X - lastWeight);
                            }
                            else //Classe diferente, formula diferente
                            {
                                newWeight = lastWeight - learningRate * gaussian * (X - lastWeight);
                            }

                            if (double.IsInfinity(newWeight) || double.IsNaN(newWeight))
                            {
                                newWeight = 0;
                            }
                            else if (newWeight > 1) newWeight = 1;
                            else if (newWeight < 0) newWeight = -1;
                            neurons[i][j].pesos[w] = newWeight;
                        }
                    }
                }
            }
        }

        double getNeuronDistance(Neuronio winner, Neuronio neighbor)
        {
            return Math.Sqrt(Math.Pow(winner.row - neighbor.row, 2) + Math.Pow(winner.column - neighbor.column, 2));
        }

        /// <summary>
        /// Calcula a 
        /// </summary>
        /// <param name="dist"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        double gaussianMath(double dist, double dp)
        {
            double d = Math.Pow(dist, 2);
            double newDP = 2 * Math.Pow(dp, 2);
            return (d == 0) ? 1 : Math.Pow(Math.E, -d / newDP);
        }

        public int GetDatasets(int StartIndex, List<List<string>> Dataset, out List<List<string>> testingSet, out List<List<String>> trainingSet) {
            testingSet = new List<List<string>>();
            trainingSet = new List<List<string>>();

            // Guarda o numero do ultimo index a ser pego
            int EndIndex = (Dataset.Count / 10) + StartIndex;
            EndIndex = EndIndex > Dataset.Count ? Dataset.Count : EndIndex;

            // Guarda o dataset de teste.
            testingSet = Dataset.GetRange(StartIndex, EndIndex - StartIndex);

            // Guarda o dataset de treino
            if (StartIndex > 0) { trainingSet = Dataset.GetRange(0, StartIndex - 1); }
            if (EndIndex < Dataset.Count) { trainingSet.AddRange(Dataset.GetRange((EndIndex + 1), (Dataset.Count - (EndIndex + 1)))); }

            return StartIndex = EndIndex + 1;
        }

        /// <summary>
        /// Ira carregar um CSV e transforma-lo em uma lista de colunas contida em uma lista de linhas
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<List<string>> LoadCSVData(string path)
        {
            Console.WriteLine("Starting LVQ from " + path);

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
        /// Calcula a distancia euclidiana entre as colunas de cada linha
        /// </summary>
        /// <param name="trainingLine"></param>
        /// <param name="testingLine"></param>
        /// <returns></returns>
        public double GetEuclideanDistance(List<string> trainingLine, List<string> testingLine)
        {
            double distance = 0;
            for (int i = 0; i < trainingLine.Count - 1; i++)
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

        // Verificar se precisa manter essa versao da distancia euclidiana.
        public double NewGetEuclideanDistance(List<string> trainingLine, Neuronio neuron)
        {
            double distance = 0;
            for (int i = 0; i < trainingLine.Count - 1; i++)
            {
                double Training = 0;
                if (!double.TryParse(trainingLine[i], out Training))
                    Console.WriteLine("ERROR: Unable to cast string to double.");
                
                distance += Math.Pow((Training - neuron.pesos[i]), 2);
            }

            return Math.Sqrt(distance);
        }
    }
}
