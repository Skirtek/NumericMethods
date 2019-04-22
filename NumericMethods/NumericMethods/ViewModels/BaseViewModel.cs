﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public abstract class BaseViewModel : ValidationBase, INotifyPropertyChanged
    {
        public IPageDialogService PageDialogService { get; }
        public INavigationService NavigationService { get; }

        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        protected BaseViewModel(IPageDialogService pageDialogService) => PageDialogService = pageDialogService;

        protected BaseViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService) :
            this(pageDialogService) => NavigationService = navigationService;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
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

        protected DelegateCommand<T> GetBusyDependedCommand<T>(Action<T> executeMethod)
            => new DelegateCommand<T>(executeMethod, (T arg) => !IsBusy).ObservesProperty(() => IsBusy);

        protected DelegateCommand GetBusyDependedCommand(Action executeMethod)
            => new DelegateCommand(executeMethod, () => !IsBusy).ObservesProperty(() => IsBusy);

        protected async Task ShowAlert(string title, string message)
            => await PageDialogService.DisplayAlertAsync(title, message, AppResources.Common_Ok);
    }
}