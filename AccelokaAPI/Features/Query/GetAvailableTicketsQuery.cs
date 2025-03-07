using MediatR;
using AccelokaAPI.DTOs;

namespace AccelokaAPI.Features.Tickets.Queries
{
    public record GetAvailableTicketsQuery(
        string? Category,
        string? Code,
        string? Name,
        decimal? MaxPrice,
        DateTime? MinEventDate,
        DateTime? MaxEventDate,
        string OrderBy = "Code",
        string OrderState = "asc",
        int Page = 1
    ) : IRequest<PaginatedResponse<TicketDTO>>;
}
