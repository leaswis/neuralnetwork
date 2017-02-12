using System;

namespace ConsoleApplication19
{
    public class LayerN
    {
        public Perceptron perceptron;
        public int size;
        public double[,] values; //wartosci warstwy
        public LayerN(int previous, int neurons_nbLayer, Perceptron.myDelegate activationFunction)
        {
            this.size = neurons_nbLayer;
            values = new double[1, neurons_nbLayer];
            perceptron = new Perceptron(previous, neurons_nbLayer, activationFunction);
            perceptron.scalesGen();
        }
    }
}
