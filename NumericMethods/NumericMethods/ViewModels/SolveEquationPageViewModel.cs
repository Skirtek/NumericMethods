using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class SolveEquationPageViewModel : BaseViewModel
    {
        public SolveEquationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService) 
            : base(navigationService, pageDialogService)
        {
            GoToLinearChartPageCommand = GetBusyDependedCommand(GoToLinearChartPage);
        }

        public DelegateCommand GoToLinearChartPageCommand { get; set; }

        private async void GoToLinearChartPage()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.LinearChart);

            IsBusy = false;
        }
    }
}
