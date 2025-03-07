using MediatR;
using AccelokaAPI.Repositories;
using AccelokaAPI.DTOs;
using AccelokaAPI.Models;
using AccelokaAPI.Commands;

namespace AccelokaAPI.Handlers
{
    public class BookTicketHandler : IRequestHandler<BookTicketCommand, BookingResponse>
    {
        private readonly IBookingRepository _bookingRepository;

        public BookTicketHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingResponse> Handle(BookTicketCommand request, CancellationToken cancellationToken)
        {
            return await _bookingRepository.BookTicketAsync(request.Request);
        }
    }
}
