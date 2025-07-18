using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using Microsoft.Data.Sqlite;
using Dapper;

namespace Carlytics
{
    /// <summary>
    /// Interakční logika pro ProgramWindow.xaml
    /// </summary>
    public partial class ProgramWindow : Window
    {
        private string _conString = "Data source=spends.db";

        public ProgramWindow()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadRefuelingData();
            statusbar.Content = "Successful login";
        }

        private void InitializeDatabase()
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_conString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Refueling(
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            PricePerLiter REAL NOT NULL,
                            Liter REAL NOT NULL,
                            LPerKm REAL NOT NULL,
                            Price REAL NOT NULL,
                            Date TEXT NOT NULL,
                            Kilometer INTEGER NOT NULL
                        );";

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error with inicialization of database: {ex.Message}", "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadRefuelingData()
        {
            try
            {
                var allRecords = GetAllRefuelings();

                dgRefueling.ItemsSource = allRecords;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error {ex}", "err");
            }
        }

        public List<RefuelingRecond> GetAllRefuelings()
        {
            using(SqliteConnection connection = new SqliteConnection(_conString))
            {
                connection.Open();

                var record = connection.Query<RefuelingRecond>(
                    "SELECT Id, Name, PricePerLiter, Liter, Price, LPerKm, Date, Kilometer FROM Refueling ORDER BY Date DESC;"
                ).ToList();

                return record;
            }
        }

        //Events
        private void onClickExit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void onClickRefresh(object sender, RoutedEventArgs e)
        {
            LoadRefuelingData();
            statusbar.Content = "Refresh completed";
        }

        private void onClickView(object sender, MouseButtonEventArgs e)
        {
            if(dgRefueling.SelectedItem is RefuelingRecond selectedRecord)
            {
                statusbar.Content = $"Item selected Id:{selectedRecord.Id}";
                ViewWindow window = new ViewWindow(selectedRecord, this);
                window.ShowDialog();
            }
            else
            {
                statusbar.Content = "Not selected item";
            }
        }

        private void onClickDelete(object sender, MouseButtonEventArgs e)
        {
            if(dgRefueling.SelectedItem is RefuelingRecond selectedRecord)
            {
                using (SqliteConnection connection = new SqliteConnection(_conString))
                {
                    MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete this entry {selectedRecord.Id} from {selectedRecord.Date}?", "Confirmation of deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if(result == MessageBoxResult.Yes)
                    {
                        connection.Open();
                        string cmd = "DELETE FROM Refueling WHERE Id = @id";
                        connection.Execute(cmd, new { id = selectedRecord.Id });
                        LoadRefuelingData();
                        statusbar.Content = $"Delete item with id={selectedRecord.Id}";
                    }
                }
            }
        }

        private void onClickAdd(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new AddWindow();
            addWindow.ShowDialog();
        }
    }
    public class RefuelingRecond
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double PricePerLiter { get; set; }
        public double Liter { get; set; }
        public double Price { get; set; }
        public double LPerKm { get; set; }
        public string? Date { get; set; }
        public int Kilometer { get; set; }
    }
}
