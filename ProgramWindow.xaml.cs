using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        void ExportToCsv(DataTable dataTable, string filePath, char separator = ';')
        {
            using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(true)))
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sw.Write("\"" + dataTable.Columns[i].ColumnName + "\"");
                    if (i < dataTable.Columns.Count - 1)
                    {
                        sw.Write(separator);
                    }
                }
                sw.WriteLine();

                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        string value = Convert.IsDBNull(row[i]) ? string.Empty : row[i].ToString();

                        string formattedValue = value.Replace("\"", "\"\"");

                        sw.Write($"\"{formattedValue}\"");

                        if (i < dataTable.Columns.Count - 1)
                        {
                            sw.Write(separator);
                        }
                    }
                    sw.WriteLine();
                }
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
            AddWindow addWindow = new AddWindow(this);
            addWindow.ShowDialog();
        }

        private void onDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                DependencyObject dep = (DependencyObject)e.OriginalSource;
                while((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                if(dep is DataGridRow row)
                {
                    if(row.DataContext is RefuelingRecond selectedItem)
                    {
                        statusbar.Content = $"Item selected Id:{selectedItem.Id}";
                        ViewWindow window = new ViewWindow(selectedItem, this);
                        window.ShowDialog();
                    }
                }
            }
        }

        private void onClickPpl(object sender, RoutedEventArgs e)
        {
            GraphWindow graphWindow = new GraphWindow(GetAllRefuelings(),0);
            graphWindow.Show();
        }

        private void onClickP(object sender, RoutedEventArgs e)
        {
            GraphWindow graphWindow = new GraphWindow(GetAllRefuelings(),1);
            graphWindow.Show();
        }

        private void onClickLiter(object sender, RoutedEventArgs e)
        {
            GraphWindow graphWindow = new GraphWindow(GetAllRefuelings(), 2);
            graphWindow.Show();
        }

        private void onClickC(object sender, RoutedEventArgs e)
        {
            GraphWindow graphWindow = new GraphWindow(GetAllRefuelings(), 3);
            graphWindow.Show();
        }

        private void onClickExport(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV soubor (*.csv)|*.csv",
                FileName = "Export_Dat.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string connectionString = "Data Source=spends.db;";
                string selectQuery = "SELECT Id, Name, PricePerLiter, Liter, Price, LPerKm, Date, Kilometer FROM Refueling ORDER BY Date DESC;";
                DataTable dataTable = new DataTable();

                try
                {
                    using (var connection = new SqliteConnection(connectionString))
                    {
                        connection.Open();
                        using (var command = new SqliteCommand(selectQuery, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                dataTable.Load(reader);
                            }
                        }
                    }

                    ExportToCsv(dataTable, saveFileDialog.FileName, ';');

                    MessageBox.Show("Data exported successfully.", "Export complete");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Err: {ex.Message}", "Err", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
