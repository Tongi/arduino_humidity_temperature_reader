using System;
using System.IO.Ports;
using Newtonsoft.Json;
using Microsoft.Data.Sqlite;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Xml.Linq;

namespace testest
{
    public class Measurements
    {
        public int Id { get; set; }
        public string temperature { get; set; }
        public string humidity { get; set; }
        public DateTime timestamp { get; set; } 
    }

    public  class DataFromCom3
    {
        public  void DataOpener()
        {
            using SerialPort port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            dataReciever DataReciever = new dataReciever();
            port.DataReceived += new SerialDataReceivedEventHandler(DataReciever.port_DataRecived);
            try
            {
                port.Open();
                Console.WriteLine("Press any key to close the port...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening port: {ex.Message}");
            }
        }
    }

    public class dataReciever
    {
        public void port_DataRecived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port_data = (SerialPort)sender;
            string data = port_data.ReadExisting();

            Measurements measurements = JsonConvert.DeserializeObject<Measurements>(data);

            if (measurements != null)
            {
                Console.WriteLine($"Humidity: {measurements.humidity} %\nTemperature: {measurements.temperature} °C");
                InsertDataToDb(measurements);
            }
            else
            {
                Console.WriteLine("Received data is not in the expected format.");
            }
        }

        public static void InsertDataToDb(Measurements measurements)
        {
            var sql = "INSERT INTO sqlData (humidity, temperature) VALUES (@get_humidity, @get_temperature)";

            try
            {
                using var connection = new SqliteConnection(@"Data Source=C:\Users\Admin\source\repos\Praktikant Opgave\Praktikant Opgave\pub.db");
                connection.Open();

                using var command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@get_humidity", measurements.humidity);
                command.Parameters.AddWithValue("@get_temperature", measurements.temperature);

                command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

      
        /*    public static void ReadingDatabase(string sql)
            {

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
                        sqlDataList.Add(new Measurements
                        {
                            Id = id,
                            temperature = temperature,
                            humidity = humidity
                        });
                    }
                    gridTextBox.ItemsSource = sqlDataList;
                }
            }*/

    }
}











