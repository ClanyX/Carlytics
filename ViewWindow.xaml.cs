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
    /// Interakční logika pro ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {
        private RefuelingRecond _record;
        private Window _window;
        public ViewWindow(RefuelingRecond record, Window window)
        {
            _record = record;
            _window = window;
            InitializeComponent();
            Show(record);
        }

        private void Show(RefuelingRecond record)
        {
            title.Content = $"{record.Name} - {record.Id}";
            one.Content = record.PricePerLiter;
            two.Content = record.Liter;
            three.Content = record.Price;
            four.Content = record.Date;
            five.Content = record.Kilometer;
        }
    }
}
