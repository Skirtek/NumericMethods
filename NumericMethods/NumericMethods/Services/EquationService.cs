using System;
using System.Collections.Generic;
using NumericMethods.Interfaces;
using NumericMethods.Models;
using NumericMethods.Settings;

namespace NumericMethods.Services
{
    public class EquationService : IEquation
    {
        private readonly IMatrix _matrix;

        public EquationService(IMatrix matrix)
        {
            _matrix = matrix;
        }

        public string TwoVariablesEquation(List<Equation> equationsList)
        {
            var constantTerms = new List<double>();
            var mainMatrix = new double[2, 2];

            short rowIterator = 0;

            foreach (var equation in equationsList)
            {
                double.TryParse(equation.X, out double x);
                double.TryParse(equation.Y, out double y);

                mainMatrix[rowIterator, 0] = x;
                mainMatrix[rowIterator, 1] = y;

                double.TryParse(equation.ConstantTerm, out double constantTerm);
                constantTerms.Add(constantTerm);
                rowIterator++;
            }

            double[,] xMatrix = new double[2, 2];
            double[,] yMatrix = new double[2, 2];

            Array.Copy(mainMatrix, xMatrix, mainMatrix.Length);
            Array.Copy(mainMatrix, yMatrix, mainMatrix.Length);

            for (short i = 0; i < equationsList.Count; i++)
            {
                xMatrix[i, 0] = constantTerms[i];
                yMatrix[i, 1] = constantTerms[i];
            }

            double w = _matrix.MatrixDeterminant(mainMatrix);
            double wx = _matrix.MatrixDeterminant(xMatrix);
            double wy = _matrix.MatrixDeterminant(yMatrix);

            switch (w)
            {
                case 0 when (Math.Abs(wx) > AppSettings.Epsilon || Math.Abs(wy) > AppSettings.Epsilon):
                    return "Układ sprzeczny";
                case 0 when Math.Abs(wx) < AppSettings.Epsilon && Math.Abs(wy) < AppSettings.Epsilon:
                    return "Układ nieoznaczony";
            }

            return $"x = {wx / w}{Environment.NewLine}y = {wy / w}";
        }

        public string ThreeVariablesEquation(List<Equation> equationsList)
        {
            var constantTerms = new List<double>();
            var mainMatrix = new double[3, 3];

            short rowIterator = 0;

            foreach (var equation in equationsList)
            {
                double.TryParse(equation.X, out double x);
                double.TryParse(equation.Y, out double y);
                double.TryParse(equation.Z, out double z);

                mainMatrix[rowIterator, 0] = x;
                mainMatrix[rowIterator, 1] = y;
                mainMatrix[rowIterator, 2] = z;

                double.TryParse(equation.ConstantTerm, out double constantTerm);
                constantTerms.Add(constantTerm);
                rowIterator++;
            }

            double[,] xMatrix = new double[3, 3];
            double[,] yMatrix = new double[3, 3];
            double[,] zMatrix = new double[3, 3];


            Array.Copy(mainMatrix, xMatrix, mainMatrix.Length);
            Array.Copy(mainMatrix, yMatrix, mainMatrix.Length);
            Array.Copy(mainMatrix, zMatrix, mainMatrix.Length);


            for (short i = 0; i < equationsList.Count; i++)
            {
                xMatrix[i, 0] = constantTerms[i];
                yMatrix[i, 1] = constantTerms[i];
                zMatrix[i, 2] = constantTerms[i];
            }

            double w = _matrix.MatrixDeterminant(mainMatrix);
            double wx = _matrix.MatrixDeterminant(xMatrix);
            double wy = _matrix.MatrixDeterminant(yMatrix);
            double wz = _matrix.MatrixDeterminant(zMatrix);

            switch (w)
            {
                case 0 when (Math.Abs(wx) > AppSettings.Epsilon || Math.Abs(wy) > AppSettings.Epsilon || Math.Abs(wz) > AppSettings.Epsilon):
                    return "Układ sprzeczny";
                case 0 when Math.Abs(wx) < AppSettings.Epsilon && Math.Abs(wy) < AppSettings.Epsilon && Math.Abs(wz) < AppSettings.Epsilon:
                    return "Układ nieoznaczony";
            }

            return $"x = {wx / w}{Environment.NewLine}y = {wy / w}{Environment.NewLine}z = {wz / w}";
        }

        public string FourVariablesEquation(List<Equation> equationsList)
        {
            var constantTerms = new List<double>();
            var mainMatrix = new double[4, 4];

            short rowIterator = 0;

            foreach (var equation in equationsList)
            {
                double.TryParse(equation.X, out double x);
                double.TryParse(equation.Y, out double y);
                double.TryParse(equation.Z, out double z);
                double.TryParse(equation.T, out double t);

                mainMatrix[rowIterator, 0] = x;
                mainMatrix[rowIterator, 1] = y;
                mainMatrix[rowIterator, 2] = z;
                mainMatrix[rowIterator, 3] = t;

                double.TryParse(equation.ConstantTerm, out double constantTerm);
                constantTerms.Add(constantTerm);
                rowIterator++;
            }

            double[,] xMatrix = new double[4, 4];
            double[,] yMatrix = new double[4, 4];
            double[,] zMatrix = new double[4, 4];
            double[,] tMatrix = new double[4, 4];

            Array.Copy(mainMatrix, xMatrix, mainMatrix.Length);
            Array.Copy(mainMatrix, yMatrix, mainMatrix.Length);
            Array.Copy(mainMatrix, zMatrix, mainMatrix.Length);
            Array.Copy(mainMatrix, tMatrix, mainMatrix.Length);

            for (short i = 0; i < equationsList.Count; i++)
            {
                xMatrix[i, 0] = constantTerms[i];
                yMatrix[i, 1] = constantTerms[i];
                zMatrix[i, 2] = constantTerms[i];
                tMatrix[i, 3] = constantTerms[i];
            }

            double w = _matrix.MatrixDeterminant(mainMatrix);
            double wx = _matrix.MatrixDeterminant(xMatrix);
            double wy = _matrix.MatrixDeterminant(yMatrix);
            double wz = _matrix.MatrixDeterminant(zMatrix);
            double wt = _matrix.MatrixDeterminant(tMatrix);

            switch (w)
            {
                case 0 when Math.Abs(wx) > AppSettings.Epsilon || Math.Abs(wy) > AppSettings.Epsilon || Math.Abs(wz) > AppSettings.Epsilon || Math.Abs(wt) > AppSettings.Epsilon:
                    return "Układ sprzeczny";
                case 0 when Math.Abs(wx) < AppSettings.Epsilon && Math.Abs(wy) < AppSettings.Epsilon && Math.Abs(wz) < AppSettings.Epsilon && Math.Abs(wt) < AppSettings.Epsilon:
                    return "Układ nieoznaczony";
            }

            return $"x = {wx / w}{Environment.NewLine}y = {wy / w}{Environment.NewLine}z = {wz / w}{Environment.NewLine}t = {wt / w}";
        }
    }
}
