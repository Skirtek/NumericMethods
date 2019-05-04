using System;
using System.Collections.Generic;
using NumericMethods.Models;
using NumericMethods.Resources;
using NumericMethods.Settings;
using NumericMethods.Views.Controls;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace NumericMethods.ViewModels
{
    public class NonLinearChartPageViewModel : BaseViewModel, INavigatedAware
    {
        public NonLinearChartPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            Size = 10;
            ChangeChartSizeCommand = new DelegateCommand(ChangeFunctionDomain);
        }

        private PlotModel _model;
        public PlotModel Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        private List<Operation> _operations = new List<Operation>();

        public DelegateCommand ChangeChartSizeCommand { get; set; }

        private string Formula { get; set; }

        private short Size { get; set; }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!parameters.TryGetValue(NavParams.Function, out List<Operation> operations) || !parameters.TryGetValue(NavParams.Formula, out string formula))
            {
                await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                await NavigationService.GoBackAsync();
                return;
            }

            Formula = formula;
            _operations = operations;

            InitializePlots();
        }

        private async void ChangeFunctionDomain()
        {
            var popup = new EquationsNumberPopup(new EquationPopupHelper
            {
                Description = "Wybierz ile liczb całkowitych ma wchodzić w skład dziedziny",
                Placeholder = "Ilość liczb",
                ItemsSource = new List<string> { "10", "20", "50", "100" },
                Message = AppSettings.ChangeDomainSize
            });

            popup.SetValue(Size.ToString());
            await PopupNavigation.Instance.PushAsync(popup);

            MessagingCenter.Subscribe<EquationsNumberPopup, object>(this, AppSettings.ChangeDomainSize, (sender, arg) =>
            {
                short.TryParse((string)arg, out short size);
                if (size == Size)
                {
                    return;
                }

                Size = size;

                InitializePlots();
            });
        }

        private void InitializePlots()
        {
            var plot = PrepareCoordinateSystem();

            var series1 = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                Title = string.Format(AppResources.NonLinearChart_Function, Formula)
            };
            for (int i = -Size; i <= Size; i++)
            {
                series1.Points.Add(new DataPoint(i, CalculateValue(i)));
            }

            plot.Series.Add(series1);

            Model = plot;
        }

        private float CalculateValue(int argument)
        {
            float value = 0;

            foreach (var operation in _operations)
            {
                value += operation.Value * (float)Math.Pow(argument, operation.Weight);
            }

            return value;
        }

        private PlotModel PrepareCoordinateSystem()
        {
            var plot = new PlotModel { PlotType = PlotType.XY };
            plot.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Key = AppResources.LinearChart_AxisXDescription });
            plot.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Key = AppResources.LinearChart_AxisYDescription });
            plot.LegendTitle = AppResources.NonLinearChart_LegendTitle;
            plot.LegendPosition = LegendPosition.RightBottom;

            return plot;
        }
    }
}