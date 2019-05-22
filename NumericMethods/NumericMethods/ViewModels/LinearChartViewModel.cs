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
    public class LinearChartViewModel : BaseViewModel, INavigatedAware
    {
        public LinearChartViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            ChangeChartSizeCommand = new DelegateCommand(ChangeEquationsNumber);
            Size = 50;
        }

        #region Properties
        private PlotModel _model;
        public PlotModel Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        private List<Equation> EquationsList { get; set; }

        private short Size { get; set; }
        #endregion

        public DelegateCommand ChangeChartSizeCommand { get; set; }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!parameters.TryGetValue(NavParams.Equations, out List<Equation> equationsList))
            {
                await ShowAlert(AppResources.Common_Ups, AppResources.Common_SomethingWentWrong);
                await NavigationService.GoBackAsync();
                return;
            }

            EquationsList = equationsList;

            InitializePlots();
        }

        private async void ChangeEquationsNumber()
        {
            var popup = new EquationsNumberPopup(new EquationPopupHelper
            {
                Description = AppResources.LinearChartPopup_Description,
                Placeholder = AppResources.LinearChartPopup_Placeholder,
                ItemsSource = new List<string> { "10", "50", "100", "200", "500" },
                Message = AppSettings.ChangePlotSize
            });

            popup.SetValue(Size.ToString());
            await PopupNavigation.Instance.PushAsync(popup);

            MessagingCenter.Subscribe<EquationsNumberPopup, object>(this, AppSettings.ChangePlotSize, (sender, arg) =>
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

            var equation = EquationsList[0];

            double.TryParse(equation.ConstantTerm, out var constantTerm);
            double.TryParse(equation.X, out var xResult);
            double.TryParse(equation.Y, out var yResult);

            var series1 = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                Title = string.Format(AppResources.LinearChart_FirstFunction, $"{xResult / yResult}x+{constantTerm / yResult}")
            };
            for (int i = -Size; i <= Size; i++)
            {
                series1.Points.Add(new DataPoint(i, (constantTerm - xResult * i) / yResult));
            }

            plot.Series.Add(series1);

            equation = EquationsList[1];
            double.TryParse(equation.ConstantTerm, out constantTerm);
            double.TryParse(equation.X, out xResult);
            double.TryParse(equation.Y, out yResult);

            var series2 = new LineSeries
            {
                MarkerType = MarkerType.Square,
                Color = OxyColor.FromRgb(0, 0, 255),
                Title = string.Format(AppResources.LinearChart_SecondFunction, $"{xResult / yResult}x+{constantTerm / yResult}")
            };

            for (int i = -Size; i <= Size; i++)
            {
                series2.Points.Add(new DataPoint(i, (constantTerm - xResult * i) / yResult));
            }

            plot.Series.Add(series2);
            Model = plot;
        }

        private PlotModel PrepareCoordinateSystem()
        {
            var plot = new PlotModel { PlotType = PlotType.XY };
            plot.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Key = AppResources.LinearChart_AxisXDescription });
            plot.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Key = AppResources.LinearChart_AxisYDescription });
            plot.LegendTitle = AppResources.LinearChart_LegendTitle;
            plot.LegendPosition = LegendPosition.RightBottom;

            return plot;
        }
    }
}
