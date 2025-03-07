using System.Threading.Tasks;
using AccelokaAPI.DTOs;
using AccelokaAPI.Models;

namespace AccelokaAPI.Repositories
{
    public interface IBookingRepository
    {
        Task<BookingResponse> BookTicketAsync(BookingRequest request);
    }
}
