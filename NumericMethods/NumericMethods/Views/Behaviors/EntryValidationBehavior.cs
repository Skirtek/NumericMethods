using System.Linq;
using NumericMethods.Settings;
using NumericMethods.Views.Controls;
using Xamarin.Forms;

namespace NumericMethods.Views.Behaviors
{
    public class EntryValidationBehavior : Behavior<ValidationEntry>
    {
        private ValidationEntry _associatedObject;

        protected override void OnAttachedTo(ValidationEntry bindable)
        {
            base.OnAttachedTo(bindable);

            _associatedObject = bindable;
            _associatedObject.EntryTextChanged += _associatedObject_TextChanged;
        }

        protected override void OnDetachingFrom(ValidationEntry bindable)
        {
            base.OnDetachingFrom(bindable);

            _associatedObject.EntryTextChanged -= _associatedObject_TextChanged;
            _associatedObject = null;
        }

        private void _associatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(_associatedObject.BindingContext is ValidationBase source) ||
                string.IsNullOrEmpty(_associatedObject.PropertyName))
            {
                return;
            }

            var errors = source.GetErrors(_associatedObject.PropertyName).Cast<string>().ToList();
            _associatedObject.EntryTextColor = errors.Any() ? Color.Red : Color.Black;

            _associatedObject.ErrorsText = errors.FirstOrDefault();
        }
    }
}
