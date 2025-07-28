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
        public ISeries[] Series { get; set; }

        public Axis[] XAxes { get; set; }
        
        public Axis[] YAxes { get; set; }

        private void LoadChart()
        {
            var charValues = new List<double> { 1,4,5,6,4,3,7,3 };
            var charLabels = new List<string> { "1","2","3","4","5","6","7","8"};

            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = charValues,
                    Name = "x",
                    Fill = null,
                    GeometryFill = new SolidColorPaint(SKColors.Blue),
                    GeometryStroke = new SolidColorPaint(SKColors.Blue),
                    GeometrySize = 8,
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 3 },
                    DataLabelsPaint = new SolidColorPaint(SKColors.DarkBlue),
                    DataLabelsPosition = DataLabelsPosition.Top,
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = charLabels.ToArray(),
                    LabelsRotation = 45,
                    Name = "Id",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 }
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Y",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    MinLimit = 0,
                    Labeler = (value) => value.ToString("F1"),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 } 
                }
            };
        }

        public GraphWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            LoadChart();
        }
    }
}
