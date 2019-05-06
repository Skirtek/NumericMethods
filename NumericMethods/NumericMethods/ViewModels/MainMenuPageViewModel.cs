using System.Threading.Tasks;
using NumericMethods.Views.Controls;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Services;

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
            var popup = new AboutPopup();

            await PopupNavigation.Instance.PushAsync(popup);
        }
    }
}
