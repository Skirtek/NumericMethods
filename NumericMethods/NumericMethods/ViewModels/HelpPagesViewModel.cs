using System.Collections.Generic;
using System.Collections.ObjectModel;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class HelpPagesViewModel : BaseViewModel, INavigatingAware
    {
        public HelpPagesViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
        }

        private string _header;
        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private ObservableCollection<string> _steps = new ObservableCollection<string>();
        public ObservableCollection<string> Steps
        {
            get => _steps;
            set => SetProperty(ref _steps, value);
        }

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            if (!parameters.TryGetValue(NavParams.Header, out string header) 
                || !parameters.TryGetValue(NavParams.Description, out string description)
                || !parameters.TryGetValue(NavParams.Steps, out List<string> steps))
            {
                await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                await NavigationService.GoBackAsync();
                return;
            }

            Header = header;
            Description = description;
            Steps = new ObservableCollection<string>(steps);
        }
    }
}
