using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NumericMethods.Enums;
using NumericMethods.Interfaces;
using NumericMethods.Models;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.AppModel;
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
        }

        private string _result;
        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        private string _formula;
        public string Formula
        {
            get => _formula;
            set => SetProperty(ref _formula, value);
        }

        private List<Operation> _operations = new List<Operation>();

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            if (!parameters.TryGetValue(NavParams.Equations, out string formula))
            {
                await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                await NavigationService.GoBackAsync();
                return;
            }

            IsBusy = true;
            Formula = formula;
            await Calculate();
            IsBusy = false;
        }

        private async Task Calculate()
        {
            //TODO dodać przekazywanie realnej funkcji
            var result = _commonFunctions.PrepareFunction(Formula);

            if (!result.IsSuccess)
            {
                switch (result.ResponseCode)
                {
                    case FunctionResponse.UnclosedParentheses:
                        await ShowError(AppResources.FunctionResponse_UnclosedParentheses_Message);
                        break;
                    case FunctionResponse.DivideByZero:
                        await ShowError(AppResources.FunctionResponse_DivideByZero_Message);
                        break;
                    case FunctionResponse.WrongFunction:
                        await ShowError(AppResources.FunctionResponse_WrongFunction_Message);
                        break;
                    case FunctionResponse.CriticalError:
                        await ShowError(AppResources.Common_SomethingWentWrong);
                        break;
                }

                return;
            }

            _operations = result.Operations;
            //TODO zmienić na dziedzinę
            //CalculateNewtonRaphsonMethod(-20);
            //CalculateBisectionMethod(-20, 20);
            var x = GraeffeMethod(2);
        }

        private float CalculateBisectionMethod(float a, float b)
        {
            float solution = (a + b) / 2;
            uint iterationsNumber = 0;

            while (Math.Abs(_commonFunctions.FunctionResult(solution, _operations)) > AppSettings.Epsilon)
            {
                if (_commonFunctions.FunctionResult(a, _operations) * _commonFunctions.FunctionResult(solution, _operations) > 0)
                {
                    a = solution;
                }
                else
                {
                    b = solution;
                }
                solution = (a + b) / 2;
                iterationsNumber++;
            }

            return solution;
        }

        private List<double> GraeffeMethod(int maxIterations)
        {

            for (int i = 0; i < maxIterations-1; i++)
            {
                MultipleFunctions();
                FindSquares();
            }

            MultipleFunctions();
            var roots = new List<double>();

            for (int i = 0 ; i < _operations.Count-2; i++)
            {
                roots.Add(Math.Pow(_operations[i].Value / _operations[i + 1].Value, 1.0 / 2 * maxIterations));
            }

            return roots;
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
            while (Math.Abs(h) >= 0.001)
            {
                h = _commonFunctions.FunctionResult(x, _operations) / _commonFunctions.FunctionResult(x, derivativeOperations);

                x -= h;
            }

            Result = $"{Math.Round(x * 100.0) / 100.0}";
        }
    }
}