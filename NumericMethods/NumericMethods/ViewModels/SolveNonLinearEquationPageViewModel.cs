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
            CalculateNewtonRaphsonMethod(-20);
            CalculateBisectionMethod(-20, 20);
            //var x = GraeffeMethod(100);
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
            var roots = new List<double>();
            var negated = NegateFunction(_operations);
            var multipled = MultipleFunctions(_operations, negated);
            var squared = FindSquares(MultipleFunctions(_operations, negated));

            for (int i = 0; i < maxIterations; i++)
            {
                multipled = MultipleFunctions(squared, NegateFunction(squared));
                squared = FindSquares(multipled);
            }

            for (int i = multipled.Count; i > 0; i--)
            {
                roots.Add(Math.Pow(Math.Abs((double)multipled[i].Value)/Math.Abs((double)multipled[i-1].Value),1.0/2*maxIterations));
            }

            return roots;
        }

        private List<Operation> FindSquares(List<Operation> operations)
        {
            foreach (var operation in operations)
            {
                operation.Weight /= 2;
            }

            return operations;
        }

        private List<Operation> NegateFunction(List<Operation> operations)
        {
            foreach (var operation in operations)
            {
                if (Math.Abs(operation.Weight % 2) > AppSettings.Epsilon)
                {
                    operation.Value *= -1;
                }
            }

            return operations;
        }

        private List<Operation> MultipleFunctions(List<Operation> firstFunction, List<Operation> secondFunction)
        {
            var operations = new List<Operation>();

            foreach (var operation in firstFunction)
            {
                foreach (var secondOperation in secondFunction)
                {
                    operations.Add(new Operation
                    {
                        Value = operation.Value * secondOperation.Value,
                        Weight = operation.Weight + secondOperation.Weight
                    });
                }
            }
            //TODO konieczne jest grupowanie 
            return operations;
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
