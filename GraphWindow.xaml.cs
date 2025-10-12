using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Carlytics
{
    /// <summary>
    /// Graph window
    /// </summary>
    public partial class GraphWindow : Window
    {
        public List<RefuelingRecond> refuelingReconds;

        public ISeries[] Series { get; set; }

        public Axis[] XAxes { get; set; }
        
        public Axis[] YAxes { get; set; }

        private void LoadChart(int i)
        {
            List<double> charValues = new List<double>();
            string Yname = string.Empty;
            switch (i)
            {
                case 0:
                    charValues = refuelingReconds.Select(record => record.PricePerLiter).ToList();
                    Yname = "Price Per Liter";
                    break;
                case 1:
                    charValues = refuelingReconds.Select(record => record.Price).ToList();
                    Yname = "Price";
                    break;
                case 2:
                    charValues = refuelingReconds.Select(record => record.Liter).ToList();
                    Yname = "Liter Taken";
                    break;
                case 3:
                    charValues = refuelingReconds.Select(record => record.LPerKm).ToList();
                    Yname = "Consumption";
                    break;
                default:
                    Yname = "none";
                    break;
            }

            charValues.Reverse();
            List<string> charLabels = refuelingReconds.Select(record => record.Id.ToString()).ToList();
            charLabels.Reverse();

            double yMinValue = 0.0;
            if (charValues.Any())
            {
                double minDataValue = charValues.Min();
                yMinValue = Math.Max(0, minDataValue - (minDataValue * 0.1));
            }


            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = charValues,
                    Name = Yname,
                    Fill = null,
                    GeometryFill = new SolidColorPaint(SKColors.Blue),
                    GeometryStroke = new SolidColorPaint(SKColors.Blue),
                    GeometrySize = 8,
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 3 },
                    DataLabelsPaint = new SolidColorPaint(SKColors.DarkBlue),
                    DataLabelsPosition = DataLabelsPosition.Top
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = charLabels.ToArray(),
                    Name = "Id",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 },
                    UnitWidth = 1,
                    MinLimit = null,
                    MaxLimit = null                    
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Name = Yname,
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    MinLimit = yMinValue,
                    Labeler = (value) => value.ToString("F1"),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 } 
                }
            };

            if (chartRefueling != null)
            {
                chartRefueling.ZoomMode = ZoomAndPanMode.X;
            }
        }

        public GraphWindow(List<RefuelingRecond> reconds, int i)
        {
            InitializeComponent();

            this.DataContext = this;

            refuelingReconds = reconds;
            LoadChart(i);
        }
    }
}
