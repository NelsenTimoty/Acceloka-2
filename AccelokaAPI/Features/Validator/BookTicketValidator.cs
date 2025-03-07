using FluentValidation;
using AccelokaAPI.DTOs;
using AccelokaAPI.Models;

namespace AccelokaAPI.Validator
{
    public class BookTicketValidator : AbstractValidator<BookingRequest>
    {
        public BookTicketValidator()
        {
            RuleFor(request => request.Tickets)
                .NotEmpty().WithMessage("Ticket list cannot be empty.");

            RuleForEach(request => request.Tickets).ChildRules(ticket =>
            {
                ticket.RuleFor(t => t.TicketCode)
                    .NotEmpty().WithMessage("Ticket code is required.");

                ticket.RuleFor(t => t.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            });
        }
    }
}
