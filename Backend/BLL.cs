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


namespace plc_booking_app.Backend
{
    public class BLL
    {
        string databaseConnection = $"Data Source={Path.Combine(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..")), "UL_data.db")};Version=3;";

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

        public void InsertNewBooking(int plcId, string bookingId, int startTimestamp, int endTimestamp)
        {
            using (SQLiteConnection connection = new SQLiteConnection(databaseConnection))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO UL_PLC_BOOKINGS (plc_id, booking_id, start, end) VALUES (@plcId, @bookingId, @startTimestamp, @endTimestamp);";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@plcId", plcId);
                        command.Parameters.AddWithValue("@bookingId", bookingId);
                        command.Parameters.AddWithValue("@startTimestamp", startTimestamp);
                        command.Parameters.AddWithValue("@endTimestamp", endTimestamp);

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
