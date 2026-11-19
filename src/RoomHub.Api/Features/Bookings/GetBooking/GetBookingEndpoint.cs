using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public static class GetBookingEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapGet("/{id:guid}", Handle).WithName("GetBooking");

    private static async Task<IResult> Handle(
        Guid id,
        IQueryHandler<GetBookingQuery, Booking?> handler,
        CancellationToken cancellationToken)
    {
        var booking = await handler.HandleAsync(new GetBookingQuery(id), cancellationToken);

        if (booking is null)
            return BookingErrors.NotFound.ToProblem();

        return Results.Ok(new
        {
            booking.Id,
            booking.RoomId,
            booking.BookedBy,
            booking.StartUtc,
            booking.EndUtc,
            booking.Attendees,
            Status = booking.Status.ToString()
        });
    }
}
