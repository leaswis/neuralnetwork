using System;

namespace ConsoleApplication19
{
    public class Perceptron
    {
        public double[,] scales; //macierz wag
        public delegate double myDelegate(double data, bool deriv = false); 
        public myDelegate activationFunction;
        public Perceptron(int x, int y, myDelegate activationFunction)
        {
            scales = new double[x, y]; //wielkosc macierzy wag
            this.activationFunction = activationFunction;
        }

        public Perceptron(myDelegate activationFunction)
        {
            this.activationFunction = activationFunction;
        }

  
        public static double binary(double data)
        {
            if (data < 0) return 0;
            else return 1;
        }


        public static double sigmod(double data, bool deriv = false)
        {
            if (deriv)
                return data * (1 - data);
            return 1 / (1 + Math.Pow(Math.E, data));
        }

        //generowanie losowej macierzy wag
        public void scalesGen()
        {
            Random rnd = new Random();
            for (int i = 0; i < scales.GetLength(0); i++)
                for (int j = 0; j < scales.GetLength(1); j++)
                    scales[i, j] = rnd.NextDouble() * 10 - 5; //zakres (-5;5)
        }
    }
}
