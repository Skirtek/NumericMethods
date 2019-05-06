using System;
using NumericMethods.Models;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace NumericMethods.Views.Controls
{
    public partial class EquationsNumberPopup : PopupPage
    {
        private string Message { get; set; }

        public EquationsNumberPopup(EquationPopupHelper helper)
        {
            InitializeComponent();
            DescriptionLabel.Text = helper.Description;
            EquationsNumberPicker.Title = helper.Placeholder;
            EquationsNumberPicker.ItemsSource = helper.ItemsSource;
            Message = helper.Message;
        }

        private async void OnClosed(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async void OnApply(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, Message, EquationsNumberPicker.SelectedItem);
            await PopupNavigation.Instance.PopAsync();
        }

        public void SetValue(string value)
        {
            EquationsNumberPicker.SelectedItem = value;
        }
    }
}