using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using NumericMethods.Android.Dependencies;
using NumericMethods.PlatformImplementations;
using OxyPlot.Xamarin.Forms.Platform.Android;
using Prism;
using Prism.Ioc;

namespace NumericMethods.Android
{
    [Activity(Label = "NumericMethods", Icon = "@mipmap/icon", Theme = "@style/splashscreen", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private static readonly ToastImplementation Toast = new ToastImplementation();
        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.Window.RequestFeature(WindowFeatures.ActionBar);
            base.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            PlotViewRenderer.Init();
            App numericMethods = new App(new AndroidInitializer());
            LoadApplication(numericMethods);
        }

        private class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                containerRegistry.RegisterInstance<IToast>(Toast);
            }
        }
    }
}
