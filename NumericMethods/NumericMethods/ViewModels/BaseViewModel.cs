using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NumericMethods.Resources;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public IPageDialogService PageDialogService { get; }
        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        protected BaseViewModel(IPageDialogService pageDialogService) => PageDialogService = pageDialogService;


        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        protected async Task ShowAlert(string title, string message)
            => await PageDialogService.DisplayAlertAsync(title, message, AppResources.Common_Ok);
    }
}
