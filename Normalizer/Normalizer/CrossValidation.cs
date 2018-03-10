    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizer
{
    class CrossValidation
    {
        public static Dictionary<string, int> binaryConfusionMatrix = new Dictionary<string, int>();

        /// <summary>
        /// Calcula o Erro amostral
        /// </summary>
        /// <param name="TestSet"></param>
        /// <param name="predictions"></param>
        /// <returns></returns>
        public static float erroAmostral(List<List<string>> TestSet, List<string> predictions)
        {
            float erro = 0;
            for (int i = 0; i < TestSet.Count; i++)
            {
                erro += deltaDeKronecker(TestSet[i].Last(), predictions[i]);
            }

            return erro / (float)TestSet.Count;
        }

        private static int deltaDeKronecker(string a, string b)
        {
            return (a == b) ? 0 : 1;
        }

        public static float erroDeValidacaoCruzada(List<float> listOfErroAmostral)
        {
            // Calcula a validacao cruzada
            float crossValidation = 0;
            foreach (float f in listOfErroAmostral)
                crossValidation += f;

            crossValidation /= listOfErroAmostral.Count;
            Console.WriteLine("erro de CROSS VALIDATION: " + (crossValidation * 100).ToString() + "%");

            return crossValidation;
        }

        public static void prepareConfusionMatrix(List<List<string>> Dataset, List<List<string>> TestSet, List<string> Predictions)
        {
            List<string> DistinctClasses = new List<string>();
            foreach (var Item in Dataset)
            {
                DistinctClasses.Add(Item.Last());
            }

            DistinctClasses = DistinctClasses.Distinct().ToList();

            if(DistinctClasses.Count > 2) // Multiclasses
            {

            }
            else //Binario
            {
                for (int i = 0; i < TestSet.Count; i++)
                {
                    string testClass = TestSet[i].Last();

                    if(testClass == "1" && Predictions[i] == "1")
                    {
                        //TP
                        if (binaryConfusionMatrix.ContainsKey("TP"))
                            binaryConfusionMatrix["TP"]++;
                        else
                            binaryConfusionMatrix.Add("TP", 1);
                    }
                    else if(testClass == "0" && Predictions[i] == "1")
                    {
                        //FP
                        if (binaryConfusionMatrix.ContainsKey("FP"))
                            binaryConfusionMatrix["FP"]++;
                        else
                            binaryConfusionMatrix.Add("FP", 1);
                    }
                    else if (testClass == "1" && Predictions[i] == "0")
                    {
                        //FN
                        if (binaryConfusionMatrix.ContainsKey("FN"))
                            binaryConfusionMatrix["FN"]++;
                        else
                            binaryConfusionMatrix.Add("FN", 1);
                    }
                    else if (testClass == "0" && Predictions[i] == "0")
                    {
                        //TN
                        if (binaryConfusionMatrix.ContainsKey("TN"))
                            binaryConfusionMatrix["TN"]++;
                        else
                            binaryConfusionMatrix.Add("TN", 1);
                    }
                }
            }
        }

        public static string GeraMatrizBinaria()
        {
            float TP = 0, FP = 0, FN = 0, TN = 0;
            TP = (binaryConfusionMatrix.ContainsKey("TP")) ? binaryConfusionMatrix["TP"] : 0;
            FP = (binaryConfusionMatrix.ContainsKey("FP")) ? binaryConfusionMatrix["FP"] : 0;
            FN = (binaryConfusionMatrix.ContainsKey("FN")) ? binaryConfusionMatrix["FN"] : 0;
            TN = (binaryConfusionMatrix.ContainsKey("TN")) ? binaryConfusionMatrix["TN"] : 0;

            string confusionOUT = "True Positive\t" + TP.ToString() +
                    "\nFalse Positive\t" + FP.ToString() +
                    "\nFalse Negative\t" + FN.ToString() +
                    "\nTrue Negative\t" + TN.ToString() +
                    "\nSensibilidade\t" + (TP/(TP+FN)).ToString() + 
                    "\nEspecificidade\t" + (TN/(TN+FP)).ToString() +
                    "\nPrecisão\t" + (TP/(TP+FP)).ToString() +
                    "\nRevocação\t" + (TP/(TP+FN)).ToString();

            //Console.WriteLine(confusionOUT);
            binaryConfusionMatrix.Clear(); //Limpa todos resultados de classe binaria

            return confusionOUT;
        }
    }
}
