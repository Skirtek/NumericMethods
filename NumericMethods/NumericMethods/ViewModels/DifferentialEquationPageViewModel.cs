using System.ComponentModel.DataAnnotations;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class DifferentialEquationPageViewModel : BaseViewModel
    {
        public DifferentialEquationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            CalculateDifferentialCommand = new DelegateCommand(CalculateDifferential);
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

        public DelegateCommand CalculateDifferentialCommand { get; set; }

        private async void CalculateDifferential()
        {
            if (HasErrors)
            {
                return;
            }

            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.SolveDifferentialEquationPage);

            IsBusy = false;
        }
    }
}
