using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NumericMethods.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LinearEquationControl : ContentView
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

        public static readonly BindableProperty SelectedSignProperty =
            BindableProperty.Create(nameof(SelectedSign),
                typeof(short),
                typeof(LinearEquationControl),
                default(short), BindingMode.TwoWay);

        public short SelectedSign
        {
            get => (short)GetValue(SelectedSignProperty);
            set => SetValue(SelectedSignProperty, value);
        }

        public static readonly BindableProperty ConstantTermProperty =
            BindableProperty.Create(nameof(ConstantTerm),
                typeof(string),
                typeof(LinearEquationControl),
                default(string), BindingMode.TwoWay);

        public string ConstantTerm
        {
            get => (string)GetValue(ConstantTermProperty);
            set => SetValue(ConstantTermProperty, value);
        }

        public LinearEquationControl()
        {
            InitializeComponent();
        }
    }
}