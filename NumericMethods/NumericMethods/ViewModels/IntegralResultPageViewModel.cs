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
        private string _resultTrapezeMethod;
        public string ResultTrapezeMethod
        {
            get => _resultTrapezeMethod;
            set => SetProperty(ref _resultTrapezeMethod, value);
        }

        private string _resultRectangleMethod;
        public string ResultRectangleMethod
        {
            get => _resultRectangleMethod;
            set => SetProperty(ref _resultRectangleMethod, value);
        }

        private string _upperLimit;
        public string UpperLimit
        {
            get => _upperLimit;
            set => SetProperty(ref _upperLimit, value);
        }

        private string _lowerLimit;
        public string LowerLimit
        {
            get => _lowerLimit;
            set => SetProperty(ref _lowerLimit, value);
        }

        private float _precision;
        public float Precision
        {
            get => _precision;
            set => SetProperty(ref _precision, value);
        }

        private bool Errors { get; set; }

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
                await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                await NavigationService.GoBackAsync();
                return;
            }

            LowerLimit = integral.LowerLimit;
            UpperLimit = integral.UpperLimit;

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
        private float Pole(float lowerLimit, float upperLimit, float precision)
        {
            float height = (upperLimit - lowerLimit) / precision;
            float areas = 0;
            float center = lowerLimit + (upperLimit - lowerLimit) / (2 * precision); 

            for (int i = 0; i < precision; i++)
            {
                areas += FunctionResult(center);
                center += height;  
            }
            return areas * height;
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
            Precision = precision;

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

            ResultTrapezeMethod = $"{integralResult}";
            ResultRectangleMethod = $"{Pole(lowerLimit, upperLimit, precision)}";
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

                if (Errors)
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

            Errors = !float.TryParse(expression, out float value);

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
