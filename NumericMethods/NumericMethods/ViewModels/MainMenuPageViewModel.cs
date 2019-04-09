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
        public MainMenuPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            AboutAppCommand = new DelegateCommand(async () => await AboutApp());
            GoToLinearEquationPageCommand = new DelegateCommand(async () => await GoToLinearEquationPage());
        }

        public DelegateCommand GoToLinearEquationPageCommand { get; set; }
        public DelegateCommand AboutAppCommand { get; set; }

        private async Task GoToLinearEquationPage()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.LinearEquationPage);

            IsBusy = false;
        }

        private async Task AboutApp()
        {
            await ShowAlert(AppResources.Menu_AboutApp, AppResources.Menu_AboutAppDescription);
        }
    }
}
