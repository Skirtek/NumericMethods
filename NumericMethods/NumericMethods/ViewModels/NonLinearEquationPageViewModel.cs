using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NumericMethods.Enums;
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
            ShowHelpCommand = new DelegateCommand(ShowHelp);
        }

        private string _formula;
        [Required(ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = nameof(AppResources.Validation_FieldEmpty))]
        [RegularExpression(AppSettings.BasicFormulaRegex, ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = nameof(AppResources.Validation_FormulaIsNotValid))]
        public string Formula
        {
            get => _formula;
            set
            {
                ValidateProperty(value);
                SetProperty(ref _formula, value);
            }
        }

        private short _selectedPrecision = 1;
        public short SelectedPrecision
        {
            get => _selectedPrecision;
            set => SetProperty(ref _selectedPrecision, value);
        }

        public DelegateCommand CalculateNonLinearEquationCommand { get; set; }

        public DelegateCommand ShowHelpCommand { get; set; }

        private async void ShowHelp()
        {
            IsBusy = true;

            var navParams = new NavigationParameters
            {
                { NavParams.Header, AppResources.NonLinearEquationHelp_Header },
                { NavParams.Description, AppResources.NonLinearEquationHelp_Description },
                { NavParams.Steps,
                    new List<string>
                    {
                        AppResources.NonLinearEquationHelp_FirstStep,
                        AppResources.HelpPage_Power,
                        AppResources.NonLinearEquationHelp_ThirdStep,
                        AppResources.NonLinearEquationHelp_FourthStep,
                        AppResources.NonLinearEquationHelp_FifthStep,
                        AppResources.NonLinearEquationHelp_SixthStep
                    } }
            };

            await NavigationService.NavigateAsync(NavSettings.HelpPages, navParams);

            IsBusy = false;
        }

        private async void CalculateNonLinearEquation()
        {
            if (HasErrors)
            {
                return;
            }

            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.SolveNonLinearEquationPage, new NavigationParameters
            {
                { NavParams.Equations, Formula },
                { NavParams.Precision, (PrecisionLevels)SelectedPrecision}
            });

            IsBusy = false;
        }
    }
}
