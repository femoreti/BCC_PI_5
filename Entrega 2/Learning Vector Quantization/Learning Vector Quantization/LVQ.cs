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

    class LVQ
    {
        /// <summary>
        /// Ira carregar um CSV e transformalo em uma lista de colunas contida em uma lista de linhas
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
