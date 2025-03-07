using MediatR;
using AccelokaAPI.Repositories;
using AccelokaAPI.DTOs;
using AccelokaAPI.Features.Tickets.Queries;

namespace AccelokaAPI.Features.Tickets.Handlers
{
    public class GetAvailableTicketsHandler : IRequestHandler<GetAvailableTicketsQuery, PaginatedResponse<TicketDTO>>
    {
        private readonly ITicketRepository _repository;

        public GetAvailableTicketsHandler(ITicketRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<TicketDTO>> Handle(GetAvailableTicketsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAvailableTickets(
                request.Category, request.Code, request.Name,
                request.MaxPrice, request.MinEventDate, request.MaxEventDate,
                request.OrderBy, request.OrderState, request.Page
            );
        }
    }
}
