using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning_Vector_Quantization
{
    public class Neuronio
    {
        public string currentClass;
        public List<double> pesos;
        public int row, column;
    }

    class Program
    {
        static void Main(string[] args)
        {
            LVQ _lvq = new LVQ();
            List<List<string>> Dataset = _lvq.LoadCSVData(@"../../Raw Data/Normalized/iris-Normalized.csv");
            List<List<Neuronio>> Neuronios = new List<List<Neuronio>>();
            
            int sizeOfNetwork = GetSizeOfNetwork(GetTotalDistinctClasses(Dataset)); //Define o tamanho N da rede neural
            int totalEntries = Dataset[0].Count - 1;

            Random rnd = new Random();
            //Inicia uma nova matriz de Neuronios NxN
            for (int i = 0; i < sizeOfNetwork; i++) //Linhas
            {
                Neuronios.Add(new List<Neuronio>());
                for (int j = 0; j < sizeOfNetwork; j++) //Colunas
                {
                    Neuronio neuron = new Neuronio();
                    neuron.pesos = new List<double>();
                    neuron.row = i;
                    neuron.column = j;
                    for (int k = 0; k < totalEntries; k++) 
                        neuron.pesos.Add(rnd.NextDouble());

                    Neuronios[i].Add(neuron);
                }
            }

            

            for (int i = 1; i <= 1; i++) //Executa os 4 tipos de R
            {
                //inicializa as Constantes
                float radius = GetRadius(i, sizeOfNetwork);
                float initial_dp = radius;
                double t1 = (Math.Log10(initial_dp) != 0) ? (double)1000 / Math.Log10(initial_dp) : 1;
                float n0 = 0.1f;
                int t2 = 1000;

                for (int n = 0; n < 500; n++) //numero de iteraçoes para aprendizado
                {
                    Console.Write("\rIteracao {0} / {1}... {2}%", n, 500, Math.Round(((float)(n + 1) / (float)500) * 100));

                    double learningRate = n0 * Math.Pow(Math.E, ((double)-n / (double)t2));
                    double dp = initial_dp * Math.Pow(Math.E, ((double)-n / t1));

                    _lvq.RunLVQ(Neuronios, Dataset, radius, dp, learningRate);
                }

                Console.WriteLine("Finish i = " + i.ToString() + "\n");
                //DEBUG
                for (int q = 0; q < sizeOfNetwork; q++) //Linhas
                {
                    Console.Write(q + " | ");
                    for (int j = 0; j < sizeOfNetwork; j++) //Colunas
                    {
                        Console.Write("[ ");
                        for (int k = 0; k < totalEntries; k++)
                            Console.Write(Neuronios[q][j].pesos[k] + " ");
                        Console.Write("] ");
                    }
                    Console.Write("\n");
                }
            }

            Console.ReadLine();
        }

        static int GetTotalDistinctClasses(List<List<string>> Dataset)
        {
            List<string> DistinctClasses = new List<string>();
            foreach (var Item in Dataset)
            {
                DistinctClasses.Add(Item.Last());
            }

            DistinctClasses = DistinctClasses.Distinct().ToList();
            return DistinctClasses.Count;
        }

        /// <summary>
        /// Pega um inteiro necessario para criar uma matriz NxN
        /// </summary>
        /// <param name="numberOfClass"></param>
        /// <returns></returns>
        static int GetSizeOfNetwork(int numberOfClass)
        {
            int minSize = numberOfClass * 10;

            int i = numberOfClass;
            while (Math.Pow(i, 2) < minSize)
                i++;

            return i;
        }

        /// <summary>
        /// Pega todas as possiveis versoes do algoritmo
        /// </summary>
        /// <param name="i"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        static float GetRadius(int i, int N)
        {
            switch (i)
            {
                case 1:
                    return 1;
                case 2:
                    return 3;
                case 3:
                    return N / 2;
                case 4:
                    return N;
            }

            return 0;
        }
    }
}
