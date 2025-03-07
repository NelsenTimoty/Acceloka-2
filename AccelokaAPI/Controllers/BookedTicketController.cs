using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccelokaAPI.Features.BookedTickets.Queries;
using AccelokaAPI.Features.BookedTickets.Commands;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace AccelokaAPI.Controllers
{
    [Route("api/v1/booked-tickets")]
    [ApiController]
    public class BookedTicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<GetBookedTicketsQuery> _getValidator;
        private readonly IValidator<RevokeBookedTicketCommand> _revokeValidator;
        private readonly IValidator<EditBookedTicketCommand> _editValidator;

        public BookedTicketController(
            IMediator mediator,
            IValidator<GetBookedTicketsQuery> getValidator,
            IValidator<RevokeBookedTicketCommand> revokeValidator,
            IValidator<EditBookedTicketCommand> editValidator)
        {
            _mediator = mediator;
            _getValidator = getValidator;
            _revokeValidator = revokeValidator;
            _editValidator = editValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookedTickets([FromQuery] Guid bookedTicketIds)
        {
            var query = new GetBookedTicketsQuery(bookedTicketIds);
            ValidationResult validationResult = await _getValidator.ValidateAsync(query);

            if (!validationResult.IsValid)
            {
                Log.Warning("Invalid booking request received: {Errors}", validationResult.Errors);

                return BadRequest(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid Request",
                    Detail = "Request parameters are missing or invalid.",
                    Instance = HttpContext.Request.Path
                });
            }

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("{bookedTicketId}/{ticketCode}/{quantity}")]
        public async Task<IActionResult> RevokeBookedTicket(Guid bookedTicketId, string ticketCode, int quantity)
        {
            var command = new RevokeBookedTicketCommand(bookedTicketId, ticketCode, quantity);
            ValidationResult validationResult = await _revokeValidator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                Log.Warning("Invalid revoke request received: {Errors}", validationResult.Errors);

                return NotFound(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Status = StatusCodes.Status404NotFound,
                    Title = "Invalid Request",
                    Detail = "Request parameters are missing or invalid.",
                    Instance = HttpContext.Request.Path
                });
            }

            bool result = await _mediator.Send(command);

            if (!result)
            {
                Log.Warning("Revoke booked ticket failed for TicketCode: {TicketCode}, BookedTicketId: {BookedTicketId}", ticketCode, bookedTicketId);
                return NotFound(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Status = StatusCodes.Status404NotFound,
                    Title = "Invalid Request",
                    Detail = "Request parameters are missing or invalid.",
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(new
            {
                message = "Ticket successfully revoked.",
                ticketCode = ticketCode,
                quantityRevoked = quantity
            });
        }

        [HttpPut("{bookedTicketId}")]
        public async Task<IActionResult> EditBookedTicket(Guid bookedTicketId, [FromBody] List<TicketUpdateModel> tickets)
        {
            var command = new EditBookedTicketCommand(bookedTicketId, tickets);
            ValidationResult validationResult = await _editValidator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                Log.Warning("Invalid edit request received: {Errors}", validationResult.Errors);

                return NotFound(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Status = StatusCodes.Status404NotFound,
                    Title = "Invalid Request",
                    Detail = "Request body is missing or invalid.",
                    Instance = HttpContext.Request.Path
                });
            }

            bool result = await _mediator.Send(command);

            if (!result)
            {
                Log.Warning("Edit booked ticket failed for BookedTicketId: {BookedTicketId}", bookedTicketId);
                return NotFound(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Status = StatusCodes.Status404NotFound,
                    Title = "Invalid Request",
                    Detail = "Request parameters are missing or invalid.",
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(new
            {
                message = "Booked ticket successfully updated.",
                bookedTicketId = bookedTicketId,
                updatedTickets = tickets.Select(t => new
                {
                    ticketCode = t.KodeTicket,
                    newQuantity = t.Quantity
                })
            });
        }
    }
}
