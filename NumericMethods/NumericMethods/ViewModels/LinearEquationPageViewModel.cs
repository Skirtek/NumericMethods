using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NumericMethods.Models;
using NumericMethods.Resources;
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

        #region Properties
        private ObservableCollection<Equation> _equationList = new ObservableCollection<Equation>();
        public ObservableCollection<Equation> EquationList
        {
            get => _equationList;
            set => SetProperty(ref _equationList, value);
        }

        private short MaxEquations { get; set; }
        #endregion

        #region Delegate Commands
        public DelegateCommand GoToSolveEquationPageCommand { get; set; }

        public DelegateCommand ShowHelpCommand { get; set; }

        public DelegateCommand ChangeEquationsNumberCommand { get; set; }
        #endregion

        #region Private methods
        private async void ShowHelp()
        {
            IsBusy = true;

            var navParams = new NavigationParameters
            {
                { NavParams.Header, AppResources.EquationHelp_Header },
                { NavParams.Description, AppResources.EquationHelp_Description },
                { NavParams.Steps,
                    new List<string>
                    {
                        AppResources.EquationHelp_FirstStep,
                        AppResources.EquationHelp_SecondStep,
                        AppResources.EquationHelp_ThirdStep,
                        AppResources.EquationHelp_FourthStep,
                        AppResources.EquationHelp_FifthStep
                    } }
            };

            await NavigationService.NavigateAsync(NavSettings.HelpPages, navParams);

            IsBusy = false;
        }

        private async void ChangeEquationsNumber()
        {
            var popup = new EquationsNumberPopup(new EquationPopupHelper
            {
                Description = AppResources.EquationsNumberPopup_Description,
                Placeholder = AppResources.EquationsNumberPopup_Placeholder,
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
        #endregion

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            PopulateEquations();
        }
    }
}