using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NumericMethods.Views.Behaviors
{
    public class ViewTappedWithAnimationBehavior : Behavior<View>
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ViewTappedWithAnimationBehavior));
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ViewTappedWithAnimationBehavior));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        private static bool _isAnimating;

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);

            if (bindable.GestureRecognizers.FirstOrDefault() is TapGestureRecognizer exists)
                exists.Tapped += View_Tapped;
        }

        protected override void OnDetachingFrom(View bindable)
        {
            if (bindable.GestureRecognizers.FirstOrDefault() is TapGestureRecognizer exists)
                exists.Tapped -= View_Tapped;

            base.OnDetachingFrom(bindable);
        }

        private async void View_Tapped(object sender, EventArgs e)
        {
            if (Command == null)
            {
                return;
            }

            var resolvedParameter = CommandParameter ?? ((TappedEventArgs)e).Parameter;

            if (!Command.CanExecute(resolvedParameter))
            {
                return;
            }

            if (_isAnimating)
                return;

            _isAnimating = true;

            var view = (View)sender;

            if (view != null)
            {
                await Task.WhenAny(
                    view.ScaleTo(0.9d, 200, Easing.SinIn),
                    view.FadeTo(0.7d, 200, Easing.CubicInOut)
                );

                await Task.WhenAny(
                    view.ScaleTo(1d, 200, Easing.SinIn),
                    view.FadeTo(1d, 200, Easing.CubicInOut)
                );
            }

            if (Command.CanExecute(resolvedParameter))
            {
                Command.Execute(resolvedParameter);
            }
            _isAnimating = false;
        }
    }
}
