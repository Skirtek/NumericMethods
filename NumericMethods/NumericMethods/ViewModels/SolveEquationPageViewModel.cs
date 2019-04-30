using System;
using System.Collections.Generic;
using NumericMethods.Interfaces;
using NumericMethods.Models;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class SolveEquationPageViewModel : BaseViewModel, INavigatingAware
    {
        private readonly IEquation _equation;

        public SolveEquationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            IEquation equation)
            : base(navigationService, pageDialogService)
        {
            _equation = equation;
            GoToLinearChartPageCommand = GetBusyDependedCommand(GoToLinearChartPage);
        }

        private List<Equation> EquationsList { get; set; }

        private bool _isLinearEquation;
        public bool IsLinearEquation
        {
            get => _isLinearEquation;
            set => SetProperty(ref _isLinearEquation, value);
        }

        public DelegateCommand GoToLinearChartPageCommand { get; set; }

        private string _result;
        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        private async void GoToLinearChartPage()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.LinearChart, new NavigationParameters { { NavParams.Equations, EquationsList } });

            IsBusy = false;
        }

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            try
            {
                if (!parameters.TryGetValue(NavParams.Equations, out List<Equation> equations))
                {
                    await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                    await NavigationService.GoBackAsync();
                    return;
                }

                EquationsList = equations;

                IsLinearEquation = EquationsList.Count == 2;

                switch (EquationsList.Count)
                {
                    case 2:
                        Result = _equation.TwoVariablesEquation(EquationsList);
                        break;
                    case 3:
                        Result = _equation.ThreeVariablesEquation(EquationsList);
                        break;
                    case 4:
                        Result = _equation.FourVariablesEquation(EquationsList);
                        break;
                }
            }
            catch (Exception)
            {
                await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                await NavigationService.GoBackAsync();
            }
        }
    }
}