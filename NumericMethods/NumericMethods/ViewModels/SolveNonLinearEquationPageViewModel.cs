using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class SolveNonLinearEquationPageViewModel : BaseViewModel
    {
        public SolveNonLinearEquationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
        }

        private string _result;
        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }


    }
}
