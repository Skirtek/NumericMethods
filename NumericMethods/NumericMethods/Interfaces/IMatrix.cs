namespace NumericMethods.Interfaces
{
    public interface IMatrix
    {
        double MatrixDeterminant(double[,] matrix);

        int rankOfMatrix(int rows, int columns, double[,] mat);
    }
}