using System.Collections.ObjectModel;
using NumericMethods.Models;
using NumericMethods.Settings;
using NumericMethods.Views.Controls;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Services;

namespace NumericMethods.ViewModels
{
    public class LinearEquationPageViewModel : BaseViewModel, INavigatingAware
    {
        //TODO Liczy się tak tak i tak metodą taką i taką pozostawienie niewypełnionego pola oznacza wstawienie 0
        public LinearEquationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            GoToSolveEquationPageCommand = GetBusyDependedCommand(GoToSolveEquationPage);
            ShowHelpCommand = new DelegateCommand(ShowHelp);
            MaxEquations = 3; //TODO Add popup with choice
        }

        private ObservableCollection<Equation> _equationList = new ObservableCollection<Equation>();
        public ObservableCollection<Equation> EquationList
        {
            get => _equationList;
            set => SetProperty(ref _equationList, value);
        }

        private short MaxEquations { get; set; }

        public DelegateCommand GoToSolveEquationPageCommand { get; set; }

        public DelegateCommand ShowHelpCommand { get; set; }

        private async void ShowHelp()
        {
            HelpPopup popup = new HelpPopup();

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void GoToSolveEquationPage()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.SolveEquationPage);

            IsBusy = false;
        }

        private void PopulateEquations()
        {
            for (var i = 0; i < MaxEquations; i++)
            {
                EquationList.Add(new Equation());
            }
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            PopulateEquations();
        }
    }
}
