using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using testest;
namespace testWPF
{
    public partial class MainWindow : Window
    {
        public SerialPort _port;

        public MainWindow()
        {
            InitializeComponent();
            ReadingDatabase();
            StartReadingData();
            GraphicforScottPlot VindueTo = new GraphicforScottPlot();
            VindueTo.Show();
        }
        public void StartReadingData()
        {
            _port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            _port.DataReceived += Port_DataReceived;

            try
            {
                _port.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToUpper());
            }
        }



        public void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = _port.ReadExisting();
            try
            {
                Measurements measurements = JsonConvert.DeserializeObject<Measurements>(data);




                this.Dispatcher.Invoke(() =>
                {
                    TemperatureTextBox.Text = $"{measurements.temperature} Gc";
                    HumidityTextBox.Text = $"{measurements.humidity} %";
                });

                
                dataReciever.InsertDataToDb(measurements);

                
            
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show($"{jsonEx.Message}\nInvalid JSON format.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nAn error occurred.");
            }
        }











        public void ReadingDatabase()
{
    var sql = "SELECT * FROM sqlData";
    using var connection = new SqliteConnection(@"Data Source=C:\Users\Admin\source\repos\Praktikant Opgave\Praktikant Opgave\pub.db");
    connection.Open();

    using var command = new SqliteCommand(sql, connection);
    
    using var reader = command.ExecuteReader();
    var sqlDataList = new List<Measurements>();

    if (reader.HasRows)
    {
        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var temperature = reader.GetString(1);
            var humidity = reader.GetString(2);
            sqlDataList.Add(new Measurements { 
                Id = id,
                temperature = temperature,
                humidity = humidity 
            });
        }
        gridTextBox.ItemsSource = sqlDataList;
    }
}

  


 

    }
}

