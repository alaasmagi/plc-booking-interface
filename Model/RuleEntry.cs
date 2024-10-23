namespace plc_booking_interface.Model
{
    public class RuleEntry
    {
        public string DayOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string PlcIds { get; set; }

        public RuleEntry(string dayOfWeek, string startTime, string endTime, string plcIds)
        {
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
            PlcIds = plcIds;
        }
    }
}



/*
 public class Booking
{
    public string DayOfWeek { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string PlcIds { get; set; }

    // Converts a time string (e.g., "14:30") and day of the week (e.g., "Mon") to a UNIX timestamp in minutes
    private static long ConvertToUnixMinutes(string dayOfWeek, string time)
    {
        // Combine day of week and time to get full datetime (e.g., "Mon 14:30")
        var format = "ddd HH:mm";
        DateTime dateTime = DateTime.ParseExact($"{dayOfWeek} {time}", format, CultureInfo.InvariantCulture);
        
        // Convert to UNIX timestamp (seconds) then divide by 60 to get minutes
        long unixTime = new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        return unixTime / 60;
    }

    public static void ProcessSchedule(Booking booking)
    {
        string[] plcIds;

        if (booking.PlcIds == "A")
        {
            // Assuming we handle "A" as a range of PLCs (you'll need to specify your range)
            plcIds = new string[] { "1001", "1002", "1003", "1004" }; // Example PLC IDs for "All PLCs"
        }
        else
        {
            plcIds = booking.PlcIds.Split(','); // Specific PLCs
        }

        // Convert StartTime and EndTime into UNIX timestamps (in minutes)
        long start = ConvertToUnixMinutes(booking.DayOfWeek, booking.StartTime);
        long end = ConvertToUnixMinutes(booking.DayOfWeek, booking.EndTime);

        foreach (var plcId in plcIds)
        {
            string bookingId = Guid.NewGuid().ToString(); // Unique ID for the booking

            // Here, you would insert the data into your database
            Console.WriteLine($"INSERT INTO bookings (plc_id, booking_id, start, end) VALUES ({plcId}, '{bookingId}', {start}, {end});");
        }
    }

    public static void Main()
    {
        // Example schedule items
        var bookings = new Booking[]
        {
            new Booking { DayOfWeek = "Mon", StartTime = "14:30", EndTime = "16:00", PlcIds = "A" },
            new Booking { DayOfWeek = "Thu", StartTime = "14:00", EndTime = "16:00", PlcIds = "A" },
            new Booking { DayOfWeek = "Fri", StartTime = "09:00", EndTime = "11:15", PlcIds = "1003,1006,1008" },
            new Booking { DayOfWeek = "Fri", StartTime = "14:00", EndTime = "17:15", PlcIds = "A" }
        };

        foreach (var booking in bookings)
        {
            ProcessSchedule(booking);
        }
    }
}*/