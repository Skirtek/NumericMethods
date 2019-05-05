using System;
using System.Collections.Generic;
using NumericMethods.Models;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class SolveInterpolationPageViewModel : BaseViewModel, INavigatedAware
    {
        public SolveInterpolationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
        }

        private List<PointModel> _pointsList = new List<PointModel>();
        public List<PointModel> PointsList
        {
            get => _pointsList;
            set => SetProperty(ref _pointsList, value);
        }

        private string _argument;
        public string Argument
        {
            get => _argument;
            set => SetProperty(ref _argument, value);
        }

        private string _result;
        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        private string _pointsCount;
        public string PointsCount
        {
            get => _pointsCount;
            set => SetProperty(ref _pointsCount, value);
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (!parameters.TryGetValue(NavParams.Points, out List<PointModel> pointsList) ||
                    !parameters.TryGetValue(NavParams.Argument, out string argument))
                {
                    ShowError();
                    return;
                }

                PointsList = pointsList;
                Argument = argument;

                if (!double.TryParse(Argument, out double arg))
                {
                    ShowError();
                    return;
                }

                IsBusy = true;

                var xArguments = new List<double>();
                var yArguments = new List<double>();

                foreach (var point in PointsList)
                {
                    double.TryParse(point.X, out double x);
                    double.TryParse(point.Y, out double y);

                    xArguments.Add(x);
                    yArguments.Add(y);
                }

                Result = $"{LagrangeInterpolatingPolynomial(arg, xArguments.ToArray(), yArguments.ToArray())}";
                PointsCount = $"{PointsList.Count}";
            }
            catch (Exception ex)
            {
                ShowError();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private double LagrangeInterpolatingPolynomial(double x, double[] xArray, double[] yArray)
        {
            int size = xArray.Length;

            double lagrangePolynomial = 0;

            for (int i = 0; i < size; i++)
            {
                lagrangePolynomial += BasicPolynomial(x, xArray, i) * yArray[i];
            }

            return lagrangePolynomial;
        }

        private double BasicPolynomial(double x, double[] xArray, int i)
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

        private async void ShowError()
        {
            await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
            await NavigationService.GoBackAsync();
        }
    }
}
