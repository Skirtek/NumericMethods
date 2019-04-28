using NumericMethods.Models;
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

        public static readonly BindableProperty ZValueProperty =
            BindableProperty.Create(nameof(ZValue),
                typeof(string),
                typeof(LinearEquationControl),
                default(string), BindingMode.TwoWay);

        public string ZValue
        {
            get => (string)GetValue(ZValueProperty);
            set => SetValue(ZValueProperty, value);
        }

        public static readonly BindableProperty TValueProperty =
            BindableProperty.Create(nameof(TValue),
                typeof(string),
                typeof(LinearEquationControl),
                default(string), BindingMode.TwoWay);

        public string TValue
        {
            get => (string)GetValue(TValueProperty);
            set => SetValue(TValueProperty, value);
        }

        public static readonly BindableProperty EquationSizeProperty =
            BindableProperty.Create(nameof(EquationsSize),
                typeof(EquationSize),
                typeof(LinearEquationControl),
                default(EquationSize), BindingMode.TwoWay);

        public EquationSize EquationsSize
        {
            get => (EquationSize)GetValue(EquationSizeProperty);
            set => SetValue(EquationSizeProperty, value);
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