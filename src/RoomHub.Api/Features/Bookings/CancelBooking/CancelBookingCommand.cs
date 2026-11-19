using RoomHub.Api.Common;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed record CancelBookingCommand(Guid Id) : ICommand<Unit>;
