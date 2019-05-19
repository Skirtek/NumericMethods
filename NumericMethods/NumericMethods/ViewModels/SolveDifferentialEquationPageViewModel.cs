using System;
using System.Collections.Generic;
using NumericMethods.Enums;
using NumericMethods.Interfaces;
using NumericMethods.Models;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class SolveDifferentialEquationPageViewModel : BaseViewModel, INavigatingAware
    {
        private readonly IExtendedFunctions _extendedFunctions;

        public SolveDifferentialEquationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            IExtendedFunctions extendedFunctions)
            : base(navigationService, pageDialogService)
        {
            _extendedFunctions = extendedFunctions;
        }

        private string _formula;
        public string Formula
        {
            get => _formula;
            set => SetProperty(ref _formula, value);
        }

        private double _argument;
        public double Argument
        {
            get => _argument;
            set => SetProperty(ref _argument, value);
        }

        private PrecisionLevels _precision;
        public PrecisionLevels Precision
        {
            get => _precision;
            set => SetProperty(ref _precision, value);
        }

        private PointModel _point;
        public PointModel Point
        {
            get => _point;
            set => SetProperty(ref _point, value);
        }

        private string _result;
        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        private string _resultEuler;
        public string ResultEuler
        {
            get => _resultEuler;
            set => SetProperty(ref _resultEuler, value);
        }

        private string _step;
        public string Step
        {
            get => _step;
            set => SetProperty(ref _step, value);
        }

        private List<ExtendedOperation> _functionOperations = new List<ExtendedOperation>();

        private double FunctionResult(double x, double y) =>
            _extendedFunctions.FunctionResult((float)x, (float)y, _functionOperations);

        private double CalculateEulerMethod(double x0, double y, double h, double x)
        {
            while (x0 < x)
            {
                y += h * FunctionResult(x0, y);
                x0 += h;
            }

            return y;
        }

        private double CalculateRungeKuttaMethod(double x0, double y0, double h, double x)
        {
            int n = (int)((x - x0) / h);

            double y = y0;

            for (int i = 1; i <= n; i++)
            {
                var k1 = h * FunctionResult(x0, y);

                var k2 = h * FunctionResult(x0 + 0.5 * h, y + 0.5 * k1);

                var k3 = h * FunctionResult(x0 + 0.5 * h, y + 0.5 * k2);

                var k4 = h * FunctionResult(x0 + h, y + k3);

                y += 1.0 / 6.0 * (k1 + 2 * k2 + 2 * k3 + k4);

                x0 += h;
            }

            return y;
        }

        private double GetStepSize()
        {
            switch (Precision)
            {
                case PrecisionLevels.Low:
                    return 0.125;
                case PrecisionLevels.Medium:
                    return 0.025;
                case PrecisionLevels.High:
                    return 0.005;
                default:
                    return 0.025;
            }
        }

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            try
            {
                if (!parameters.TryGetValue(NavParams.Formula, out string formula)
                    || !parameters.TryGetValue(NavParams.Precision, out short precision)
                    || !parameters.TryGetValue(NavParams.Argument, out string argument)
                    || !parameters.TryGetValue(NavParams.InitialValues, out PointModel point))
                {
                    await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                    await NavigationService.GoBackAsync();
                    return;
                }

                Formula = formula;
                Precision = (PrecisionLevels)precision;

                if (!float.TryParse(argument, out float arg))
                {
                    await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                    await NavigationService.GoBackAsync();
                    return;
                }

                Argument = arg;
                Point = point;

                if (point.X == null)
                {
                    point.X = "0";
                }

                if (point.Y == null)
                {
                    point.Y = "0";
                }

                var result = _extendedFunctions.PrepareFunction(Formula);

                if (!result.IsSuccess)
                {
                    await ShowError(HandleResponseCode(result.ResponseCode));
                    return;
                }

                _functionOperations = result.ExtendedOperations;

                var stepSize = GetStepSize();

                Result = $"{CalculateRungeKuttaMethod(double.Parse(point.X), double.Parse(point.Y), stepSize, arg)}";
                ResultEuler = $"{CalculateEulerMethod(double.Parse(point.X), double.Parse(point.Y), stepSize, arg)}";

                Step = $"{stepSize}";
            }
            catch (Exception ex)
            {
                await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                await NavigationService.GoBackAsync();
            }
        }
    }
}
