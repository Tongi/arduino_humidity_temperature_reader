using Microsoft.Data.Sqlite;
using ScottPlot;
using ScottPlot.AxisPanels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using testest;

namespace testWPF
{
    public partial class GraphicforScottPlot : Window
    {
        public GraphicforScottPlot()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            var sql = "SELECT Id, temperature, humidity, timestamp FROM sqlData";
            using var connection = new SqliteConnection(@"Data Source=C:\Users\Admin\source\repos\Praktikant Opgave\Praktikant Opgave\pub.db");
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();
            var sqlDataList = new List<Measurements>();
            var dataX = new List<DateTime>();
            var dataYTemperature = new List<double>();
            var dataYHumidity = new List<double>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var temperature = reader.GetString(1);
                    var humidity = reader.GetString(2);
                    var timestamp = reader.GetDateTime(3); 
                    sqlDataList.Add(new Measurements
                    {
                        Id = id,
                        temperature = temperature,
                        humidity = humidity
                    });

                    if (double.TryParse(temperature, out var _temperature) && double.TryParse(humidity, out var _humidity))
                    {
                        _temperature /= 10; 
                        _humidity /= 10;

                        dataX.Add(timestamp);
                        dataYTemperature.Add(_temperature);
                        dataYHumidity.Add(_humidity);
                    }
                 
                }

                if (WpfPlot1 != null)
                {
                    WpfPlot1.Plot.Clear();

                    WpfPlot1.Plot.Axes.AddXAxis(new DateTimeXAxis());
                    var temperaturePlot = WpfPlot1.Plot.Add.Scatter(dataX.ToArray(), dataYTemperature.ToArray());
                    temperaturePlot.LegendText = "Temperature";

                    var humidityPlot = WpfPlot1.Plot.Add.Scatter(dataX.ToArray(), dataYHumidity.ToArray());
                    humidityPlot.LegendText = "Humidity";
                    WpfPlot1.Plot.Add.Legend();
                    WpfPlot1.Refresh();
                }
                else
                {
                    MessageBox.Show("Plot not initialized.");
                }
            }
            else
            {
                MessageBox.Show("No rows found in the database.");
            }
        }

    }
}
