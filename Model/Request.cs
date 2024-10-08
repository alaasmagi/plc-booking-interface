namespace plc_booking_interface.Model
{
    public class Request
    {
        public DateTime requestTimestamp { get; set; }
        public string bookingId { get; set; }
        public DateTime bookingStart { get; set; }
        public DateTime bookingEnd { get; set; }
        public string plcValue { get; set; }
        public string requestBody { get; set; }
    }
}
