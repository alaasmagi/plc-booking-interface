using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Xml.Linq;
using plc_booking_interface.Model;


namespace plc_booking_app.Backend
{
    public class BLL
    {
        string databaseConnection = $"Data Source={Path.Combine(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../Data/")), "UL_data.db")};Version=3;";
        private const string Username = "123"; // Replace with your actual username
        private const string Password = "456"; // Replace with your actual password

        public bool IsAuthorized(HttpRequest request)
        {
            if (!request.Headers.ContainsKey("Authorization"))
                return false;

            var authHeader = request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Basic ".Length).Trim();
                var credentialString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialString.Split(':');

                return credentials.Length == 2 && credentials[0] == Environment.GetEnvironmentVariable("PLC_API_USER") && credentials[1] == 
                                                                                                    Environment.GetEnvironmentVariable("PLC_API_PASS");
            }

            return false;
        }

        public int checkPlcStatus(int plcId)
        {
            int result = 9;
            using (SQLiteConnection connection = new SQLiteConnection(databaseConnection))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT plc_status FROM UL_PLC_STATUS WHERE plc_id = @plcId;";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@plcId", plcId);

                        object status = command.ExecuteScalar();

                        if (status != null)
                        {
                            result = Convert.ToInt32(result);
                        }
                    }
                    connection.Close();
                }
                catch (Exception Ex)
                {

                }
            }
            return result;
        }

        void UpdatePlcStatus(int plcId, int plcStatus)
        {
            using (SQLiteConnection connection = new SQLiteConnection(databaseConnection))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE UL_PLC_BOOKINGS SET plc_status = @plcStatus WHERE plc_id = plcId;";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@plcId", plcId);
                        command.Parameters.AddWithValue("@plcStatus", plcStatus);

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                catch (Exception Ex)
                {

                }
            }
        }

        public int CheckBookingStatus(int plcId)
        {
            int status = 0;

            return status;
        }

        public void UpdateBookingStatus(int plcId, int plcStatus)
        {

        }

        public void InsertNewBooking(Request request)
        {
            Console.WriteLine(databaseConnection);
            Console.WriteLine(request.requestBody);
            using (SQLiteConnection connection = new SQLiteConnection(databaseConnection))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO UL_PLC_BOOKINGS (plc_id, booking_id, start, end) VALUES (@plcId, @bookingId, @startTimestamp, @endTimestamp);";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@plcId", request.plcId);
                        command.Parameters.AddWithValue("@bookingId", request.bookingId);
                        command.Parameters.AddWithValue("@startTimestamp", request.bookingStart);
                        command.Parameters.AddWithValue("@endTimestamp", request.bookingEnd);

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                catch (Exception Ex)
                {

                }

            }
        }
    }
}
