using NumericMethods.Models;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    //TODO Wszystkie entries muszą być walidowane
    //TODO Max lenght na własnym poziomie przybliżenia to 7 
    public class IntegralPageViewModel : BaseViewModel
    {
        public IntegralPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            PrecisionChangedCommand = new DelegateCommand(PrecisionChanged);
            SolveIntegralCommand = GetBusyDependedCommand(SolveIntegral);
        }

        #region Properties      
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

        private string _formula;
        public string Formula
        {
            get => _formula;
            set => SetProperty(ref _formula, value);
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
        #endregion
    }
}
