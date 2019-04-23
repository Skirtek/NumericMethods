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

        public int rankOfMatrix(int rows, int columns, double[,] mat)
        {

            int rank = columns;

            for (int row = 0; row < rank; row++)
            {
                if (mat[row, row] != 0)
                {
                    for (int col = 0; col < rows; col++)
                    {
                        if (col != row)
                        {
                            double mult =
                               mat[col, row] /
                                        mat[row, row];

                            for (int i = 0; i < rank; i++)

                                mat[col, i] -= (int)mult
                                         * mat[row, i];
                        }
                    }
                }
                else
                {
                    bool reduce = true;
                    for (int i = row + 1; i < rows; i++)
                    {
                        if (mat[i, row] != 0)
                        {
                            swap(mat, row, i, rank);
                            reduce = false;
                            break;
                        }
                    }

                    if (reduce)
                    {
                        rank--;

                        for (int i = 0; i < rows; i++)
                            mat[i, row] = mat[i, rank];
                    }

                    row--;
                }
            }

            return rank;
        }

        private void swap(double[,] mat,
            int row1, int row2, int col)
        {
            for (int i = 0; i < col; i++)
            {
                double temp = mat[row1, i];
                mat[row1, i] = mat[row2, i];
                mat[row2, i] = temp;
            }
        }
    }
}
