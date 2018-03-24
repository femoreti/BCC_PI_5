using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning_Vector_Quantization
{
    public struct Neuronio
    {
        public string currentClass;
        public double peso;
    }

    class Program
    {
        static void Main(string[] args)
        {
            LVQ _lvq = new LVQ();
            List<List<string>> Dataset = _lvq.LoadCSVData(@"../../Raw Data/Normalized/iris-Normalized.csv");
            List<List<Neuronio>> Neuronios = new List<List<Neuronio>>();
            
            int sizeOfNetwork = GetSizeOfNetwork(GetTotalDistinctClasses(Dataset)); //Define o tamanho N da rede neural

            Random rnd = new Random();
            for (int i = 0; i < sizeOfNetwork; i++) //Inicia uma nova matriz de Neuronios NxN
            {
                Neuronios.Add(new List<Neuronio>());
                for (int j = 0; j < sizeOfNetwork; j++)
                {
                    Neuronio neuronio = new Neuronio(); //Cria um novo neuronio
                    neuronio.peso = rnd.NextDouble(); //Define um peso inicial aleatorio entre 0 e 1
                    Neuronios[i].Add(neuronio);
                }
            }

            for (int i = 1; i <= 4; i++) //Para os 4 tipos de R
            {
                //inicializa as Constantes
                float radius = GetRadius(i, sizeOfNetwork);
                float dp = radius;
                double T1 = (double)1000 / Math.Log10(dp);
                float n0 = 0.1f;
                int t2 = 1000;
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
