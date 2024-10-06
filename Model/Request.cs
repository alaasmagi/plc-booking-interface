namespace plc_booking_app.Model
{
    public class Request
    {
        public DateTime requestTimestamp { get; set; }
        public string? bookingId { get; set; }
        public DateTime bookingStart { get; set; }
        public DateTime bookingEnd { get; set; }
        public string? plcId { get; set; }
        public string? requestBody { get; set; }
        public string? apiAuthUser { get; set; }
        public string? apiAuthKey { get; set; }
    }
}
