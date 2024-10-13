using Microsoft.AspNetCore.Mvc;
using plc_booking_interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using plc_booking_app.Backend;
using System.Text;
using plc_booking_interface.Backend;

namespace plc_booking_interface.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private static List<(Guid Id, Request request)> _requests = new List<(Guid, Request)>();
        public BLL BusinessLogic = new BLL();
        public DAL DataAccess = new DAL();

        [HttpGet("booked_PLCs")]
        public IActionResult GetBookedPLCs([FromQuery] string dateTimeStart, [FromQuery] string dateTimeEnd)
        {
            int startDateTime = DataAccess.ConvertDateToInt(DateTime.Parse(dateTimeStart));
            int endDateTime = DataAccess.ConvertDateToInt(DateTime.Parse(dateTimeEnd));
            List<int> bookedPLCs = DataAccess.GetAllBookedPLCs(startDateTime, endDateTime);

            Console.WriteLine("Käis läbi");
            return Ok(bookedPLCs);
        }

        // GET: api/requests
        [HttpGet]
        public ActionResult<IEnumerable<Request>> GetRequests()
        {
            if (!BusinessLogic.IsAuthorized(Request))
            {
                DataAccess.LogMessage("Failed to obtain access: authorization unsuccessful.", "INFO");
                return Unauthorized("Invalid credentials");
            }

            DataAccess.LogMessage("GET request received successfully.", "IMPORTANT");
            return Ok(_requests.Select(r => r.request));
        }

        // GET: api/requests/{id}
         [HttpGet("{id}")]
        public ActionResult<Request> GetRequest(Guid id)
        {
            if (!BusinessLogic.IsAuthorized(Request))
            {
                DataAccess.LogMessage("Failed to obtain access: authorization unsuccessful.", "INFO");
                return Unauthorized("Invalid credentials");
            }

            var request = _requests.FirstOrDefault(r => r.Id == id);

            if (request.Equals(default))
            {
                return NotFound();
            }

            DataAccess.LogMessage("GET request received successfully.", "IMPORTANT");
            return Ok(request.request);
        }

        // POST: api/requests
        [HttpPost]
        public ActionResult<Request> PostRequest(Request request)
        {

            if (!BusinessLogic.IsAuthorized(Request))
            {
                DataAccess.LogMessage("Failed to obtain access: authorization unsuccessful.", "INFO");
                return Unauthorized("Invalid credentials");
            }

            if (request == null)
            {
                DataAccess.LogMessage("Failed to obtain data from client: empty request.", "WARNING");
                return BadRequest("Request data is null.");
            }

            request.requestTimestamp = TimeZoneInfo.ConvertTimeFromUtc(request.requestTimestamp, TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));
            request.bookingStart = TimeZoneInfo.ConvertTimeFromUtc(request.bookingStart, TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));
            request.bookingEnd = TimeZoneInfo.ConvertTimeFromUtc(request.bookingEnd, TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));

            var id = Guid.NewGuid();

            _requests.Add((id, request));

            DataAccess.LogMessage("POST request received successfully.", "IMPORTANT");
            BusinessLogic.CheckRequestData(request);
            return CreatedAtAction(nameof(GetRequest), new { id }, request);
        }

    }
}

