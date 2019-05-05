using System;
using System.Collections.Generic;
using NumericMethods.Interfaces;
using NumericMethods.Models;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class ApproximationPageViewModel : BaseViewModel
    {
        public ApproximationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            GoToSolveApproximationPageCommand = new DelegateCommand(GoToSolveApproximationPage);

        }

        public DelegateCommand GoToSolveApproximationPageCommand { get; set; }

        private async void GoToSolveApproximationPage()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.SolveApproximationPage);

            IsBusy = false;
        }
    }
}
