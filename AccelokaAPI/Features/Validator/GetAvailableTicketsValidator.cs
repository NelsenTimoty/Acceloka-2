using FluentValidation;
using AccelokaAPI.Features.Tickets.Queries;

namespace AccelokaAPI.Features.Tickets.Validators
{
    public class GetAvailableTicketsValidator : AbstractValidator<GetAvailableTicketsQuery>
    {
        public GetAvailableTicketsValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Max price must be a positive number.");

            RuleFor(x => x.MinEventDate)
                .LessThan(x => x.MaxEventDate)
                .When(x => x.MinEventDate.HasValue && x.MaxEventDate.HasValue)
                .WithMessage("MinEventDate must be before MaxEventDate.");
        }
    }
}
