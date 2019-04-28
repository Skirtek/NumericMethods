using System.Collections.ObjectModel;
using System.Linq;
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
            MaxEquations = 2; //TODO Add popup with choice
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


        private string _result;
        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        private async void ShowHelp()
        {
            var popup = new HelpPopup();

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void GoToSolveEquationPage()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.SolveEquationPage,
                new NavigationParameters
                {
                    { NavParams.Equations, EquationList.ToList() }
                });

            IsBusy = false;
        }

        private void PopulateEquations()
        {
            if (EquationList.Count >= MaxEquations)
            {
                return;
            }

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