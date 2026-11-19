using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public static class CreateBookingEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/{roomId:guid}/bookings", Handle).WithName("CreateBooking");

    private static async Task<IResult> Handle(
        Guid roomId,
        CreateBookingRequest request,
        ICommandHandler<CreateBookingCommand, Booking> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateBookingCommand(
            roomId,
            request.BookedBy,
            request.StartUtc,
            request.EndUtc,
            request.Attendees);

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblem();

        var booking = result.Value;
        return Results.Created(
            $"/api/bookings/{booking.Id}",
            new { booking.Id, booking.RoomId, Status = booking.Status.ToString() });
    }
}

/// <summary>Separate request body record so roomId comes from the route, not the body.</summary>
public sealed record CreateBookingRequest(
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees);
