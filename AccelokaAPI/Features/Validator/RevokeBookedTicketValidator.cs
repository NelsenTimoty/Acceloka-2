using FluentValidation;
using AccelokaAPI.Features.BookedTickets.Commands;

namespace AccelokaAPI.Features.BookedTickets.Validators
{
    public class RevokeBookedTicketValidator : AbstractValidator<RevokeBookedTicketCommand>
    {
        public RevokeBookedTicketValidator()
        {
            RuleFor(x => x.BookedTicketId).NotEmpty().WithMessage("Booked Ticket ID is required.");
            RuleFor(x => x.TicketCode).NotEmpty().WithMessage("Ticket Code is required.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
