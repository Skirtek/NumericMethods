using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NumericMethods.Interfaces;
using NumericMethods.Models;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class SolveEquationPageViewModel : BaseViewModel, INavigatingAware
    {
        private readonly IMatrix _matrix;

        public SolveEquationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            IMatrix matrix)
            : base(navigationService, pageDialogService)
        {
            _matrix = matrix;
            GoToLinearChartPageCommand = GetBusyDependedCommand(GoToLinearChartPage);
        }

        private List<Equation> EquationsList { get; set; }

        public DelegateCommand GoToLinearChartPageCommand { get; set; }

        private string _result;

        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        private async void GoToLinearChartPage()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.LinearChart);

            IsBusy = false;
        }

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            bool canGetList = parameters.TryGetValue(NavParams.Equations, out List<Equation> equations);

            if (!canGetList)
            {
                await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                await NavigationService.GoBackAsync();
                return;
            }

            EquationsList = equations;

            await TwoVariablesEquation();
        }

        private async Task TwoVariablesEquation()
        {
            try
            {
                IsBusy = true;
                var constantTerms = new List<double>();
                var mainMatrix = new double[2, 2];

                short rowIterator = 0;

                foreach (var equation in EquationsList)
                {
                    mainMatrix[rowIterator, 0] = double.Parse(equation.X);
                    mainMatrix[rowIterator, 1] = double.Parse(equation.Y);

                    constantTerms.Add(double.Parse(equation.ConstantTerm));
                    rowIterator++;
                }

                double[,] xMatrix = new double[2, 2];
                double[,] yMatrix = new double[2, 2];

                Array.Copy(mainMatrix, xMatrix, mainMatrix.Length);
                Array.Copy(mainMatrix, yMatrix, mainMatrix.Length);

                for (short i = 0; i < EquationsList.Count; i++)
                {
                    xMatrix[i, 0] = constantTerms[i];
                    yMatrix[i, 1] = constantTerms[i];
                }

                double w = _matrix.MatrixDeterminant(mainMatrix);
                double wx = _matrix.MatrixDeterminant(xMatrix);
                double wy = _matrix.MatrixDeterminant(yMatrix);

                switch (w)
                {
                    case 0 when (wx == 0 || wy == 0):
                        Result = "Układ sprzeczny";
                        return;
                    case 0 when wx == 0 && wy == 0:
                        Result = "Układ nieoznaczony";
                        return;
                }

                Result = $"x = {wx / w} y = {wy / w}";
            }
            catch (Exception ex)
            {
                await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
