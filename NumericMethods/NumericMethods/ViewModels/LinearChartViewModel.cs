using NumericMethods.Resources;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class LinearChartViewModel : BaseViewModel, INavigatedAware
    {

        public LinearChartViewModel(
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

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            var plot = new PlotModel { PlotType = PlotType.XY };
            plot.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Key = AppResources.LinearChart_AxisXDescription });
            plot.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Key = AppResources.LinearChart_AxisYDescription });
            plot.LegendTitle = AppResources.LinearChart_LegendTitle;
            plot.LegendPosition = LegendPosition.RightBottom;

            var series1 = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                Title = string.Format(AppResources.LinearChart_FirstFunction, "2x+5")
            };
            for (int i = -20; i <= 20; i++)
            {
                series1.Points.Add(new DataPoint(i, 2 * i + 5));
            }

            plot.Series.Add(series1);

            var series2 = new LineSeries
            {
                MarkerType = MarkerType.Square,
                Color = OxyColor.FromRgb(0, 0, 255),
                Title = string.Format(AppResources.LinearChart_SecondFunction, "-x+4")
            };
            for (int i = -20; i <= 20; i++)
            {
                series2.Points.Add(new DataPoint(i, -i + 4));
            }

            plot.Series.Add(series2);
            Model = plot;
        }
    }
}
