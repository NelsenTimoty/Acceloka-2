using AccelokaAPI.Data;
using AccelokaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccelokaAPI.Repositories
{
    public class BookedTicketRepository : IBookedTicketRepository
        {
            private readonly ApplicationDbContext _context;

            public BookedTicketRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<BookedTicketDetailDto>> GetBookedTicketDetails(List<Guid> bookedTicketIds)
            {
                return await _context.BookedTickets
                    .Where(bt => bookedTicketIds.Contains(bt.Id))  // Accept multiple GUIDs
                    .SelectMany(bt => bt.BookedTicketDetails)
                    .Select(d => new BookedTicketDetailDto
                    {
                        TicketCode = d.Ticket.Code,
                        TicketName = d.Ticket.Name,
                        EventDate = d.Ticket.EventDate,
                        Quantity = d.Quantity,
                        CategoryName = d.Ticket.Category.CategoryName
                    })
                    .ToListAsync();
            }


            // public async Task<List<BookedTicketDetailDto>> GetBookedTicketDetails(List<Guid> bookedTicketIds)
            // {
            //     return await _context.BookedTickets
            //         .Where(bt => bookedTicketIds.Contains(bt.Id))  // Accept multiple GUIDs
            //         .SelectMany(bt => bt.BookedTicketDetails)
            //         .Select(d => new BookedTicketDetailDto
            //         {
            //             TicketCode = d.Ticket.Code,
            //             TicketName = d.Ticket.Name,
            //             EventDate = d.Ticket.EventDate,
            //             Quantity = d.Quantity,
            //             CategoryName = d.Ticket.Category.CategoryName
            //         })
            //         .ToListAsync();
            // }
            public async Task<BookedTicket> GetBookedTicketById(Guid bookedTicketId)
            {
                return await _context.BookedTickets
                    .Include(bt => bt.BookedTicketDetails)
                    .ThenInclude(d => d.Ticket)
                    .ThenInclude(t => t.Category)
                    .FirstOrDefaultAsync(bt => bt.Id == bookedTicketId);
            }

            public async Task UpdateBookedTicket(BookedTicket bookedTicket)
            {
                _context.BookedTickets.Update(bookedTicket);
                await _context.SaveChangesAsync();
            }
            public async Task<bool> RevokeBookedTicket(Guid bookedTicketId, string ticketCode, int quantity)
            {
                var bookedTicket = await _context.BookedTickets
                    .Include(bt => bt.BookedTicketDetails)
                    .FirstOrDefaultAsync(bt => bt.Id == bookedTicketId);

                if (bookedTicket == null)
                    return false;

                var ticketDetail = bookedTicket.BookedTicketDetails
                    .FirstOrDefault(td => td.Ticket.Code == ticketCode);

                if (ticketDetail == null || ticketDetail.Quantity < quantity)
                    return false;

                // Adjust the quantity or remove if 0
                ticketDetail.Quantity -= quantity;
                if (ticketDetail.Quantity == 0)
                {
                    _context.BookedTicketDetails.Remove(ticketDetail);
                }

                // Remove booked ticket if no details left
                if (!bookedTicket.BookedTicketDetails.Any())
                {
                    _context.BookedTickets.Remove(bookedTicket);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            public async Task<int?> GetRemainingTicketQuota(string ticketCode)
            {
                return await _context.Tickets
                    .Where(t => t.Code == ticketCode)
                    .Select(t => (int?)t.Quota)
                    .FirstOrDefaultAsync() ?? 0;
            }
            public async Task<bool> EditBookedTicket(Guid bookedTicketId, List<TicketUpdateModel> tickets)
            {
                var bookedTicket = await _context.BookedTickets
                    .Include(bt => bt.BookedTicketDetails)
                    .ThenInclude(btd => btd.Ticket) // Ensure Ticket is loaded
                    .FirstOrDefaultAsync(bt => bt.Id == bookedTicketId);

                foreach (var item in tickets)
                {
                   var bookedDetail = bookedTicket.BookedTicketDetails
                    .FirstOrDefault(d => d.Ticket != null && d.Ticket.Code == item.KodeTicket);

                if (bookedDetail != null)
                {
                    int oldQuantity = bookedDetail.Quantity;  // Previous quantity
                    int newQuantity = item.Quantity;  // New quantity from request
                    int difference = newQuantity - oldQuantity;

                    // Get the ticket
                    var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Code == item.KodeTicket);
                    if (ticket == null)
                    {
                        throw new KeyNotFoundException($"Ticket with code {item.KodeTicket} not found.");
                    }

                    // ✅ Check if new quantity exceeds available quota
                    if (difference > 0 && difference > ticket.Quota)
                    {
                        throw new InvalidOperationException($"Not enough quota available for ticket {item.KodeTicket}. Remaining: {ticket.Quota}");
                    }

                    // Update the booked quantity
                    bookedDetail.Quantity = newQuantity;

                    // Update ticket quota
                    ticket.Quota -= difference; // Adjust quota based on change
                }
                }
                await _context.SaveChangesAsync();
                return true;
            }
        }

}
