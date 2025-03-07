using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AccelokaAPI.Repositories;
using AccelokaAPI.Features.BookedTickets.Commands;
using AccelokaAPI.Models;

namespace AccelokaAPI.Features.BookedTickets.Handlers
{
    public class RevokeBookedTicketHandler : IRequestHandler<RevokeBookedTicketCommand, bool>
    {
        private readonly IBookedTicketRepository _bookedTicketRepository;

        public RevokeBookedTicketHandler(IBookedTicketRepository bookedTicketRepository)
        {
            _bookedTicketRepository = bookedTicketRepository;
        }

        public async Task<bool> Handle(RevokeBookedTicketCommand request, CancellationToken cancellationToken)
        {
            var bookedTicket = await _bookedTicketRepository.GetBookedTicketById(request.BookedTicketId);
            if (bookedTicket == null) return false;

            return await _bookedTicketRepository.RevokeBookedTicket(request.BookedTicketId, request.TicketCode, request.Quantity);
        }
    }
}
