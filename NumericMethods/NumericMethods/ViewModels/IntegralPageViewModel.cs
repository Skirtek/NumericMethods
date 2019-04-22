using System.ComponentModel.DataAnnotations;
using NumericMethods.Models;
using NumericMethods.PlatformImplementations;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class IntegralPageViewModel : BaseViewModel
    {
        private readonly IToast _toast;

        public IntegralPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            IToast toast)
            : base(navigationService, pageDialogService)
        {
            PrecisionChangedCommand = new DelegateCommand(PrecisionChanged);
            SolveIntegralCommand = GetBusyDependedCommand(SolveIntegral);
            _toast = toast;
        }

        #region Properties      
        private string _upperLimit;
        [Required(ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = nameof(AppResources.Validation_FieldEmpty))]
        public string UpperLimit
        {
            get => _upperLimit;
            set
            {
                ValidateProperty(value);
                SetProperty(ref _upperLimit, value);
            }
        }

        private string _lowerLimit;
        [Required(ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = nameof(AppResources.Validation_FieldEmpty))]
        public string LowerLimit
        {
            get => _lowerLimit;
            set
            {
                ValidateProperty(value);
                SetProperty(ref _lowerLimit, value);
            }
        }

        private string _formula;
        [Required(ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = nameof(AppResources.Validation_FieldEmpty))]
        [RegularExpression(AppSettings.FormulaRegex, ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = nameof(AppResources.Validation_FormulaIsNotValid))]
        public string Formula
        {
            get => _formula;
            set
            {
                ValidateProperty(value);
                SetProperty(ref _formula, value);
            }
        }

        private string _customPrecision;
        public string CustomPrecision
        {
            get => _customPrecision;
            set => SetProperty(ref _customPrecision, value);
        }

        private short _selectedPrecision;
        public short SelectedPrecision
        {
            get => _selectedPrecision;
            set => SetProperty(ref _selectedPrecision, value);
        }

        private bool _isCustomPrecisionSet;
        public bool IsCustomPrecisionSet
        {
            get => _isCustomPrecisionSet;
            set => SetProperty(ref _isCustomPrecisionSet, value);
        }
        #endregion

        #region Commands
        public DelegateCommand SolveIntegralCommand { get; set; }

        public DelegateCommand PrecisionChangedCommand { get; set; }
        #endregion

        #region Private Methods
        private void PrecisionChanged()
        {
            IsCustomPrecisionSet = SelectedPrecision == 4;
        }

        private async void SolveIntegral()
        {
            if (HasErrors || !ValidateProperties())
            {
                return;
            }

            IsBusy = true;

            var integral = new Integral
            {
                LowerLimit = LowerLimit,
                SelectedPrecision = SelectedPrecision,
                UpperLimit = UpperLimit,
                CustomPrecision = CustomPrecision,
                Formula = Formula
            };

            await NavigationService.NavigateAsync(NavSettings.IntegralResultPage, new NavigationParameters { { NavParams.Integral, integral } });

            IsBusy = false;
        }

        private bool ValidateProperties()
        {

            if (!float.TryParse(LowerLimit, out float lowerLimit) || !float.TryParse(UpperLimit, out float upperLimit))
            {
                _toast.ShortAlert(AppResources.Common_SomethingWentWrong);
                return false;
            }


            if (lowerLimit > upperLimit)
            {
                _toast.ShortAlert(AppResources.Validation_InvalidBorder);
                return false;
            }

            if (IsCustomPrecisionSet && string.IsNullOrEmpty(CustomPrecision))
            {
                _toast.ShortAlert(AppResources.Validation_CustomPrecisionIsEmpty);
                return false;
            }

            return true;
        }
        #endregion
    }
}
