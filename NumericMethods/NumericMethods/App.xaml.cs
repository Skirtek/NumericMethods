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
        }

        private void RegisterPagesWithViewModels(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainMenuPage, MainMenuPageViewModel>(NavSettings.MainMenuPage);
            containerRegistry.RegisterForNavigation<LinearChart, LinearChartViewModel>(NavSettings.LinearChart);
            containerRegistry.RegisterForNavigation<LinearEquationPage, LinearEquationPageViewModel>(NavSettings.LinearEquationPage);
            containerRegistry.RegisterForNavigation<SolveEquationPage, SolveEquationPageViewModel>(NavSettings.SolveEquationPage);
        }
        private void RegisterAddOns(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(CrossConnectivity.Current);
            containerRegistry.RegisterInstance(UserSettings.Instance);
        }
        protected override async void OnInitialized()
        {
            InitializeComponent();
            await NavigationService.NavigateAsync(NavSettings.MainMenuPage);
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
