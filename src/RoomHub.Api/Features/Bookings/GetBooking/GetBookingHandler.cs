using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed class GetBookingHandler(IBookingRepository bookingRepository)
    : IQueryHandler<GetBookingQuery, Booking?>
{
    public async Task<Booking?> HandleAsync(
        GetBookingQuery query,
        CancellationToken cancellationToken = default) =>
        await bookingRepository.GetByIdAsync(query.Id, cancellationToken);
}
