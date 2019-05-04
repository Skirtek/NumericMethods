using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class SolveInterpolationPageViewModel : BaseViewModel
    {
        public SolveInterpolationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            double[] x = { -2, -1, 0, 1, 3 };
            double[] y = {4, 1, 0, 1, 9};

            var result = LagrangeInterpolatingPolynomial(2, x, y);
        }

        private static double BasicPolynomial(double x, double[] xArray, int i)
        {
            int size = xArray.Length;

            double basicPolynomial = 1;

            for (int j = 0; j < size; j++)
            {
                if (j != i)
                {
                    basicPolynomial *= (x - xArray[j]) / (xArray[i] - xArray[j]);
                }
            }

            return basicPolynomial;
        }

        public static double LagrangeInterpolatingPolynomial(double x, double[] xArray, double[] yArray)
        {
            int size = xArray.Length;

            double lagrangePolynomial = 0;

            for (int i = 0; i < size; i++)
            {
                lagrangePolynomial += BasicPolynomial(x, xArray, i) * yArray[i];
            }

            return lagrangePolynomial;
        }
    }
}
