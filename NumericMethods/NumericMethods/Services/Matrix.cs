using System;
using NumericMethods.Interfaces;

namespace NumericMethods.Services
{
    public class Matrix : IMatrix
    {
        public double MatrixDeterminant(double[,] matrix)
        {

            double result = 0;

            switch (matrix.GetLength(0))
            {
                case 1:
                    result = matrix[0, 0];
                    return result;
                case 2:
                    result = matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
                    return result;
            }

            for (int k = 0; k < matrix.GetLength(0); k++)
            {
                result += Math.Pow(-1, k) * matrix[0, k] * MatrixDeterminant(TrimArray(0, k, matrix));
            }

            return result;
        }

        private double[,] TrimArray(int rowToRemove, int columnToRemove, double[,] originalArray)
        {
            double[,] result = new double[originalArray.GetLength(0) - 1, originalArray.GetLength(1) - 1];

            for (int i = 0, j = 0; i < originalArray.GetLength(0); i++)
            {
                if (i == rowToRemove)
                {
                    continue;
                }

                for (int k = 0, u = 0; k < originalArray.GetLength(1); k++)
                {
                    if (k == columnToRemove)
                    {
                        continue;
                    }

                    result[j, u] = originalArray[i, k];
                    u++;
                }

                j++;
            }

            return result;
        }
    }
}
