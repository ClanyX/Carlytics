using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interakční logika pro AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private string _conString = "Data source=spends.db";
        private ProgramWindow _window;
        public AddWindow(Window window)
        {
            InitializeComponent();
            if(window is ProgramWindow)
            {
                _window = (ProgramWindow)window;
            }
        }

        private void onClickAdd(object sender, RoutedEventArgs e)
        {
            if (name.Text == null || ppl.Text == null || lt.Text == null || price.Text == null || kilometers.Text == null || !dp.SelectedDate.HasValue) return;

            string _name = name.Text.Trim();
            double _ppl = Math.Round(double.Parse(ppl.Text), 2);
            double _lt = Math.Round(double.Parse(lt.Text), 2);
            double _price = Math.Round(double.Parse(price.Text), 2);
            int _kilometers = (int)double.Parse(kilometers.Text);
            double _average = 0.0;
            string _dp = dp.SelectedDate.Value.ToString("dd-MM-yyyy").Trim();

            using (SqliteConnection connection = new SqliteConnection(_conString))
            {
                connection.Open();
                string cmd = "INSERT INTO Refueling (Name, PricePerLiter, Liter, LPerKm, Price, Date, Kilometer) " +
                             "VALUES (@name, @priceperliter, @liter, @lperkm, @price, @date, @kilometer)";

                string cmd1 = "SELECT SUM(Liter) AS TotalLiters, MAX(Kilometer) AS MaxKM, MIN(Kilometer) AS MinKM FROM Refueling;";
                var result = connection.QueryFirstOrDefault(cmd1);
                double totalDis = 0.0;
                try
                {
                    totalDis = result?.MaxKM - result?.MinKM;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (result == null || totalDis <= 0)
                {
                    _average = 0.0;
                }
                else
                {
                    _average = ((double)result?.TotalLiters * 100.00) / totalDis;
                    string x = _average.ToString("F2");
                    _average = double.Parse(x);
                }

                object[] parameters = { new {  name = _name, priceperliter = _ppl, liter =  _lt, lperkm = _average, price = _price, date = _dp, kilometer = _kilometers  } };
                connection.Execute(cmd, parameters[0]);
            }
            _window.LoadRefuelingData();
            this.Close();
        }

        private void onClickCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void spaceChecker(object sender, KeyEventArgs e)
        {
            if(e.Key  == Key.Space && sender != name)
            {
                e.Handled = true;
            }
        }

        private void decimalInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == null) return;
            
            string currentText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);
            string newChar = e.Text;

            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
            string decimalSeparator = nfi.NumberDecimalSeparator;

            bool isDigit = char.IsDigit(newChar, 0);
            bool isDecimalSeparator = newChar == decimalSeparator;

            if (isDigit)
            {
                e.Handled = false;
            }
            else if(isDecimalSeparator) 
            {
                if(currentText.Contains(decimalSeparator) || string.IsNullOrEmpty(currentText))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
            }

        }
    }
}
