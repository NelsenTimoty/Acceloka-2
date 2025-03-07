using MediatR;
using System;

namespace AccelokaAPI.Features.BookedTickets.Commands
{
    public class RevokeBookedTicketCommand : IRequest<bool>
    {
        public Guid BookedTicketId { get; set; }
        public string TicketCode { get; set; }
        public int Quantity { get; set; }

        public RevokeBookedTicketCommand(Guid bookedTicketId, string ticketCode, int quantity)
        {
            BookedTicketId = bookedTicketId;
            TicketCode = ticketCode;
            Quantity = quantity;
        }
    }
}
