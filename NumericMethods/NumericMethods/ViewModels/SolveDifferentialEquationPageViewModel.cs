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
            var y = euler(0, 1, 0.025, 0.1);
            var z = RungeKutta(0, 1, 0.1, 0.025);
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

        private List<ExtendedOperation> _extendedOperations = new List<ExtendedOperation>();

        private double func(double x, double y)
        {
            return 2 * x - 4 * y;
        }

        private double euler(double x0, double y, double h, double x)
        {
            while (x0 < x)
            {
                y += h * func(x0, y);
                x0 += h;
            }

            return y;
        }

        private double RungeKutta(double x0, double y0, double x, double h)
        {
            int n = (int)((x - x0) / h);

            double y = y0;

            for (int i = 1; i <= n; i++)
            {
                var k1 = h * func(x0, y);

                var k2 = h * func(x0 + 0.5 * h, y + 0.5 * k1);

                var k3 = h * func(x0 + 0.5 * h, y + 0.5 * k2);

                var k4 = h * func(x0 + h, y + k3);

                y += 1.0 / 6.0 * (k1 + 2 * k2 + 2 * k3 + k4);

                x0 += h;
            }

            return y;
        }

        public async void OnNavigatingTo(INavigationParameters parameters)
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

            var result = _extendedFunctions.PrepareFunction(Formula);
            if (!result.IsSuccess)
            {
                await ShowError(HandleResponseCode(result.ResponseCode));
                return;
            }

            _extendedOperations = result.ExtendedOperations;

        }
    }
}
