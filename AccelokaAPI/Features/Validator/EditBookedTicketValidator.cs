using FluentValidation;
using AccelokaAPI.Features.BookedTickets.Commands;
using System.Data;

namespace AccelokaAPI.Features.BookedTickets.Validators
{
    public class EditBookedTicketValidator : AbstractValidator<EditBookedTicketCommand>
    {
        public EditBookedTicketValidator()
        {
            RuleFor(x => x.BookedTicketId).NotEmpty().WithMessage("Booked Ticket ID is required.");
            RuleFor(x => x.Tickets)
                .NotNull().WithMessage("Tickets list is required.")
                .NotEmpty().WithMessage("Tickets list cannot be empty.");
        }
    }
}
