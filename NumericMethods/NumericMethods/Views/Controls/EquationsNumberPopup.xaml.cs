using System;
using NumericMethods.Settings;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace NumericMethods.Views.Controls
{
    public partial class EquationsNumberPopup : PopupPage
    {
        public EquationsNumberPopup()
        {
            InitializeComponent();
        }

        private async void OnClosed(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async void OnApply(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, AppSettings.ChangeEquationNumber, EquationsNumberPicker.SelectedItem);
            await PopupNavigation.Instance.PopAsync();
        }

        public void SetValue(string value)
        {
            EquationsNumberPicker.SelectedItem = value;
        }
    }
}