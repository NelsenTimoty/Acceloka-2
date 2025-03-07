using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccelokaAPI.Repositories;
using AccelokaAPI.Models;
using AccelokaAPI.Features.BookedTickets.Queries;

namespace AccelokaAPI.Features.BookedTickets.Handlers
{
    public class GetBookedTicketsHandler : IRequestHandler<GetBookedTicketsQuery, List<BookedTicketDetailDto>>
    {
        private readonly IBookedTicketRepository _bookedTicketRepository;

        public GetBookedTicketsHandler(IBookedTicketRepository bookedTicketRepository)
        {
            _bookedTicketRepository = bookedTicketRepository;
        }

        public async Task<List<BookedTicketDetailDto>> Handle(GetBookedTicketsQuery request, CancellationToken cancellationToken)
        {
            return await _bookedTicketRepository.GetBookedTicketDetails(new List<Guid> { request.BookedTicketIds });
        }
    }
}
