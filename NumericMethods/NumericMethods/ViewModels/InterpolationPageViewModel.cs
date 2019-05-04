using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class InterpolationPageViewModel : BaseViewModel
    {
        public InterpolationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            GoToSolveInterpolationPageCommand = new DelegateCommand(GoToSolveInterpolationPage);
        }

        public DelegateCommand GoToSolveInterpolationPageCommand { get; set; }

        private async void GoToSolveInterpolationPage()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.SolveInterpolationPage);

            IsBusy = false;
        }
    }
}
