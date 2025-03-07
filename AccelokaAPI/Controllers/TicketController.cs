using Microsoft.AspNetCore.Mvc;
using MediatR;
using AccelokaAPI.Features.Tickets.Queries;
using FluentValidation;
using Serilog;

namespace AccelokaAPI.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<GetAvailableTicketsQuery> _validator;

        public TicketController(IMediator mediator, IValidator<GetAvailableTicketsQuery> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> GetAvailableTickets([FromQuery] GetAvailableTicketsQuery query)
        {
            var validationResult = await _validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                Log.Warning("Invalid ticket query request received.");

                var errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
                
                return BadRequest(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Invalid Request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = string.Join("; ", errors),  // Format errors as a readable string
                    Instance = HttpContext.Request.Path
                });
            }

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
