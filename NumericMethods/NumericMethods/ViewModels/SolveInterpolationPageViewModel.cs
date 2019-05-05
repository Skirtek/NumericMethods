using System;
using System.Collections.Generic;
using System.Linq;
using NumericMethods.Models;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class SolveInterpolationPageViewModel : BaseViewModel, INavigatingAware
    {
        public SolveInterpolationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            GoToInterpolationChartPageCommand = new DelegateCommand(GoToInterpolationChartPage);
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

        private string _aitkenResult;
        public string AitkenResult
        {
            get => _aitkenResult;
            set => SetProperty(ref _aitkenResult, value);
        }

        private string _pointsCount;
        public string PointsCount
        {
            get => _pointsCount;
            set => SetProperty(ref _pointsCount, value);
        }

        public DelegateCommand GoToInterpolationChartPageCommand { get; set; }

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            try
            {
                IsBusy = true;

                if (!parameters.TryGetValue(NavParams.Points, out List<PointModel> pointsList) ||
                    !parameters.TryGetValue(NavParams.Argument, out string argument))
                {
                    ShowError();
                    IsBusy = false;

                    return;
                }

                PointsList = pointsList;
                Argument = argument;

                if (!double.TryParse(Argument, out double arg))
                {
                    ShowError();
                    IsBusy = false;

                    return;
                }

                var duplicates = PointsList.GroupBy(x => x).Select(x => x.Key.X).GroupBy(x => x).Where(g => g.Count() > 1).ToList();

                if (duplicates.Count > 0)
                {
                    await ShowAlert(AppResources.Common_InvalidFunction, AppResources.Common_InvalidFunction_Message);
                    await NavigationService.GoBackAsync();
                    IsBusy = false;

                    return;
                }

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
                AitkenResult = $"{AitkenInterpolation(arg, xArguments.ToArray(), yArguments.ToArray())}";
                PointsCount = $"{PointsList.Count}";
            }
            catch (Exception)
            {
                ShowError();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void GoToInterpolationChartPage()
        {
            IsBusy = true;

            var list = new List<PointModel>();
            PointsList.ForEach(list.Add);
            list.Add(new PointModel { X = Argument, Y = Result });

            await NavigationService.NavigateAsync(NavSettings.InterpolationChartPage, new NavigationParameters
            {
                { NavParams.Points, list }
            });

            IsBusy = false;
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

        private static void FillInTable(ref List<List<double>> table, int size, double argument, double[] xArray, double[] yArray)
        {
            table[0].AddRange(yArray);

            for (int step = 1; step <= size; step++)
            {
                for (int i = 1; i < table[step - 1].Count; i++)
                {
                    double polynomial = 1 / (xArray[i + step - 1] - xArray[i - 1]) *
                                        ((table[step - 1][i - 1] * (xArray[i + step - 1] - argument)) -
                                         ((xArray[i - 1] - argument) * table[step - 1][i]));

                    table[step].Add(polynomial);
                }
            }
        }

        private static double AitkenInterpolation(double argument, double[] xArray, double[] yArray)
        {
            int size = yArray.Length;

            var table = new List<List<double>>();

            for (int i = 0; i <= size; i++)
            {
                table.Add(new List<double>());
            }

            FillInTable(ref table, size, argument, xArray, yArray);

            return table[size - 1][0];
        }
    }
}
