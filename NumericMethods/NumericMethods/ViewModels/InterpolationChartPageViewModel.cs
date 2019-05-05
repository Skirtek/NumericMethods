using System.Collections.Generic;
using NumericMethods.Models;
using NumericMethods.Resources;
using NumericMethods.Settings;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class InterpolationChartPageViewModel : BaseViewModel, INavigatedAware
    {
        public InterpolationChartPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
        }

        private PlotModel _model;
        public PlotModel Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        private List<PointModel> _points = new List<PointModel>();

        private List<double> _xList = new List<double>();

        private List<double> _yList = new List<double>();

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!parameters.TryGetValue(NavParams.Points, out List<PointModel> points))
            {
                await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                await NavigationService.GoBackAsync();
                return;
            }

            _points = points;

            ConvertValues();
            InitializePlots();
        }

        private void ConvertValues()
        {
            foreach (var point in _points)
            {
                double.TryParse(point.X, out double x);
                double.TryParse(point.Y, out double y);

                _xList.Add(x);
                _yList.Add(y);
            }
        }

        private void InitializePlots()
        {
            var plot = PrepareCoordinateSystem();

            var series1 = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(0,96,255),
                Color = OxyColors.Transparent
            };

            for (var i = 0; i < _xList.Count; i++)
            {
                series1.Points.Add(new DataPoint(_xList[i], _yList[i]));
            }

            plot.Series.Add(series1);

            Model = plot;
        }

        private PlotModel PrepareCoordinateSystem()
        {
            var plot = new PlotModel { PlotType = PlotType.XY };
            plot.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Key = AppResources.LinearChart_AxisXDescription });
            plot.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Key = AppResources.LinearChart_AxisYDescription });

            return plot;
        }
    }
}
