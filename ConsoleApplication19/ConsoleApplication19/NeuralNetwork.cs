using System;
using System.Linq;

namespace ConsoleApplication19
{
    public class NeuralNetwork
    {
        public TrainingData training_data;
        public LayerN[] layers;
        private int input_; //liczba danych na wejsciu
        private int output_; //liczba danych na wyjsciu
        private Perceptron.myDelegate activationFunction;

        public NeuralNetwork(int x, int y, Perceptron.myDelegate activationFunction)
        {
            this.input_ = x;
            this.output_ = y;
            this.activationFunction = activationFunction;
     
            //tworzenie warstwy wej+wyj
            layers = new LayerN[2];
            layers[0] = new LayerN(0, x, activationFunction);
            layers[1] = new LayerN(x, y, activationFunction);
        }

        //dodawanie warstwy ukrytej
        public void HiddenLayerCreate(int n)
        {
            int last_size= 0;
            var tmp = layers;
            layers = new LayerN[tmp.Length + 1];
            for (int i = 0; i < tmp.Length - 1; i++)
            {
                layers[i] = tmp[i];
                last_size = tmp[i].size;
            }
            layers[tmp.Length - 1] = new LayerN(last_size, n, activationFunction);
            layers[tmp.Length] = new LayerN(n, output_, activationFunction);
        }

 
        public double[,] ForwardPropagation(params double[] data)
        {
            training_data = new TrainingData(data, new double[] { });
            double[,] input_data = VecToMatrix(training_data.input);
            layers[0].values = input_data;
            for (int i = 1; i < layers.Length; i++)
            {
                input_data = Multiply(input_data, layers[i].perceptron.scales);
                input_data = activate(input_data, layers[i].perceptron.activationFunction);
                layers[i].values = input_data;
            }
            return input_data; //wynik propagacji w przod
        }

        private double[,] activate(double[,] input, Perceptron.myDelegate activationFunction, bool deriv = false)
        {
            for (int i = 0; i < input.GetLength(0); i++)
            {
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    input[i, j] = activationFunction(input[i, j], deriv);
                }
            }
            return input;
        }

   
        private double[,] error(TrainingData tmp)
        {
            double[,] y = VecToMatrix(tmp.output);
            var ysim = ForwardPropagation(tmp.input);
            return Substract(y, ysim);
        }

      
        public void BackPropagation(double[] input, double[] output, Perceptron.myDelegate activationFunction)
        {
            var error_ = error(new TrainingData(input, output));
            int n_layers = layers.Length - 1;
            for (int i = 0; i < n_layers; i++)
            {
                var activate_values= activate(layers[n_layers - i].values, activationFunction, true);
                var delta = MultiplyVector(error_, activate_values); 

                var scales_trans = Trans(layers[n_layers - i].perceptron.scales);
                error_ = Multiply(delta, scales_trans);
               
                var multiply_matrix = MultiplyMatrixSc(layers[n_layers - i - 1].values, Trans(delta));
                layers[n_layers - i].perceptron.scales = Substract(layers[n_layers - i].perceptron.scales, Trans(multiply_matrix));
            }
        }


        #region helpers
        public static double[,] Multiply(double[,] firstMatrix, double[,] secondMatrix)
        {
            int firstColumns = firstMatrix.GetLength(1);
            int firstRows = firstMatrix.GetLength(0);
            int secondColumns = secondMatrix.GetLength(1);
            int secondRows = secondMatrix.GetLength(0);
            if (firstColumns != secondRows)
            {
                if (firstColumns < secondRows)
                {
                    firstMatrix = ColumnsComplement(firstMatrix, secondRows);
                    firstColumns = secondRows;
                }
                else
                {
                    secondMatrix = RowsComplement(secondMatrix, firstColumns);
                    secondRows = firstColumns;
                }
            }
            double[,] resultMatrix = new double[firstRows, secondColumns];
            for (int i = 0; i < firstRows; i++)
            {
                for (int j = 0; j < secondColumns; j++)
                {
                    for (int k = 0; k < firstColumns; k++)
                    {
                        resultMatrix[i, j] += firstMatrix[i, k] * secondMatrix[k, j];
                    }
                }
            }
            return resultMatrix;
        }

     
        private static double[,] RowsComplement(double[,] matrix, int totalRows)
        {
            double[,] complementedMatrix = new double[totalRows, matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    complementedMatrix[i, j] = matrix[i, j];
            return complementedMatrix;
        }


        private static double[,] ColumnsComplement(double[,] matrix, int totalColumns)
        {
            double[,] complementedMatrix = new double[matrix.GetLength(0), totalColumns];
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    complementedMatrix[i, j] = matrix[i, j];
            return complementedMatrix;
        }

 
        private static void Complement(ref double[,] firstMatrix, ref double[,] secondMatrix)
        {
            int firstColumns = firstMatrix.GetLength(1);
            int firstRows = firstMatrix.GetLength(0);
            int secondColumns = secondMatrix.GetLength(1);
            int secondRows = secondMatrix.GetLength(0);

            if (firstColumns != secondColumns || firstRows != secondRows)
            {
                if (firstColumns < secondColumns)
                {
                    firstMatrix = ColumnsComplement(firstMatrix, secondColumns);
                    firstColumns = secondColumns;
                }
                else
                {
                    secondMatrix = ColumnsComplement(secondMatrix, firstColumns);
                    secondColumns = firstColumns;
                }

                if (firstRows < secondRows)
                {
                    firstMatrix = RowsComplement(firstMatrix, secondRows);
                    firstRows = secondRows;
                }
                else
                {
                    secondMatrix = RowsComplement(secondMatrix, firstRows);
                    secondRows = firstRows;
                }
            }
        }

   
        public static double[,] Substract(double[,] firstMatrix, double[,] secondMatrix)
        {
            Complement(ref firstMatrix, ref secondMatrix);
            double[,] resultMatrix = new double[firstMatrix.GetLength(0), firstMatrix.GetLength(1)];
            for (int i = 0; i < firstMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < firstMatrix.GetLength(1); j++)
                {
                    resultMatrix[i, j] = firstMatrix[i, j] - secondMatrix[i, j];
                }
            }
            return resultMatrix;
        }

        public static double[,] Trans(double[,] matrix)
        {
            int matrixRow = matrix.GetLength(0);
            int matrixColumn = matrix.GetLength(1);
            double[,] newMatrix = new double[matrixColumn, matrixRow];
            for (int i = 0; i < matrixColumn; i++)
            {
                for (int j = 0; j < matrixRow; j++)
                {
                    newMatrix[i, j] = matrix[j, i];
                }
            }
            return newMatrix;
        }

  

        public static double[,] VecToMatrix(double[] input)
        {
            double[,] matrix = new double[1, input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                matrix[0, i] = input[i];
            }
            return matrix;
        }


        public static double[,] MultiplyMatrixSc(double[,] firstMatrix, double[,] secondMatrix)
        {
            int firstColumns = firstMatrix.GetLength(1);
            int firstRows = firstMatrix.GetLength(0);
            int secondColumns = secondMatrix.GetLength(1);
            int secondRows = secondMatrix.GetLength(0);

            if (firstRows == secondColumns)
            {
                var resultMatrix = new double[secondRows, firstColumns];
                for (int i = 0; i < secondRows; i++)
                {
                    if (secondColumns == 1)
                    {
                        for (int j = 0; j < firstColumns; j++)
                        {
                            resultMatrix[i, j] = firstMatrix[0, j] * secondMatrix[i, 0];
                        }
                    }
                    else
                    {
                        for (int j = 0; j < firstColumns; j++)
                        {
                            resultMatrix[i, j] = firstMatrix[0, j] * secondMatrix[i, j];
                        }
                    }
                }
                return resultMatrix;
            }
            else
            {
                throw new Exception("Liczba wierszy pierwszej macierzy różni się od liczby kolumn drugiej");
            }
        }


        public static double[,] MultiplyVector(double[,] firstMatrix, double[,] secondMatrix)
        {
            int firstColumns = firstMatrix.GetLength(1);
            int secondColumns = secondMatrix.GetLength(1);
            if (firstColumns == secondColumns)
            {
                var result = new double[1, secondColumns];
                for (int i = 0; i < firstColumns; i++)
                {
                    result[0, i] = firstMatrix[0, i] * secondMatrix[0, i];
                }

                return result;
            }
            else
            {
                throw new Exception("Wektory mają różną długość");
            }

        }

        #endregion
    }
}
