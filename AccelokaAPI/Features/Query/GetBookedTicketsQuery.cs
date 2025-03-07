using MediatR;
using System;
using System.Collections.Generic;
using AccelokaAPI.Models;

namespace AccelokaAPI.Features.BookedTickets.Queries
{
    public class GetBookedTicketsQuery : IRequest<List<BookedTicketDetailDto>>
    {
        public Guid BookedTicketIds { get; set; }

        public GetBookedTicketsQuery(Guid bookedTicketIds)
        {
            BookedTicketIds = bookedTicketIds;
        }
    }
}
