using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NumericMethods.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuItemRow : ContentView
    {
        public static readonly BindableProperty LabelTextProperty =
            BindableProperty.Create(nameof(LabelText),
                typeof(string),
                typeof(MenuItemRow),
                default(string),
                propertyChanged: (bindable, oldVal, newVal) =>
                {
                    ((MenuItemRow)bindable).MenuLabel.Text = (string)newVal;
                });

        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command),
                typeof(ICommand),
                typeof(MenuItemRow),
                default(ICommand));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter),
                typeof(object),
                typeof(MenuItemRow));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly BindableProperty MenuIconProperty =
            BindableProperty.Create(nameof(MenuIcon),
                typeof(FileImageSource),
                typeof(MenuItemRow),
                default(FileImageSource),
                propertyChanged: (bindable, oldVal, newVal) =>
                {
                    ((MenuItemRow)bindable).MenuIconName.Source = (FileImageSource)newVal;
                });

        public FileImageSource MenuIcon
        {
            get => (FileImageSource)GetValue(MenuIconProperty);
            set => SetValue(MenuIconProperty, value);
        }

        public MenuItemRow()
        {
            InitializeComponent();
        }
    }
}