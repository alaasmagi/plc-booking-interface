using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Xml.Linq;
using plc_booking_interface.Model;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using plc_booking_interface.Backend;


namespace plc_booking_app.Backend
{
    public class BLL
    {
        public DAL DataAccess = new DAL();
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

        public void CheckRequestData(Request request)
        {
            if (request.requestBody == "occupy")
            {
                DataAccess.InsertNewBooking(request);
            }
            else if (request.requestBody == "update")
            {
                DataAccess.RemoveBooking(request.bookingId);
                DataAccess.InsertNewBooking(request);
            }
            else if (request.requestBody == "cancel")
            {
                DataAccess.RemoveBooking(request.bookingId);
            }
            else
            {
                DataAccess.LogMessage("Booking request is invalid.", "WARNING");
            }
        }
    }
}
