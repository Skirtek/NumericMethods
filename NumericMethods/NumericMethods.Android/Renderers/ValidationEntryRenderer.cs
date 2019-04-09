using Android.Content;
using NumericMethods.Android.Renderers;
using NumericMethods.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ValidationEntry), typeof(ValidationEntryRenderer))]
namespace NumericMethods.Android.Renderers
{
    public class ValidationEntryRenderer : EditorRenderer
    {
        public ValidationEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            Control?.SetBackgroundColor(global::Android.Graphics.Color.Transparent);
        }
    }
}