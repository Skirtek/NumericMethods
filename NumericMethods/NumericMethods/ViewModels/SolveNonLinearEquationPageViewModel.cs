using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NumericMethods.Enums;
using NumericMethods.Interfaces;
using NumericMethods.Models;
using NumericMethods.Resources;
using NumericMethods.Settings;
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

        private List<Operation> _operations = new List<Operation>();

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            await Calculate();
        }

        private async Task Calculate()
        {
            //TODO dodać przekazywanie realnej funkcji
            var result = _commonFunctions.PrepareFunction("x^3-x^2+2");

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
            var x = Sieczne(-100, 100);
        }

        //TODO Nazwać to zgodnie z konwencją
        private float Sieczne(float x1, float x2)
        {
            float x0 = 0;
            float f1 = _commonFunctions.FunctionResult(x1,_operations);
            float f2 = _commonFunctions.FunctionResult(x2, _operations);
            int i = 64;
            while (i > 0 && Math.Abs(x1 - x2) > AppSettings.Epsilon)
            {
                if (Math.Abs(f1 - f2) < AppSettings.Epsilon)
                {
                    break;
                }

                x0 = x1 - f1 * (x1 - x2) / (f1 - f2);
                float f0 = _commonFunctions.FunctionResult(x0, _operations);
                if (Math.Abs(f0) < AppSettings.Epsilon)
                {
                    break;
                }

                x2 = x1; f2 = f1;
                x1 = x0; f1 = f0;
            }

            return x0;
        }

        private void CalculateNewtonRaphsonMethod(float x)
        {
            var derivativeOperations = _commonFunctions.CalculateDerivative(_operations);

            float h = _commonFunctions.FunctionResult(x, _operations) / _commonFunctions.FunctionResult(x, derivativeOperations);
            while (Math.Abs(h) >= AppSettings.Epsilon)
            {
                h = _commonFunctions.FunctionResult(x, _operations) / _commonFunctions.FunctionResult(x, derivativeOperations);

                x = x - h;
            }

            Result = $"{Math.Round(x * 100.0) / 100.0}";
        }
    }
}
