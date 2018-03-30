﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning_Vector_Quantization
{
    public struct Distance
    {
        public List<string> codebook;
        public double dist;

        public Distance(List<string> a, double b)
        {
            codebook = a;
            dist = b;
        }
    }

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
        public void RunLVQ(List<List<Neuronio>> neurons, List<List<string>> dataset, float learningRate) 
        {
            int StartIndex = 0;
            List<List<string>> trainingSet, testingSet;
            while (StartIndex < dataset.Count) 
            {
                StartIndex = GetDatasets(StartIndex, dataset, out testingSet, out trainingSet);

                // Itera no dataset pegando cada linha
                foreach (var line in trainingSet)
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

                    // Pega o neuronio com a menor distancia para a linha de entrada
                    NewDistance closest = distances.OrderBy(item => item.distance).First();

                    // Se esse neuronio nao tiver classe coloca a classe da linha de entrada
                    if (string.IsNullOrEmpty(closest.neuron.currentClass))
                        neurons[closest.neuron.row][closest.neuron.column].currentClass = closest.datasetRow.Last();

                    // Atualiza os pesos
                    // TODO: Implementar metodo de atualizacao
                }
            }
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
            //FullDataset = FullDataset.OrderBy(n => rnd.Next()).ToList();

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

        /// <summary>
        /// Encontra o que mais se assimila ao teste
        /// </summary>
        /// <param name="Dataset"></param>
        /// <param name="testRow"></param>
        /// <returns></returns>
        public List<string> GetBestMatchUnit(List<List<string>> Dataset, List<string> testRow)
        {
            List<Distance> distances = new List<Distance>();
            for (int i = 0; i < Dataset.Count; i++)
            {
                double dist = GetEuclideanDistance(Dataset[i], testRow);
                distances.Add(new Distance(Dataset[i], dist));
            }

            distances = distances.OrderBy(n => n.dist).ToList();

            return distances[0].codebook;
        }
    }
}
