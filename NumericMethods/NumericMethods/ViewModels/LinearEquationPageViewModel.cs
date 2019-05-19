using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NumericMethods.Models;
using NumericMethods.Settings;
using NumericMethods.Views.Controls;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

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
            ChangeEquationsNumberCommand = new DelegateCommand(ChangeEquationsNumber);
            MaxEquations = 2;
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

        public DelegateCommand ChangeEquationsNumberCommand { get; set; }

        private async void ShowHelp()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.EquationHelpPage);

            IsBusy = false;
        }

        private async void ChangeEquationsNumber()
        {
            var popup = new EquationsNumberPopup(new EquationPopupHelper
            {
                Description = "Wybierz ilość niewiadomych w równaniach",
                Placeholder = "Ilość niewiadomych",
                ItemsSource = new List<string> { "2", "3", "4" },
                Message = AppSettings.ChangeEquationNumber
            });

            popup.SetValue(MaxEquations.ToString());
            await PopupNavigation.Instance.PushAsync(popup);

            MessagingCenter.Subscribe<EquationsNumberPopup, object>(this, AppSettings.ChangeEquationNumber, (sender, arg) =>
            {
                short.TryParse((string)arg, out short maxEquations);
                if (maxEquations == MaxEquations)
                {
                    return;
                }

                MaxEquations = maxEquations;

                PopulateEquations();
            });
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
            EquationList.Clear();

            for (var i = 0; i < MaxEquations; i++)
            {
                EquationList.Add(new Equation { EquationCount = (EquationSize)MaxEquations });
            }
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            PopulateEquations();
        }
    }
}