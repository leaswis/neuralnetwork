using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApplication19
{
    class Program
    {
        static Perceptron.myDelegate activate = new Perceptron.myDelegate(Perceptron.sigmod);

        public static int i = 0;

        public static string path = @"C:\Users\Laura\Desktop\YT_Data.csv";

        static void Main(string[] args)
        {
           
            
            List<KeyValuePair<double, double>> csv_results = new List<KeyValuePair<double, double>>();

            int n = CrossValidYT(csv_results);
            //int n = CrossValidXO(csv_results);
 
            var excellent = csv_results.Where(x => Math.Abs(x.Key / x.Value - 1) <= 0.1).Count();
            var passed = csv_results.Where(x => Math.Abs(x.Key / x.Value - 1) > 0.1 && Math.Abs(x.Key / x.Value - 1) <= 0.3).Count();
            var notbad = csv_results.Where(x => Math.Abs(x.Key / x.Value - 1) > 0.3 && Math.Abs(x.Key / x.Value - 1) <= 0.7).Count();
            var failed = csv_results.Where(x => Math.Abs(x.Key / x.Value - 1) > 0.7).Count();
            var net_quality = Math.Round((excellent+passed+notbad)/(double)csv_results.Count, 3)*100;

            Console.WriteLine("Sprawdzian kroswalidacji - wykonano: {0} z {1}", i, n);
            Console.WriteLine("Liczba zdanych sprawdzianów kroswalidacji z maks. odchyleniem 10% {0}", excellent);
            Console.WriteLine("Liczba zdanych sprawdzianów kroswalidacji  z maks. odchyleniem na poziomie 11% - 30% {0}", passed);
            Console.WriteLine("Liczba zdanych sprawdzianów kroswalidacji z maks. odchyleniem na poziomie 31% - 50% {0}", notbad);
            Console.WriteLine("Liczba niezdanych sprawdzianów kroswalidacji, odchylenie 49% - 100%  {0}", failed);
            Console.WriteLine("Sieć neuronowa ma jakość: {0}%", net_quality);
        

            Console.ReadLine();
        }


        #region helpers


        public static int CrossValidXO(List<KeyValuePair<double,double>> list_res)
        {
            var csv_list = CSVReadXO();
 

            foreach (var c in csv_list)
            {
                var nn = new NeuralNetwork(2, 1, activate);
                nn.HiddenLayerCreate(3);
                nn.HiddenLayerCreate(3);

                TrainingNetwork(ref nn, activate, csv_list.Where(x => x != c).ToList());

                i++;

                var result = nn.ForwardPropagation(c.input);

                list_res.Add(new KeyValuePair<double, double>(result[0, 0], c.output[0]));

            }

            return csv_list.Count();
        }

        public static int CrossValidYT(List<KeyValuePair<double, double>> list_res)
        {
            var csv_list = CSVReadYT();


            foreach (var c in csv_list)
            {
                var nn = new NeuralNetwork(2, 1, activate);
                nn.HiddenLayerCreate(3);
                nn.HiddenLayerCreate(3);

                TrainingNetwork(ref nn, activate, csv_list.Where(x => x != c).ToList());

                i++;

                var result = nn.ForwardPropagation(c.input);

                list_res.Add(new KeyValuePair<double, double>(result[0, 0], c.output[0]));

            }

            return csv_list.Count();
        }
   
     
        public static void TrainingNetwork(ref NeuralNetwork nn, Perceptron.myDelegate activationFunction, List<TrainingData> list)
        {
            foreach (var l in list)
            {
                nn.BackPropagation(l.input, l.output, activationFunction);
            }
        }

        public static List<TrainingData> CSVReadYT()
        {
            Console.WriteLine("Wczytanie danych z pliku CSV");
            var list_of_data = new List<TrainingData>();
                    
            var column1 = new List<string>();
            var column2 = new List<string>();
            var column3 = new List<string>();
            var column4 = new List<string>();
            var column5 = new List<string>();
            var column6 = new List<string>();
            var column7 = new List<string>();
            var column8 = new List<string>();
            var column9 = new List<string>();
            var column10 = new List<string>();

            using (var rd = new StreamReader(path))
            {
                while (!rd.EndOfStream)
                {
                    var splits = rd.ReadLine().Split(';');
                    column1.Add(splits[0]);
                    column2.Add(splits[1]);
                    column3.Add(splits[2]);
                    column4.Add(splits[3]);
                    column5.Add(splits[4]);

                }
            }



            for (int i = 0; i < column1.Count(); i++ )
            {
                var tr_data = new TrainingData(new double[] { double.Parse(column1[i]), double.Parse(column2[i]), double.Parse(column3[i]), 

                    double.Parse(column4[i])              
                },
                    
                    
                    new double[] { double.Parse(column5[i]) });
                list_of_data.Add(tr_data);
            }

            return list_of_data;
        }


        public static List<TrainingData> CSVReadXO()
        {
            Console.WriteLine("Wczytanie danych z pliku CSV");
            var list_of_data = new List<TrainingData>();

            var column1 = new List<string>();
            var column2 = new List<string>();
            var column3 = new List<string>();
            var column4 = new List<string>();
            var column5 = new List<string>();
            var column6 = new List<string>();
            var column7 = new List<string>();
            var column8 = new List<string>();
            var column9 = new List<string>();
            var column10 = new List<string>();

            using (var rd = new StreamReader(path))
            {
                while (!rd.EndOfStream)
                {
                    var splits = rd.ReadLine().Split(';');
                    column1.Add(splits[0]);
                    column2.Add(splits[1]);
                    column3.Add(splits[2]);
                    column4.Add(splits[3]);
                    column5.Add(splits[4]);
                    column6.Add(splits[5]);
                    column7.Add(splits[6]);
                    column8.Add(splits[7]);
                    column9.Add(splits[8]);
                    column10.Add(splits[9]);
   

                }
            }



            for (int i = 0; i < column1.Count(); i++)
            {
                var tr_data = new TrainingData(new double[] { double.Parse(column1[i]), double.Parse(column2[i]), double.Parse(column3[i]), 

                    double.Parse(column4[i]), double.Parse(column5[i]), double.Parse(column6[i]), 
                    double.Parse(column7[i]), double.Parse(column8[i]), double.Parse(column9[i])
                 
                },

                    new double[] { double.Parse(column10[i]) });
                list_of_data.Add(tr_data);
            }

            return list_of_data;
        }

        #endregion
    }
}
