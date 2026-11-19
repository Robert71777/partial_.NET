using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed record GetBookingQuery(Guid Id) : IQuery<Booking?>;
