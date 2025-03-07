using AccelokaAPI.Data;
using AccelokaAPI.Models;
using AccelokaAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using AccelokaAPI.Repositories;
using Serilog;

namespace AccelokaAPI.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BookingResponse> BookTicketAsync(BookingRequest request)
        {
            Log.Information("Processing booking request: {@Request}", request);
            
            var ticketCodes = request.Tickets.Select(t => t.TicketCode).ToList();
            var tickets = await _context.Tickets
                                        .Include(t => t.Category)
                                        .Where(t => ticketCodes.Contains(t.Code))
                                        .ToListAsync();

            if (tickets.Count != ticketCodes.Count)
            {
                Log.Warning("One or more ticket codes are invalid.");
                throw new KeyNotFoundException("One or more ticket codes are invalid.");
            }

            var booking = new BookedTicket();
            var categorySummaries = new Dictionary<string, CategoryBookingSummary>();
            decimal totalBookingPrice = 0;

            foreach (var ticketRequest in request.Tickets)
            {
                var ticket = tickets.FirstOrDefault(t => t.Code == ticketRequest.TicketCode);
                if (ticket == null) continue;

                if (ticket.Quota < ticketRequest.Quantity)
                {
                    Log.Warning("Insufficient quota for ticket {TicketCode}.", ticket.Code);
                    throw new InvalidOperationException($"Insufficient quota for ticket {ticket.Code}.");
                }

                if (ticket.EventDate <= DateTime.UtcNow)
                {
                    Log.Warning("Attempt to book a past event ticket: {TicketCode}", ticket.Code);
                    throw new InvalidOperationException($"Cannot book past event ticket {ticket.Code}.");
                }

                decimal totalPrice = ticket.Price * ticketRequest.Quantity;
                ticket.Quota -= ticketRequest.Quantity;

                booking.BookedTicketDetails.Add(new BookedTicketDetail
                {
                    TicketId = ticket.Id,
                    Quantity = ticketRequest.Quantity,
                    TotalPrice = totalPrice
                });

                string categoryName = ticket.Category?.CategoryName ?? "Unknown";
                if (!categorySummaries.ContainsKey(categoryName))
                {
                    categorySummaries[categoryName] = new CategoryBookingSummary
                    {
                        CategoryName = categoryName,
                        SummaryPrice = 0,
                        Tickets = new List<TicketBookingDetail>()
                    };
                }

                categorySummaries[categoryName].Tickets.Add(new TicketBookingDetail
                {
                    TicketCode = ticket.Code,
                    TicketName = ticket.Name,
                    Price = totalPrice
                });

                categorySummaries[categoryName].SummaryPrice += totalPrice;
                totalBookingPrice += totalPrice;
            }

            _context.BookedTickets.Add(booking);
            await _context.SaveChangesAsync();

            Log.Information("Booking successful. Total price: {TotalPrice}, Booking ID: {BookingId}",
                            totalBookingPrice, booking.Id);

            return new BookingResponse
            {
                PriceSummary = totalBookingPrice,
                TicketsPerCategories = categorySummaries.Values.ToList(),
                BookingId = booking.Id,
            };
        }
    }
}
