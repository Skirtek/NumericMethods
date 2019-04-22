using Android.App;
using Android.Widget;
using NumericMethods.PlatformImplementations;

namespace NumericMethods.Android.Dependencies
{
    public class ToastImplementation : IToast
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}