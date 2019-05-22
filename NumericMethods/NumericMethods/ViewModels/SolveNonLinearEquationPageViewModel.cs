using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NumericMethods.Enums;
using NumericMethods.Interfaces;
using NumericMethods.Models;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class SolveNonLinearEquationPageViewModel : BaseViewModel, INavigatingAware
    {
        private readonly ICommonFunctions _commonFunctions;

        public SolveNonLinearEquationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            ICommonFunctions commonFunctions)
            : base(navigationService, pageDialogService)
        {
            _commonFunctions = commonFunctions;
            GoToNonLinearChartPageCommand = GetBusyDependedCommand(GoToNonLinearChartPage);
        }

        private string _result;
        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        private string _iterations;
        public string Iterations
        {
            get => _iterations;
            set => SetProperty(ref _iterations, value);
        }

        private string _formula;
        public string Formula
        {
            get => _formula;
            set => SetProperty(ref _formula, value);
        }

        private ObservableCollection<ResultList> _resultList = new ObservableCollection<ResultList>();
        public ObservableCollection<ResultList> ResultList
        {
            get => _resultList;
            set => SetProperty(ref _resultList, value);
        }

        public DelegateCommand GoToNonLinearChartPageCommand { get; set; }

        private PrecisionLevels Precision { get; set; }

        private List<Operation> _operations = new List<Operation>();

        private List<Operation> _initialFunction = new List<Operation>();

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            if (!parameters.TryGetValue(NavParams.Equations, out string formula) || !parameters.TryGetValue(NavParams.Precision, out PrecisionLevels precision))
            {
                await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                await NavigationService.GoBackAsync();
                return;
            }

            Formula = formula;
            Precision = precision;

            await Calculate();
        }

        private async void GoToNonLinearChartPage()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.NonLinearChartPage, new NavigationParameters
            {
                { NavParams.Function, _initialFunction },
                { NavParams.Formula, Formula }
            });

            IsBusy = false;
        }

        private async Task Calculate()
        {
            try
            {
                IsBusy = true;

                var result = _commonFunctions.PrepareFunction(Formula);

                if (!result.IsSuccess)
                {
                    await ShowError(HandleResponseCode(result.ResponseCode));
                    return;
                }

                short precision;

                switch (Precision)
                {
                    case PrecisionLevels.Low:
                        precision = 2;
                        break;
                    case PrecisionLevels.Medium:
                        precision = 4;
                        break;
                    case PrecisionLevels.High:
                        precision = 8;
                        break;
                    default:
                        precision = 4;
                        break;
                }

                _operations = result.Operations;
                _initialFunction = result.Operations;

                if (_operations.Count == 0)
                {
                    await ShowAlert(AppResources.Common_Ups, AppResources.Common_NoOperations);
                    return;
                }

                CalculateNewtonRaphsonMethod(AppSettings.InitialX);
                GraeffesMethod(precision);
            }
            catch (Exception ex)
            {
                await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                await NavigationService.GoBackAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void GraeffesMethod(int maxIterations)
        {

            for (int i = 0; i < maxIterations - 1; i++)
            {
                MultipleFunctions();
                FindSquares();
            }

            MultipleFunctions();

            var roots = new List<ResultList>();
            short iterator = 1;

            for (int i = 0; i < _operations.Count - 1; i++)
            {
                var value = (float)Math.Pow(_operations[i].Value / _operations[i + 1].Value,
                    1.0 / Math.Pow(2, maxIterations));
                if (Math.Abs(_commonFunctions.FunctionResult(value, _initialFunction)) < 10)
                {
                    roots.Add(new ResultList { Value = value, Position = $"{iterator}. " });
                    iterator++;
                }
                else if (Math.Abs(_commonFunctions.FunctionResult(value * -1, _initialFunction)) < 10)
                {
                    roots.Add(new ResultList { Value = value * -1, Position = $"{iterator}. " });
                    iterator++;
                }
            }

            ResultList = new ObservableCollection<ResultList>(roots);
            Iterations = $"{maxIterations}";
        }

        private void FindSquares()
        {
            foreach (var operation in _operations)
            {
                operation.Weight /= 2;
            }
        }

        private void MultipleFunctions()
        {
            var operationsList = new List<Operation>();
            var negatedOperationsList = new List<Operation>();

            foreach (var operation in _operations)
            {
                operationsList.Add(new Operation { Value = operation.Value, Weight = operation.Weight });
                negatedOperationsList.Add(new Operation { Value = Math.Abs(operation.Weight % 2) > AppSettings.Epsilon ? operation.Value * -1 : operation.Value, Weight = operation.Weight });
            }

            var result = MultiplyFunctions(operationsList, negatedOperationsList, operationsList.Count);
            var summed = new List<Operation>();

            foreach (var operation in result)
            {
                float sum = result.Where(c => c.Weight == operation.Weight).Sum(f => f.Value);
                if (sum != 0)
                {
                    summed.Add(new Operation { Value = Math.Abs(sum), Weight = operation.Weight });
                }
            }

            _operations = summed.GroupBy(y => y.Weight).Select(group => group.First()).ToList();
        }

        private List<Operation> MultiplyFunctions(List<Operation> operationsList, List<Operation> negatedOperationsList, int count)
        {
            var prod = new List<Operation>();

            for (var i = 0; i < count; i++)
            {
                for (var j = 0; j < count; j++)
                {
                    prod.Add(new Operation
                    {
                        Value = operationsList[i].Value * negatedOperationsList[j].Value,
                        Weight = operationsList[i].Weight + negatedOperationsList[j].Weight
                    });
                }
            }

            return prod;
        }

        private void CalculateNewtonRaphsonMethod(float x)
        {
            var derivativeOperations = _commonFunctions.CalculateDerivative(_operations);

            float h = _commonFunctions.FunctionResult(x, _operations) / _commonFunctions.FunctionResult(x, derivativeOperations);
            uint iterations = 0;

            while (Math.Abs(h) >= 0.001)
            {
                h = _commonFunctions.FunctionResult(x, _operations) / _commonFunctions.FunctionResult(x, derivativeOperations);

                x -= h;
                iterations++;
                if (iterations == 100000)
                {
                    break;
                }
            }

            if (float.IsNaN(x) || Formula.Contains("ln[x]") && x < 0)
            {
                Result = AppResources.Common_NoSolutions;
                return;
            }

            Result = $"{Math.Round(x * 100.0) / 100.0}";
        }
    }
}