using RoomHub.Api.Common;

namespace RoomHub.Api.Domain.Bookings;

public static class BookingErrors
{
    public static readonly Error BookedByRequired =
        Error.Validation("Booking.BookedByRequired", "BookedBy is required.");

    public static readonly Error EndMustBeAfterStart =
        Error.Validation("Booking.EndMustBeAfterStart", "End time must be strictly after start time.");

    public static readonly Error AttendeesMustBePositive =
        Error.Validation("Booking.AttendeesMustBePositive", "Attendees must be strictly positive.");

    public static readonly Error AttendeesExceedCapacity =
        Error.Validation("Booking.AttendeesExceedCapacity", "Attendees exceed the room capacity.");

    public static readonly Error StartInThePast =
        Error.Validation("Booking.StartInThePast", "A booking cannot start in the past.");

    public static readonly Error DurationExceedsFourHours =
        Error.Validation("Booking.DurationExceedsFourHours", "A booking cannot last more than 4 hours.");

    public static readonly Error DurationExceedsRoomLimit =
        Error.Validation("Booking.DurationExceedsRoomLimit", "A booking for a room with capacity greater than 10 cannot last more than 2 hours.");

    public static readonly Error RoomOccupied =
        Error.Conflict("Booking.RoomOccupied", "The room is already booked in this interval.");

    public static readonly Error AlreadyCancelled =
        Error.Conflict("Booking.AlreadyCancelled", "The booking is already cancelled.");

    public static readonly Error CancellationTooLate =
        Error.Conflict("Booking.CancellationTooLate", "A booking can only be cancelled at least 60 minutes before it starts.");

    public static readonly Error NotFound =
        Error.NotFound("Booking.NotFound", "The booking does not exist.");

    public static readonly Error DailyLimitExceeded =
        Error.Conflict("Booking.DailyLimitExceeded", "A participant may not have more than 2 active bookings on the same day.");
}
