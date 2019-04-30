using System.Threading.Tasks;
using NumericMethods.Resources;
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
            NavigateToPageCommand = GetBusyDependedCommand<string>(NavigateToPage);
        }

        public DelegateCommand<string> NavigateToPageCommand { get; set; }

        public DelegateCommand AboutAppCommand { get; set; }

        private async void NavigateToPage(string page)
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(page);

            IsBusy = false;
        }

        private async Task AboutApp()
        {
            await ShowAlert(AppResources.Menu_AboutApp, AppResources.Menu_AboutAppDescription);
        }
    }
}
