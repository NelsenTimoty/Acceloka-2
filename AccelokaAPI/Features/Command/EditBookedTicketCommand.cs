using MediatR;
using System;
using System.Collections.Generic;
using AccelokaAPI.Models;

namespace AccelokaAPI.Features.BookedTickets.Commands
{
    public class EditBookedTicketCommand : IRequest<bool>
    {
        public Guid BookedTicketId { get; set; }
        public List<TicketUpdateModel> Tickets { get; set; }

        public EditBookedTicketCommand(Guid bookedTicketId, List<TicketUpdateModel> tickets)
        {
            BookedTicketId = bookedTicketId;
            Tickets = tickets;
        }
    }
}
