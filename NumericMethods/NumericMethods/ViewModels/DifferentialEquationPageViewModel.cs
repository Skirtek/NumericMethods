using System.ComponentModel.DataAnnotations;
using NumericMethods.Models;
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
        [RegularExpression(AppSettings.ExtendedFormulaRegex, ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = nameof(AppResources.Validation_FormulaIsNotValid))]
        public string Formula
        {
            get => _formula;
            set
            {
                ValidateProperty(value);
                SetProperty(ref _formula, value);
            }
        }

        private string _argument;
        [Required(ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = nameof(AppResources.Validation_FieldEmpty))]
        public string Argument
        {
            get => _argument;
            set
            {
                ValidateProperty(value);
                SetProperty(ref _argument, value);
            }
        }

        private short _selectedPrecision = 1;
        public short SelectedPrecision
        {
            get => _selectedPrecision;
            set => SetProperty(ref _selectedPrecision, value);
        }

        private PointModel _initialValues;

        public PointModel InitialValues
        {
            get => _initialValues;
            set => SetProperty(ref _initialValues, value);
        }

        public DelegateCommand CalculateDifferentialCommand { get; set; }

        private async void CalculateDifferential()
        {
            if (HasErrors)
            {
                return;
            }

            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.SolveDifferentialEquationPage,
                new NavigationParameters
                {
                    { NavParams.Formula, Formula },
                    { NavParams.Argument, Argument },
                    { NavParams.Precision, SelectedPrecision },
                    { NavParams.InitialValues, InitialValues }
                });

            IsBusy = false;
        }
    }
}
