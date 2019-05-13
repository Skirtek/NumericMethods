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
    public class IntegralResultPageViewModel : BaseViewModel, INavigatedAware
    {
        private readonly ICommonFunctions _commonFunctions;

        #region CTOR

        public IntegralResultPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            ICommonFunctions commonFunctions)
            : base(navigationService, pageDialogService)
        {
            _commonFunctions = commonFunctions;
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

        private List<Operation> _operations = new List<Operation>();

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

            IsBusy = true;

            LowerLimit = integral.LowerLimit;
            UpperLimit = integral.UpperLimit;

            await CalculateIntegral(integral);

            IsBusy = false;
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

        private float Area(float lowerLimit, float upperLimit, float precision)
        {
            float height = (upperLimit - lowerLimit) / precision;
            float areas = 0;
            float center = lowerLimit + (upperLimit - lowerLimit) / (2 * precision);

            for (int i = 0; i < precision; i++)
            {
                areas += _commonFunctions.FunctionResult(center, _operations);
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

            var result = _commonFunctions.PrepareFunction(integral.Formula);

            if (!result.IsSuccess)
            {
                await ShowError(HandleResponseCode(result.ResponseCode));
                return;
            }

            _operations = result.Operations;

            for (var i = 1; i < precision; i++)
            {
                integralResult += _commonFunctions.FunctionResult(lowerLimit + i * height, _operations);
            }

            integralResult += _commonFunctions.FunctionResult(lowerLimit,_operations) / 2;
            integralResult += _commonFunctions.FunctionResult(upperLimit, _operations) / 2;
            integralResult *= height;

            ResultTrapezeMethod = $"{integralResult}";
            ResultRectangleMethod = $"{Area(lowerLimit, upperLimit, precision)}";
        }
        #endregion
    }
}