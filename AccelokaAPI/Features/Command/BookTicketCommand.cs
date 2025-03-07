using MediatR;
using AccelokaAPI.DTOs;
using AccelokaAPI.Models;

namespace AccelokaAPI.Commands
{
    public class BookTicketCommand : IRequest<BookingResponse>
    {
        public BookingRequest Request { get; }

        public BookTicketCommand(BookingRequest request)
        {
            Request = request;
        }
    }
}
