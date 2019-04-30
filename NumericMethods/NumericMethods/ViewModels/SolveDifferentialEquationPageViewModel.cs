using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class SolveDifferentialEquationPageViewModel : BaseViewModel
    {
        public SolveDifferentialEquationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService) 
            : base(navigationService, pageDialogService)
        {
            var x = CalculateHeunsMethod(0, 1, 0.025, 20);
        }

        private string _result;
        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        private double CalculateHeunsMethod(double x0, double y0, double h, int number_of_steps)
        {
            return (h * (func(x0, y0) + func(x0 + h, y0 + func(x0, y0) * h))) / 2;
        }

        private double func(double x, double y)
        {
            return (x + y + x * y);
        }
    }
}
