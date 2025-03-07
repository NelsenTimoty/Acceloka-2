using Microsoft.AspNetCore.Mvc;
using MediatR;
using AccelokaAPI.Commands;
using AccelokaAPI.DTOs;
using AccelokaAPI.Models;
using FluentValidation;
using Serilog;

namespace AccelokaAPI.Controllers
{
    [Route("api/v1/book-ticket")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<BookingRequest> _validator;

        public BookingController(IMediator mediator, IValidator<BookingRequest> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpPost]
        public async Task<IActionResult> BookTicket([FromBody] BookingRequest request)
        {
            Log.Information("Received booking request: {@Request}", request);

            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                Log.Warning("Invalid booking request received.");

                return BadRequest(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid Request",
                    Detail = "Request body is missing or invalid.",
                    Instance = HttpContext.Request.Path
                });
            }

            try
            {
                var result = await _mediator.Send(new BookTicketCommand(request));
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                Log.Warning(ex.Message);
                return NotFound(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Status = StatusCodes.Status404NotFound,
                    Title = "Tickets Not Found",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                });
            }
            catch (InvalidOperationException ex)
            {
                Log.Warning(ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid Booking Request",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                });
            }
        }
    }
}
