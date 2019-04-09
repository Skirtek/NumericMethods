using NumericMethods.PlatformImplementations;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class LinearEquationPageViewModel : BaseViewModel
    {
        private readonly IKeyboardHelper _keyboardHelper;

        public LinearEquationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            IKeyboardHelper keyboardHelper)
            : base(navigationService, pageDialogService)
        {
            keyboardHelper = _keyboardHelper;
            AppendPatternCommand = GetBusyDependedCommand<string>(AppendPattern);
            RemovePatternCharacterCommand = GetBusyDependedCommand(RemovePatternCharacter);
            GoToSolveEquationPageCommand = GetBusyDependedCommand(GoToSolveEquationPage);
        }

        private string _equationPattern;
        public string EquationPattern
        {
            get => _equationPattern;
            set => SetProperty(ref _equationPattern, value);
        }
        //TODO walidacja czy w równaniu jest znak =
        public DelegateCommand<string> AppendPatternCommand { get; set; }

        public DelegateCommand RemovePatternCharacterCommand { get; set; }

        public DelegateCommand GoToSolveEquationPageCommand { get; set; }

        private void AppendPattern(string text) => EquationPattern += text;

        private void RemovePatternCharacter()
        {
            if (string.IsNullOrEmpty(EquationPattern))
            {
                return;
            }

            EquationPattern = EquationPattern.Remove(EquationPattern.Length - 1);
        }

        private async void GoToSolveEquationPage()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.SolveEquationPage);

            IsBusy = false;
        }
    }
}
