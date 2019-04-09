using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using NumericMethods.PlatformImplementations;

namespace NumericMethods.Android.Dependencies
{
    public class KeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            var context = MainActivity.Instance;
            if (!(context.GetSystemService(Context.InputMethodService) is InputMethodManager inputMethodManager)
                || context == null)
            {
                return;
            }
            var activity = (Activity)context;
            var token = activity.CurrentFocus?.WindowToken;
            inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
            activity.Window.DecorView.ClearFocus();
        }
    }
}