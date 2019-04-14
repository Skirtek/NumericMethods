using NumericMethods.Models;
using NumericMethods.Settings;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class IntegralResultPageViewModel : BaseViewModel, INavigatedAware
    {
        public IntegralResultPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService) 
            : base(navigationService, pageDialogService)
        {
        }

        private string _upperLimit;
        public string UpperLimit
        {
            get => _upperLimit;
            set => SetProperty(ref _upperLimit, value);
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!parameters.TryGetValue(NavParams.Integral, out Integral integral))
            {
                await ShowAlert("Ups!", "Stało się coś złego");
                await NavigationService.GoBackAsync();
                return;
            }

            UpperLimit = integral.UpperLimit;
        }
    }
}
