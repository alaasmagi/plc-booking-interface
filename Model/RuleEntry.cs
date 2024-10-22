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
