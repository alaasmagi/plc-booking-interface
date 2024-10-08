using Microsoft.AspNetCore.Mvc;
using plc_booking_interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using plc_booking_app.Backend;
using System.Text;

namespace plc_booking_interface.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private static List<(Guid Id, Request request)> _requests = new List<(Guid, Request)>();
        public BLL Logic = new BLL(); 


        // GET: api/requests
        [HttpGet]
        public ActionResult<IEnumerable<Request>> GetRequests()
        {
            if (!Logic.IsAuthorized(Request))
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(_requests.Select(r => r.request));
        }

        // GET: api/requests/{id}
         [HttpGet("{id}")]
        public ActionResult<Request> GetRequest(Guid id)
        {
            if (!Logic.IsAuthorized(Request))
            {
                return Unauthorized("Invalid credentials");
            }

            var request = _requests.FirstOrDefault(r => r.Id == id);

            if (request.Equals(default))
            {
                return NotFound();
            }

            return Ok(request.request);
        }

        // POST: api/requests
        [HttpPost]
        public ActionResult<Request> PostRequest(Request request)
        {

            if (!Logic.IsAuthorized(Request))
            {
                return Unauthorized("Invalid credentials");
            }

            if (request == null)
            {
                return BadRequest("Request data is null.");
            }

            request.requestTimestamp = TimeZoneInfo.ConvertTimeFromUtc(request.requestTimestamp, TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));
            request.bookingStart = TimeZoneInfo.ConvertTimeFromUtc(request.bookingStart, TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));
            request.bookingEnd = TimeZoneInfo.ConvertTimeFromUtc(request.bookingEnd, TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));

            var id = Guid.NewGuid();

            _requests.Add((id, request));


            Logic.InsertNewBooking(request);
            return CreatedAtAction(nameof(GetRequest), new { id }, request);
        }

    }
}

