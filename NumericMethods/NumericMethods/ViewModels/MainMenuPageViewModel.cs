using System.Threading.Tasks;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class MainMenuPageViewModel : BaseViewModel
    {
        public MainMenuPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            AboutAppCommand = new DelegateCommand(async () => await AboutApp());
            GoToLinearChartCommand = new DelegateCommand(async () => await GoToLinearChart());
        }

        public DelegateCommand GoToLinearChartCommand { get; set; }
        public DelegateCommand AboutAppCommand { get; set; }

        private async Task GoToLinearChart()
        {
            IsBusy = true;
            await NavigationService.NavigateAsync(NavSettings.LinearChart);
            IsBusy = false;
        }

        private async Task AboutApp()
        {
            await ShowAlert(AppResources.Menu_AboutApp, "Twórcami aplikacji są");
        }
    }
}
