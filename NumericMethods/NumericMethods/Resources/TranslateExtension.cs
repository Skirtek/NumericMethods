using System;
using System.Reflection;
using System.Resources;
using Plugin.Multilingual;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NumericMethods.Resources
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        const string ResourceId = "NumericMethods.Resources.AppResources";

        private static readonly Lazy<ResourceManager> ResManager = new Lazy<ResourceManager>(() =>
            new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly));

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return string.Empty;

            var ci = CrossMultilingual.Current.CurrentCultureInfo;

            var translation = ResManager.Value.GetString(Text, ci) ?? Text;

            return translation;
        }
    }
}