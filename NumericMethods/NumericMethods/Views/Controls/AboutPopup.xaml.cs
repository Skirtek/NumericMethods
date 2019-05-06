using System;
using NumericMethods.Settings;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace NumericMethods.Views.Controls
{
    public partial class AboutPopup : PopupPage
    {
        public AboutPopup()
        {
            InitializeComponent();
        }

        private void OxyPlotHyperlink_Tapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(AppSettings.OxyPlotWebsite));
        }

        private void IconsHyperlink_Tapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(AppSettings.IconsWebsite));
        }

        private async void OnClosed(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}