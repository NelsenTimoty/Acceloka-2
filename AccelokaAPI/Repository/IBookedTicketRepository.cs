using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccelokaAPI.Models;

namespace AccelokaAPI.Repositories
{
    public interface IBookedTicketRepository
    {
        Task<List<BookedTicketDetailDto>> GetBookedTicketDetails(List<Guid> bookedTicketIds);
        Task<BookedTicket> GetBookedTicketById(Guid bookedTicketId);
        Task<bool> RevokeBookedTicket(Guid bookedTicketId, string ticketCode, int quantity);
        Task<bool> EditBookedTicket(Guid bookedTicketId, List<TicketUpdateModel> tickets);
    }
}
