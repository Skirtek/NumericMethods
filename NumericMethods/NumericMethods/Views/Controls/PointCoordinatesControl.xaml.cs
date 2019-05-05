using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NumericMethods.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PointCoordinatesControl : ContentView
    {
        public static readonly BindableProperty YValueProperty =
            BindableProperty.Create(nameof(YValue),
                typeof(string),
                typeof(LinearEquationControl),
                default(string), BindingMode.TwoWay);

        public string YValue
        {
            get => (string)GetValue(YValueProperty);
            set => SetValue(YValueProperty, value);
        }

        public static readonly BindableProperty XValueProperty =
            BindableProperty.Create(nameof(XValue),
                typeof(string),
                typeof(LinearEquationControl),
                default(string), BindingMode.TwoWay);

        public string XValue
        {
            get => (string)GetValue(XValueProperty);
            set => SetValue(XValueProperty, value);
        }

        public PointCoordinatesControl()
        {
            InitializeComponent();
        }
    }
}