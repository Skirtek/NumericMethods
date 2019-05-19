using NumericMethods.Interfaces;
using NumericMethods.Services;
using NumericMethods.Settings;
using NumericMethods.ViewModels;
using NumericMethods.Views;
using Plugin.Connectivity;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace NumericMethods
{
    public partial class App : PrismApplication
    {

        public App(IPlatformInitializer initializer = null) : base(initializer) { }
        public App() => InitializeComponent();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterPagesWithViewModels(containerRegistry);
            RegisterAddOns(containerRegistry);
            RegisterServicesAndUtils(containerRegistry);
        }

        private void RegisterPagesWithViewModels(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainMenuPage, MainMenuPageViewModel>(NavSettings.MainMenuPage);
            containerRegistry.RegisterForNavigation<LinearChart, LinearChartViewModel>(NavSettings.LinearChart);
            containerRegistry.RegisterForNavigation<LinearEquationPage, LinearEquationPageViewModel>(NavSettings.LinearEquationPage);
            containerRegistry.RegisterForNavigation<SolveEquationPage, SolveEquationPageViewModel>(NavSettings.SolveEquationPage);
            containerRegistry.RegisterForNavigation<IntegralPage, IntegralPageViewModel>(NavSettings.IntegralPage);
            containerRegistry.RegisterForNavigation<IntegralResultPage, IntegralResultPageViewModel>(NavSettings.IntegralResultPage);
            containerRegistry.RegisterForNavigation<DifferentialEquationPage, DifferentialEquationPageViewModel>(NavSettings.DifferentialEquationPage);
            containerRegistry.RegisterForNavigation<SolveDifferentialEquationPage, SolveDifferentialEquationPageViewModel>(NavSettings.SolveDifferentialEquationPage);
            containerRegistry.RegisterForNavigation<NonLinearEquationPage, NonLinearEquationPageViewModel>(NavSettings.NonLinearEquationPage);
            containerRegistry.RegisterForNavigation<SolveNonLinearEquationPage, SolveNonLinearEquationPageViewModel>(NavSettings.SolveNonLinearEquationPage);
            containerRegistry.RegisterForNavigation<NonLinearChartPage, NonLinearChartPageViewModel>(NavSettings.NonLinearChartPage);
            containerRegistry.RegisterForNavigation<InterpolationPage, InterpolationPageViewModel>(NavSettings.InterpolationPage);
            containerRegistry.RegisterForNavigation<SolveInterpolationPage, SolveInterpolationPageViewModel>(NavSettings.SolveInterpolationPage);
            containerRegistry.RegisterForNavigation<InterpolationChartPage, InterpolationChartPageViewModel>(NavSettings.InterpolationChartPage);
            containerRegistry.RegisterForNavigation<EquationHelpPage, HelpPagesViewModel>(NavSettings.EquationHelpPage);
            containerRegistry.RegisterForNavigation<NonLinearEquationHelpPage, HelpPagesViewModel>(NavSettings.NonLinearEquationHelpPage);
            containerRegistry.RegisterForNavigation<InterpolationHelpPage, HelpPagesViewModel>(NavSettings.InterpolationHelpPage);
            containerRegistry.RegisterForNavigation<IntegralHelpPage, HelpPagesViewModel>(NavSettings.IntegralHelpPage);
        }
        private void RegisterAddOns(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(CrossConnectivity.Current);
            containerRegistry.RegisterInstance(UserSettings.Instance);
        }

        private void RegisterServicesAndUtils(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IMatrix, Matrix>();
            containerRegistry.Register<IEquation, EquationService>();
            containerRegistry.Register<ICommonFunctions, CommonFunctions>();
            containerRegistry.Register<IExtendedFunctions, ExtendedFunctions>();
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            await NavigationService.NavigateAsync($"NavigationPage/{NavSettings.MainMenuPage}");
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
