using API.Models;
using Application.Features.Schedule.BookClass.Commands;
using Application.Features.Schedule.CancelBooking.Commands;
using Application.Features.Schedule.CheckIn.Commands;
using Application.Features.Schedule.GetSchedules.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScheduleController(IMediator mediator) => _mediator = mediator;

        private int GetUserId() => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetSchedules([FromQuery] string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode)) return BadRequest("CountryCode is required.");
            var query = new GetSchedulesQuery(countryCode);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookClass([FromBody] BookClassRequest request)
        {
            try
            {
                var userId = GetUserId();
                var command = new BookClassCommand(userId, request.ClassScheduleId);
                var result = await _mediator.Send(command);

                if (result.Contains("successful") || result.Contains("waitlist"))
                {
                    return Ok(new { Message = result });
                }
                return BadRequest(new { Message = result });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingRequest request)
        {
            try
            {
                var userId = GetUserId();
                var command = new CancelBookingCommand(userId, request.BookingId);
                var result = await _mediator.Send(command);
                return Ok(new { Message = result });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            try
            {
                var userId = GetUserId();
                var command = new CheckInCommand(userId, request.BookingId);
                var result = await _mediator.Send(command);
                return Ok(new { Message = result ?? "Check-in successful." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
