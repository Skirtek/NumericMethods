using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NumericMethods.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ValidationEntry : ContentView
    {
        public static readonly BindableProperty EntryTextProperty =
            BindableProperty.Create(nameof(EntryText),
                typeof(string),
                typeof(ValidationEntry),
                default(string), BindingMode.TwoWay);

        public string EntryText
        {
            get => (string)GetValue(EntryTextProperty);
            set => SetValue(EntryTextProperty, value);
        }

        public static readonly BindableProperty MaximumLengthProperty =
            BindableProperty.Create(nameof(MaximumLength),
                typeof(int),
                typeof(ValidationEntry),
                default(int), BindingMode.TwoWay);

        public int MaximumLength
        {
            get => (int)GetValue(MaximumLengthProperty);
            set => SetValue(MaximumLengthProperty, value);
        }

        public string PropertyName { get; set; }

        public static readonly BindableProperty EntryTextColorProperty =
            BindableProperty.Create(nameof(EntryTextColor),
                typeof(Color),
                typeof(ValidationEntry),
                default(Color), BindingMode.TwoWay);

        public Color EntryTextColor
        {
            get => (Color)GetValue(EntryTextColorProperty);
            set => SetValue(EntryTextColorProperty, value);
        }

        public static readonly BindableProperty EntryFontSizeProperty =
            BindableProperty.Create(nameof(EntryFontSize),
                typeof(double),
                typeof(ValidationEntry),
                default(double), BindingMode.TwoWay);

        public double EntryFontSize
        {
            get => (double)GetValue(EntryFontSizeProperty);
            set => SetValue(EntryFontSizeProperty, value);
        }

        public Keyboard Keyboard
        {
            get => Entry.Keyboard;
            set => Entry.Keyboard = value;
        }

        public string ErrorsText
        {
            get => Errors.Text;
            set => Errors.Text = value;
        }

        public ValidationEntry()
        {
            InitializeComponent();

            Entry.Text = EntryText;
            Entry.TextColor = EntryTextColor;
            Entry.FontSize = EntryFontSize;
            Entry.TextChanged += OnTextChanged;
        }

        public event EventHandler<TextChangedEventArgs> EntryTextChanged;

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == EntryTextProperty.PropertyName)
            {
                Entry.Text = EntryText;
            }
            else if (propertyName == MaximumLengthProperty.PropertyName)
            {
                Entry.MaxLength = MaximumLength;
            }
            else if (propertyName == EntryTextColorProperty.PropertyName)
            {
                Entry.TextColor = EntryTextColor;
            }
            else if (propertyName == EntryFontSizeProperty.PropertyName)
            {
                Entry.FontSize = EntryFontSize;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            EntryText = e.NewTextValue;
            EntryTextChanged?.Invoke(sender, e);
        }
    }
}