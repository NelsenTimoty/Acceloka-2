using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AccelokaAPI.Repositories;
using AccelokaAPI.Features.BookedTickets.Commands;

namespace AccelokaAPI.Features.BookedTickets.Handlers
{
    public class EditBookedTicketHandler : IRequestHandler<EditBookedTicketCommand, bool>
    {
        private readonly IBookedTicketRepository _bookedTicketRepository;

        public EditBookedTicketHandler(IBookedTicketRepository bookedTicketRepository)
        {
            _bookedTicketRepository = bookedTicketRepository;
        }

        public async Task<bool> Handle(EditBookedTicketCommand request, CancellationToken cancellationToken)
        {
            return await _bookedTicketRepository.EditBookedTicket(request.BookedTicketId, request.Tickets);
        }
    }
}
