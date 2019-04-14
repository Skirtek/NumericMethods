using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace NumericMethods.Views.Behaviors
{
    public class SelectedPrecisionChanged : Behavior<Picker>
    {
        private Picker _associatedObject;
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command),
                typeof(ICommand),
                typeof(SelectedPrecisionChanged),
                default(ICommand));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttachedTo(Picker bindable)
        {
            base.OnAttachedTo(bindable);

            _associatedObject = bindable;
            _associatedObject.SelectedIndexChanged += Picker_OnSelectedIndexChanged;
        }

        protected override void OnDetachingFrom(Picker bindable)
        {
            base.OnDetachingFrom(bindable);

            _associatedObject.SelectedIndexChanged -= Picker_OnSelectedIndexChanged;
            _associatedObject = null;
        }

        private void Picker_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var parameter = ((Picker) sender).SelectedIndex;

            if (Command == null || !Command.CanExecute(parameter))
            {
                return;
            }

            Command.Execute(parameter);
        }
    }
}
