using System.Drawing;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class LinearChartViewModel : BaseViewModel
    {

        public LinearChartViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            DrawPlotCommand = GetBusyDependedCommand(DrawPlot);
        }
        private PlotModel _model;
        public PlotModel Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        public DelegateCommand DrawPlotCommand { get; private set; }

        private void DrawPlot()
        {
            var plot = new PlotModel { Title = "y = 2x + 5", PlotType = PlotType.XY };
            plot.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Key = "x" });
            plot.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Key = "y" });

            var series1 = new LineSeries { MarkerType = MarkerType.Circle };
            for (int i = -20; i <= 20; i++)
            {
                series1.Points.Add(new DataPoint(i, 2*i+5));
            }

            plot.Series.Add(series1);

            Model = plot;
        }
    }
}
