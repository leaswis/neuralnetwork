using System.Collections.Generic;
using System.Windows;

namespace ConsoleApplication19
{
    public class TrainingData
    {
        public double[] input;
        public double[] output;
   
        public TrainingData(double[] data_in, double[] data_out)
        {
            input = data_in;
            output = data_out;
        }


        public TrainingData(double[] data_in)
        {
            input = data_in;
            output = new double[0];
        }
    }
}
