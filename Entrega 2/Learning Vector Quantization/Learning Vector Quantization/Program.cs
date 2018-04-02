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
        public int color, totalColorsSet;
    }

    class Program
    {
        static void Main(string[] args)
        {
            //ExecuteLVQ(@"../../Raw Data/Normalized/abalone-Normalized.csv");
            //ExecuteLVQ(@"../../Raw Data/Normalized/adult-Normalized.csv");
            ExecuteLVQ(@"../../Raw Data/Normalized/iris-Normalized.csv");
            //ExecuteLVQ(@"../../Raw Data/Normalized/wdbc-Normalized.csv");
            //ExecuteLVQ(@"../../Raw Data/Normalized/wine-Normalized.csv");
            //ExecuteLVQ(@"../../Raw Data/Normalized/winequality-red-Normalized.csv");
        }

        static void ExecuteLVQ(string path)
        {
            string fileName = path.Split('/').Last().Split('-')[0];

            LVQ _lvq = new LVQ();
            List<List<string>> Dataset = _lvq.LoadCSVData(path);
            List<List<Neuronio>> Neuronios = new List<List<Neuronio>>();

            int sizeOfNetwork = GetSizeOfNetwork(GetTotalDistinctClasses(Dataset)); //Define o tamanho N da rede neural
            int totalEntries = Dataset[0].Count - 1;

            string CrossValidationErrors = string.Empty;
            for (int i = 1; i <= 4; i++) //Executa os 4 tipos de R
            {
                Console.Write("Iniciando, i = {0}\n", i.ToString());

                Random rnd = new Random();
                Neuronios.Clear();
                //Inicia uma nova matriz de Neuronios NxN
                for (int k = 0; k < sizeOfNetwork; k++) //Linhas
                {
                    Neuronios.Add(new List<Neuronio>());
                    for (int j = 0; j < sizeOfNetwork; j++) //Colunas
                    {
                        Neuronio neuron = new Neuronio();
                        neuron.pesos = new List<double>();
                        neuron.currentClass = string.Empty;
                        neuron.row = k;
                        neuron.column = j;
                        neuron.totalColorsSet = 1;
                        for (int l = 0; l < totalEntries; l++)
                            neuron.pesos.Add(rnd.NextDouble());

                        Neuronios[k].Add(neuron);
                    }
                }

                int StartIndex = 0;
                List<List<string>> trainingSet, testingSet;
                List<float> listOfErroAmostral = new List<float>();
                int etapa = 1;
                while (StartIndex < Dataset.Count) //REVER, acredito q esteja no local errado
                {
                    StartIndex = _lvq.GetDatasets(StartIndex, Dataset, out testingSet, out trainingSet);

                    //inicializa as Constantes
                    float radius = GetRadius(i, sizeOfNetwork);
                    float initial_dp = radius;
                    double t1 = (Math.Log10(initial_dp) != 0) ? (double)1000 / Math.Log10(initial_dp) : 1;
                    float n0 = 0.1f;
                    int t2 = 1000;

                    for (int n = 0; n < 500; n++) //numero de iteraçoes para aprendizado
                    {
                        Console.Write("\retapa {0} Iteracao {1} / {2}... {3}%", etapa, n + 1, 500, Math.Round(((float)(n + 1) / (float)500) * 100));

                        double learningRate = n0 * Math.Pow(Math.E, ((double)-n / (double)t2));
                        if (learningRate < 0.01f)
                            learningRate = 0.01f;
                        double dp = initial_dp * Math.Pow(Math.E, ((double)-n / t1));

                        _lvq.RunLVQ(Neuronios, Dataset, radius, dp, learningRate);
                    }


                    List<string> predictions = new List<string>();
                    for (int x = 0; x < testingSet.Count; x++)
                    {
                        string result = string.Empty;

                        NewDistance prediction = _lvq.BMU(Neuronios, testingSet[x]);

                        predictions.Add(prediction.neuron.currentClass);
                    }

                    // Guarda erro amostral da linha
                    listOfErroAmostral.Add(CrossValidation.erroAmostral(testingSet, predictions));
                    CrossValidation.prepareConfusionMatrix(Dataset, testingSet, predictions);

                    if (etapa == 10)
                    {
                        //Console.WriteLine("Para R = {0}\n", radius);
                        //string heatmap = string.Empty;
                        ////DEBUG
                        //heatmap += "HEATMAP\n";
                        //for (int q = 0; q < sizeOfNetwork; q++) //Linhas
                        //{
                        //    for (int j = 0; j < sizeOfNetwork; j++) //Colunas
                        //    {
                        //        if (!string.IsNullOrEmpty(Neuronios[q][j].currentClass))
                        //            heatmap += ("" + Neuronios[q][j].currentClass);
                        //        else
                        //            heatmap += (" ");
                        //        //for (int k = 0; k < totalEntries; k++)
                        //        //    Console.Write(Neuronios[q][j].pesos[k] + " ");
                        //        heatmap += ("\t");
                        //    }
                        //    heatmap += ("\n");
                        //}
                        //heatmap += ("\n");
                        //Console.WriteLine(heatmap);
                        FileSystem.SaveFileContents(GenerateHeatmap(Dataset, Neuronios, _lvq), @"../../Raw Data/Normalized/output/" + fileName + "/", fileName + "-Heatmap-" + i.ToString() + ".txt");
                    }
                    etapa++;
                }

                if (CrossValidation.binaryConfusionMatrix.Count > 0) //Se for matriz binaria ira salvar os dados aqui
                {
                    FileSystem.SaveFileContents(CrossValidation.GeraMatrizBinaria(), @"../../Raw Data/Normalized/output/" + fileName + "/", fileName + "-Matriz-Binaria-Confusao-" + i.ToString() + ".txt");
                }
                if (CrossValidation.multiClassConfusionMatrix.Count > 0) //Se for matriz multi-classe irá salvar aqui
                {
                    FileSystem.SaveFileContents(CrossValidation.GeraMatrizMultiClasse(), @"../../Raw Data/Normalized/output/" + fileName + "/", fileName + "-Matriz-MultiClasse-Confusao-" + i.ToString() + ".txt");
                }

                CrossValidationErrors += "Erro de validação cruzada para R = " + GetRadius(i, sizeOfNetwork).ToString() + "\t" + (CrossValidation.erroDeValidacaoCruzada(listOfErroAmostral) * 100) + "%\n";
            }

            FileSystem.SaveFileContents(CrossValidationErrors, @"../../Raw Data/Normalized/output/" + fileName + "/", fileName + "-CROSSVALIDATION REPORT.txt");

            Console.ReadLine();
        }

        static string GenerateHeatmap(List<List<string>> Dataset, List<List<Neuronio>> neuronios, LVQ lvq)
        {
            for (int p = 0; p < Dataset.Count; p++)
            {
                Neuronio bmu = lvq.BMU(neuronios, Dataset[p]).neuron;
                int line = bmu.row;
                int column = bmu.column;
                neuronios[line][column].color += int.Parse(Dataset[p].Last()) + 1; //incrementa a cor de acordo com a classe
                neuronios[line][column].totalColorsSet++; //soma quantas vezes foi modificado
            }

            string heatmap = string.Empty;
            heatmap += "HEATMAP\n";
            for (int q = 0; q < neuronios.Count; q++) //Linhas
            {
                for (int j = 0; j < neuronios[q].Count; j++) //Colunas
                {
                    heatmap += ("" + (float)neuronios[q][j].color / (float)neuronios[q][j].totalColorsSet); //aqui faz o valor ser proximo de qual classe deve pertencer
                    heatmap += ("\t");
                }
                heatmap += ("\n");
            }
            heatmap += ("\n");

            heatmap = heatmap.Replace('.', ',');

            return heatmap;
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
