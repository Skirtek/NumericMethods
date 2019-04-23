using System.Collections.ObjectModel;
using NumericMethods.Interfaces;
using NumericMethods.Models;
using NumericMethods.PlatformImplementations;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class LinearEquationPageViewModel : BaseViewModel
    {
        private readonly IMatrix _matrix;
        private readonly IToast _toast;

        public LinearEquationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            IMatrix matrix,
            IToast toast)
            : base(navigationService, pageDialogService)
        {
            GoToSolveEquationPageCommand = GetBusyDependedCommand(GoToSolveEquationPage);
            AddEquationCommand = new DelegateCommand(AddEquation);
            _matrix = matrix;
            _toast = toast;
            MaxEquations = 3; //TODO Add popup with choice
        }

        private ObservableCollection<Equation> _equationList = new ObservableCollection<Equation> { new Equation() };
        public ObservableCollection<Equation> EquationList
        {
            get => _equationList;
            set => SetProperty(ref _equationList, value);
        }

        private short MaxEquations { get; set; }

        public DelegateCommand GoToSolveEquationPageCommand { get; set; }

        public DelegateCommand AddEquationCommand { get; set; }

        private async void GoToSolveEquationPage()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.SolveEquationPage);

            IsBusy = false;
        }

        private void AddEquation()
        {
            var x = _matrix.MatrixDeterminant(new double[,] { { 1, -2, 5 }, { -2, 4, 1 } });
            var y = _matrix.rankOfMatrix(2, 3, new double[,] { { 1, -2, 5 }, { -2, 4, 1 } });
            if (EquationList.Count < MaxEquations)
            {
                EquationList.Add(new Equation());
                return;
            }

            _toast.ShortAlert($"Osiągnięto maksymalną liczbę równań dla { MaxEquations - 1 } niewiadomych");
        }
    }
}
