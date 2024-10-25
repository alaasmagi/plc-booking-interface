﻿using System;
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
using System.Timers;

namespace plc_booking_app.Backend
{
    public class BLL
    {
        public static int totalNrOfPLCs = 13;
        public static DAL DataAccess = new DAL();
        public static System.Timers.Timer SystemCleanTimer;

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
            if (request.requestBody == "occupy" && DataAccess.GetPLCId(request.plcValue) > 0)
            {
                DataAccess.InsertNewBooking(request, 0);
            }
            else if (request.requestBody == "update" && DataAccess.GetPLCId(request.plcValue) > 0)
            {
                DataAccess.RemoveBooking(request.bookingId);
                DataAccess.InsertNewBooking(request, 0);
            }
            else if (request.requestBody == "cancel" && DataAccess.GetPLCId(request.plcValue) > 0)
            {
                DataAccess.RemoveBooking(request.bookingId);
            }
            else
            {
                DataAccess.LogMessage("Booking request is invalid.", "WARNING");
            }
        }

        public static DateTime GetNearestDayOfWeek(string dayOfWeek)
        { 
            var targetDayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayOfWeek, true);
            DateTime today = DateTime.Today;
            int daysToAdd = ((int)targetDayOfWeek - (int)today.DayOfWeek + 7) % 7;

            if (daysToAdd == 0 && today.DayOfWeek.ToString() != dayOfWeek)
            {
                daysToAdd = 7;
            }

            return today.AddDays(daysToAdd);
        }

        public static List<int> GetAllPLCsFromRule(string rulePLCPart)
        {
            var result = new List<int>();

            if (rulePLCPart == "A")
            {
                for (int i = 1; i < totalNrOfPLCs + 1; i++)
                {
                    result.Add(1000 + i);
                }
                return result;
            }

            var PLCids = rulePLCPart.Split(',');

            foreach (var PLCid in PLCids)
            {
                result.Add(int.Parse(PLCid));
            }

            return result;
        }


        public static void ApplyingRules(List<RuleEntry> rules)
        {
            foreach (RuleEntry rule in rules)
            {
                var plcIds = GetAllPLCsFromRule(rule.PlcIds);

                foreach (var plcId in plcIds)
                {
                    Request request = new Request();

                    request.requestTimestamp = DateTime.Now;
                    request.bookingStart = GetNearestDayOfWeek(rule.DayOfWeek).Add(TimeSpan.Parse(rule.StartTime));
                    request.bookingEnd = GetNearestDayOfWeek(rule.DayOfWeek).Add(TimeSpan.Parse(rule.EndTime));
                    request.bookingId = DataAccess.ConvertDateToInt(request.bookingStart).ToString() + plcId;
                    DataAccess.InsertNewBooking(request, plcId);
                }
            }
        }

        public static void StartSystemCleanTimer()
        {
            var rules = DataAccess.UpdateBookingsByRulesTxt();
            ApplyingRules(rules);
            SystemCleanTimer = new System.Timers.Timer(86400000);
            SystemCleanTimer.Elapsed += SystemCleanElapseEvent;
            SystemCleanTimer.AutoReset = true;
            SystemCleanTimer.Enabled = true;
            DataAccess.LogMessage("System cleaning timer started. System will be cleaned in 24 hours.", "IMPORTANT");
        }
        private static void SystemCleanElapseEvent(Object source, ElapsedEventArgs e)
        {
            DataAccess.SystemClean();
        }       
    }
}
