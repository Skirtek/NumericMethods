using System.ComponentModel.DataAnnotations;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class NonLinearEquationPageViewModel : BaseViewModel
    {
        public NonLinearEquationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            CalculateNonLinearEquationCommand = new DelegateCommand(CalculateNonLinearEquation);
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

        public DelegateCommand CalculateNonLinearEquationCommand { get; set; }

        private async void CalculateNonLinearEquation()
        {
            if (HasErrors)
            {
                return;
            }

            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.SolveNonLinearEquationPage, new NavigationParameters { { NavParams.Equations, Formula } });

            IsBusy = false;
        }
    }
}
