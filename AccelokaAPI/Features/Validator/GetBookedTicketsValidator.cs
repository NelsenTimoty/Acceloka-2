using FluentValidation;
using AccelokaAPI.Features.BookedTickets.Queries;

namespace AccelokaAPI.Features.BookedTickets.Validators
{
    public class GetBookedTicketsValidator : AbstractValidator<GetBookedTicketsQuery>
    {
        public GetBookedTicketsValidator()
        {
            RuleFor(x => x.BookedTicketIds)
                .NotNull().WithMessage("Booked Ticket IDs are required.")
                .NotEmpty().WithMessage("Booked Ticket IDs list cannot be empty.");
        }
    }
}
