using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NumericMethods.Models;
using NumericMethods.Resources;
using NumericMethods.Settings;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class InterpolationPageViewModel : BaseViewModel
    {
        public InterpolationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            GoToSolveInterpolationPageCommand = new DelegateCommand(GoToSolveInterpolationPage);
            AddPointCommand = new DelegateCommand(AddPoint);
            ShowHelpCommand = new DelegateCommand(ShowHelp);
        }

        private ObservableCollection<PointModel> _pointsList = new ObservableCollection<PointModel> { new PointModel(), new PointModel(), new PointModel() };
        public ObservableCollection<PointModel> PointsList
        {
            get => _pointsList;
            set => SetProperty(ref _pointsList, value);
        }

        private string _argument;
        [Required(ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = nameof(AppResources.Validation_FieldEmpty))]
        public string Argument
        {
            get => _argument;
            set
            {
                ValidateProperty(value);
                SetProperty(ref _argument, value);
            }
        }

        public DelegateCommand AddPointCommand { get; set; }

        public DelegateCommand GoToSolveInterpolationPageCommand { get; set; }

        public DelegateCommand ShowHelpCommand { get; set; }

        private async void ShowHelp()
        {
            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.InterpolationHelpPage);

            IsBusy = false;
        }

        private async void GoToSolveInterpolationPage()
        {
            if (HasErrors)
            {
                return;
            }

            IsBusy = true;

            await NavigationService.NavigateAsync(NavSettings.SolveInterpolationPage, new NavigationParameters
            {
                { NavParams.Points, PointsList.ToList() },
                { NavParams.Argument, Argument }
            });

            IsBusy = false;
        }

        private void AddPoint()
        {
            PointsList.Add(new PointModel());
        }
    }
}
