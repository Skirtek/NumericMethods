using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NumericMethods.Enums;
using NumericMethods.Models;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class IntegralResultPageViewModel : BaseViewModel, INavigatedAware
    {
        #region CTOR
        public IntegralResultPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
        }
        #endregion

        #region Properties
        private string _result;
        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        private bool HasErrors { get; set; }

        private List<Operation> Operations = new List<Operation>();
        #endregion

        #region OnNavigating*
        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!parameters.TryGetValue(NavParams.Integral, out Integral integral))
            {
                await ShowAlert("Ups!", "Stało się coś złego");
                await NavigationService.GoBackAsync();
                return;
            }

            await CalculateIntegral(integral);
        }
        #endregion

        #region Private Methods
        private float GetPrecision(Integral integral)
        {
            var precision = (PrecisionLevels)integral.SelectedPrecision;
            switch (precision)
            {
                case PrecisionLevels.Low:
                    return 100;
                case PrecisionLevels.Medium:
                    return 500;
                case PrecisionLevels.High:
                    return 1000;
                case PrecisionLevels.VeryHigh:
                    return 10000;
                case PrecisionLevels.Custom:
                    bool canParse = float.TryParse(integral.CustomPrecision, out float value);
                    return canParse ? value : 100;
                default:
                    return 100;
            }
        }

        private async Task CalculateIntegral(Integral integral)
        {
            if (!float.TryParse(integral.LowerLimit, out float lowerLimit) ||
                !float.TryParse(integral.UpperLimit, out float upperLimit))
            {
                await ShowError(AppResources.Common_SomethingWentWrong);
                return;
            }

            float precision = GetPrecision(integral);

            float height = (upperLimit - lowerLimit) / precision;
            float integralResult = 0;

            await PrepareFunction(integral.Formula);

            for (var i = 1; i < precision; i++)
            {
                integralResult += FunctionResult(lowerLimit + i * height);
            }

            integralResult += FunctionResult(lowerLimit) / 2;
            integralResult += FunctionResult(upperLimit) / 2;
            integralResult *= height;

            Result = $"{integralResult}";
        }

        private float FunctionResult(float x)
        {
            float result = 0;
            foreach (var operation in Operations)
            {
                if (operation.IsNegative)
                {
                    result -= operation.Value * (float)Math.Pow(x, operation.Weight);
                }
                else
                {
                    result += operation.Value * (float)Math.Pow(x, operation.Weight);
                }
            }

            return result;
        }

        private async Task PrepareFunction(string formula)
        {
            try
            {
                string lastExpression = Regex.Match(formula, AppSettings.ConstantTermRegex, RegexOptions.RightToLeft).ToString();

                if (!string.IsNullOrWhiteSpace(lastExpression))
                {
                    Operations.Add(new Operation
                    {
                        IsNegative = IsNegative(lastExpression),
                        Value = GetValue(lastExpression),
                        Weight = 0
                    });
                }

                var expressions = Regex.Matches(formula, AppSettings.ArgumentRegex);

                foreach (var expression in expressions)
                {
                    string exp = expression.ToString();

                    if (exp.Contains("^"))
                    {
                        var split = exp.Split('^');
                        string part = split[0];

                        if (!int.TryParse(split[1], out int weight))
                        {
                            await ShowError(AppResources.Common_WrongFunction);
                            return;
                        }

                        Operations.Add(new Operation
                        {
                            IsNegative = IsNegative(part),
                            Value = part.Equals("x") || part.Equals("X") ? 1 : GetValue(part.Remove(part.Length - 1, 1)),
                            Weight = weight
                        });
                    }
                    else
                    {
                        Operations.Add(new Operation
                        {
                            IsNegative = IsNegative(exp),
                            Value = exp.Equals("x") || exp.Equals("X") ? 1 : GetValue(exp.Remove(exp.Length - 1, 1)),
                            Weight = 1
                        });
                    }
                }

                if (HasErrors)
                {
                    await ShowError(AppResources.Common_WrongFunction);
                }
            }
            catch (Exception)
            {
                await ShowError(AppResources.Common_SomethingWentWrong);
            }
        }

        private bool IsNegative(string expression) => expression.Substring(0, 1).Equals("-");

        private float GetValue(string expression)
        {
            if (expression.Substring(0, 1).Equals("-") || expression.Substring(0, 1).Equals("+"))
            {
                expression = expression.Remove(0, 1);
            }

            if (expression.Contains("."))
            {
                expression = expression.Replace(".", ",");
            }

            HasErrors = !float.TryParse(expression, out float value);

            return value;
        }

        private async Task ShowError(string message)
        {
            await ShowAlert(AppResources.Common_Ups, message);
            await NavigationService.GoBackAsync();
        }
        #endregion
    }
}
