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
        public AddWindow()
        {
            InitializeComponent();
        }

        private void onClickAdd(object sender, RoutedEventArgs e)
        {
            if (name.Text == null) return;
            if (ppl.Text == null) return;
            if (lt.Text == null) return;
            if (price.Text == null) return;
            if (kilometers.Text == null) return;
            if (!dp.SelectedDate.HasValue) return;

            string _name = name.Text;
            double _ppl = double.Parse(ppl.Text);
            double _lt = double.Parse(lt.Text);
            double _price = double.Parse(price.Text);
            int _kilometers = (int)double.Parse(kilometers.Text);
            double _average = 0.0;
            string _dp = dp.SelectedDate.Value.ToString("dd-MM-yyyy");

            using (SqliteConnection connection = new SqliteConnection(_conString))
            {
                connection.Open();
                string cmd = "INSERT INTO Refueling (Name, PricePerLiter, Liter, LPerKm, Price, Date, Kilometer) " +
                             "VALUES (@name, @priceperliter, @liter, @lperkm, @price, @date, @kilometer)";

                string cmd1 = "SELECT SUM(Liter) AS TotalLiters, MAX(Kilometer) AS MaxKM, MIN(Kilometer) AS MinKM FROM Refueling;";
                var result = connection.QueryFirstOrDefault(cmd1);
                double totalDis = result?.MaxKM - result?.MinKM;

                //ten prumer bude asi vsede stejny kdyztak zmenit jen na jedno tankovani(pouze 2 des mista)
                if (result == null || totalDis <= 0)
                {
                    _average = 0.0;
                }
                else
                {
                    _average = ((double)result?.TotalLiters * 100.00) / totalDis;
                }

                object[] parameters = { new {  name = _name, priceperliter = _ppl, liter =  _lt, lperkm = _average, price = _price, date = _dp, kilometer = _kilometers  } };
                connection.Execute(cmd, parameters[0]);
            }

            this.Close();
        }

        private void onClickCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void spaceChecker(object sender, KeyEventArgs e)
        {
            if(e.Key  == Key.Space)
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
