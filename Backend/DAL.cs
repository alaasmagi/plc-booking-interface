using Microsoft.Data.Sqlite;
using plc_booking_app.Backend;
using plc_booking_interface.Model;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace plc_booking_interface.Backend
{
    public class DAL
    {
        string databaseConnection = $"Data Source={Path.Combine(Path.GetFullPath(Path.Combine
                                            (AppDomain.CurrentDomain.BaseDirectory, @"../../../Data/")), "UL_data.db")};";

        private const string logFilePath = "logs.txt";
        private const string rulesFilePath = "rules.txt";

        public int ConvertDateToInt(DateTime dateTime)
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            int totalMinutes = (int)(dateTime - epochStart).TotalMinutes;

            return totalMinutes;
        }

        public DateTime ConvertIntToDate(int datetime)
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime dateTime = epochStart.AddMinutes(datetime);

            return dateTime;
        }

        public List<int> GetAllBookedPLCs(int startTime, int endTime)
        {
            List<int> PLCs = new List<int>();

            if (startTime == 0 || endTime == 0)
            {
                startTime = ConvertDateToInt(DateTime.Now);
                endTime = startTime;
            }

            using (SqliteConnection connection = new SqliteConnection(databaseConnection))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT plc_id FROM UL_PLC_BOOKINGS WHERE (@dateTimeStart BETWEEN start - 30 AND start + 30 ) " +
                                    "AND (@dateTimeEnd BETWEEN end - 30 AND end + 30);";
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@dateTimeStart", startTime);
                        command.Parameters.AddWithValue("@dateTimeEnd", endTime);
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int plcId = reader.GetInt32(0);
                                PLCs.Add(plcId);
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    LogMessage($"GetAllBookedPLCs() operation unsuccessful. {ex}", "ERROR");
                }
            }
            return PLCs;
        }

        public string GetPLCName(int plcId)
        {
            string plcName = "";
            using (SqliteConnection connection = new SqliteConnection(databaseConnection))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT plc_name FROM UL_PLC_DICTIONARY WHERE plc_id = @plcId;";
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@plcId", plcId);
                        command.ExecuteNonQuery();

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            plcName = Convert.ToString(result);
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    LogMessage($"GetPLCName() operation unsuccessful. {ex}", "ERROR");
                }
            }
            return plcName!;
        }

        public int GetPLCId(string plcValue)
        {
            int plcId = 0;
            using (SqliteConnection connection = new SqliteConnection(databaseConnection))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT plc_id FROM UL_PLC_DICTIONARY WHERE plc_value = @plcValue;";
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@plcValue", plcValue);
                        command.ExecuteNonQuery();

                        object result = command.ExecuteScalar()!;
                        if (result != null && result != DBNull.Value)
                        {
                            plcId = Convert.ToInt32(result);
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    LogMessage($"GetPLCId() operation unsuccessful. {ex}", "ERROR");
                }
            }
            return plcId;
        }

        public bool DoesBookingExist(string bookingId)
        {
            bool exists = false;

            using (SqliteConnection connection = new SqliteConnection(databaseConnection))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM UL_PLC_BOOKINGS WHERE booking_id = @bookingId;";
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@bookingId", bookingId);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        if (count > 0)
                        {
                            exists = true;
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    LogMessage($"DoesBookingExist() operation unsuccessful . {ex}", "ERROR");
                }
            }
            return exists;
        }

        public void InsertNewBooking(Request request)
        {
            int plcId = GetPLCId(request.plcValue);
            int bookingStart = ConvertDateToInt(request.bookingStart); 
            int bookingEnd = ConvertDateToInt(request.bookingEnd);

            string plcName = GetPLCName(plcId);

            if (DoesBookingExist(request.bookingId) == true)
            {
                LogMessage($"Booking already exists. Booking-ID: {request.bookingId}", "WARNING");
                return;
            }
            using (SqliteConnection connection = new SqliteConnection(databaseConnection))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO UL_PLC_BOOKINGS (plc_id, booking_id, start, end) " +
                                    "VALUES (@plcId, @bookingId, @startTimestamp, @endTimestamp);";
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@plcId", plcId);
                        command.Parameters.AddWithValue("@bookingId", request.bookingId);
                        command.Parameters.AddWithValue("@startTimestamp", bookingStart);
                        command.Parameters.AddWithValue("@endTimestamp", bookingEnd);

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    LogMessage($"InsertNewBooking() operation unsuccessful. {ex}", "ERROR");
                }
                LogMessage($"Booking insertion successful. PLC-name: {plcName} Booking-ID: {request.bookingId}", "IMPORTANT");
            }
        }

        public void RemoveBooking(string bookingId)
        {
            if (DoesBookingExist(bookingId) == false)
            {
                LogMessage($"Booking with ID: {bookingId} does not exist. ", "WARNING");
                return;
            }

            using (SqliteConnection connection = new SqliteConnection(databaseConnection))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM UL_PLC_BOOKINGS WHERE booking_id = @bookingId;";
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@bookingId", bookingId);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    LogMessage($"RemoveBooking() operation unsuccessful. {ex}", "ERROR");
                }
                LogMessage($"Booking removal successful. Booking-ID: {bookingId}", "IMPORTANT");
            }
        }

        public void LogMessage(string message, string messageType)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {messageType}: {message}";
            if (messageType == "ERROR" || messageType == "WARNING" || messageType == "IMPORTANT")
            {
                Console.WriteLine(logEntry);
            }
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(logEntry);
            }
        }

        public List<Tuple<DateTime, DateTime>> GetPLCBookings(int plcId)
        {
            List<Tuple<DateTime, DateTime>> PLCbookings = new List<Tuple<DateTime, DateTime>>();

            using (SqliteConnection connection = new SqliteConnection(databaseConnection))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT start, end FROM UL_PLC_BOOKINGS WHERE plc_id = @plcId;";
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@plcId", plcId);
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PLCbookings.Add(Tuple.Create(ConvertIntToDate(reader.GetInt32(0)), ConvertIntToDate(reader.GetInt32(1))));
                                
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"GetAllBookedPLCs() operation unsuccessful. {ex}", "ERROR");
                }
            }

            return PLCbookings;
        }

        public List<RuleEntry> UpdateBookingsByRulesTxt()
        {
            List<RuleEntry> rules = new List<RuleEntry>();
            foreach (var line in File.ReadLines(rulesFilePath))
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("!"))
                {
                    string content = trimmedLine.Substring(1);

                    var parts = content.Split(';');

                    if (parts.Length == 4)
                    {
                        string dayOfWeek = parts[0];
                        string startTime = parts[1];
                        string endTime = parts[2];
                        string plcIds = parts[3];

                        rules.Add(new RuleEntry(dayOfWeek, startTime, endTime, plcIds));
                    }
                    else
                    {
                        LogMessage("rules.txt file had an incorrectly formated entry.", "WARNING");
                    }
                }
            }
            LogMessage($"rules.txt file had {rules.Count} entries.", "IMPORTANT");
            return rules;
        }


        public void SystemClean()
        {
            if (File.Exists(logFilePath))
            { 
                File.Delete(logFilePath);
                Console.WriteLine($"Log file '{logFilePath}' deleted at {DateTime.Now}");
            }
            else
            {
                Console.WriteLine($"Log file '{logFilePath}' does not exist.");
            }

            using (SqliteConnection connection = new SqliteConnection(databaseConnection))
            {
                try
                {
                    connection.Open();
                    int oneHourAgo = ConvertDateToInt(DateTime.UtcNow) - 60;
                    string query = "DELETE FROM UL_PLC_BOOKINGS WHERE end < @oneHourAgo;";
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@oneHourAgo", oneHourAgo);
                        int rowsAffected = command.ExecuteNonQuery();
                        LogMessage($"Removed {rowsAffected} booking(s) that ended more than 1 hour ago.", "IMPORTANT");
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    LogMessage($"SystemClean() operation unsuccessful. {ex}", "ERROR");
                }
            }
            LogMessage($"System cleaning done.", "IMPORTANT");
        }
    }
}
