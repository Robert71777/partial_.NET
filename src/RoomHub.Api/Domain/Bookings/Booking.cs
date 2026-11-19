using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public enum BookingStatus
{
    Active,
    Cancelled
}

public sealed class Booking
{
    public const int MaxActivePerDay = 2;

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    private Booking() { }

    /// <summary>
    /// R2: bookedBy required.
    /// R3: endUtc strictly after startUtc.
    /// R4: attendees > 0 and &lt;= room.Capacity.
    /// R5: startUtc not before nowUtc.
    /// R6: duration &lt;= 4 hours (exactly 4 h is allowed).
    /// </summary>
    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        DateTime nowUtc)
    {
        var trimmedBookedBy = bookedBy?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(trimmedBookedBy))
            return Result<Booking>.Failure(BookingErrors.BookedByRequired);

        if (endUtc <= startUtc)
            return Result<Booking>.Failure(BookingErrors.EndMustBeAfterStart);

        if (attendees <= 0)
            return Result<Booking>.Failure(BookingErrors.AttendeesMustBePositive);

        if (attendees > room.Capacity)
            return Result<Booking>.Failure(BookingErrors.AttendeesExceedCapacity);

        if (startUtc < nowUtc)
            return Result<Booking>.Failure(BookingErrors.StartInThePast);

        // R6 / R10: max duration depends on room capacity
        var maxDuration = room.Capacity > 10
            ? TimeSpan.FromHours(2)
            : TimeSpan.FromHours(4);
        var durationError = room.Capacity > 10
            ? BookingErrors.DurationExceedsRoomLimit
            : BookingErrors.DurationExceedsFourHours;

        if ((endUtc - startUtc) > maxDuration)
            return Result<Booking>.Failure(durationError);

        return Result<Booking>.Success(new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            BookedBy = trimmedBookedBy,
            StartUtc = startUtc,
            EndUtc = endUtc,
            Attendees = attendees,
            Status = BookingStatus.Active
        });
    }

    /// <summary>
    /// R8: Active → Cancelled. Returns Conflict if already Cancelled.
    /// </summary>
    public Result<Unit> Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return Result<Unit>.Failure(BookingErrors.AlreadyCancelled);

        Status = BookingStatus.Cancelled;
        return Result<Unit>.Success(Unit.Value);
    }

    /// <summary>
    /// R7: touching intervals (one ends exactly when the other starts) do not overlap.
    /// </summary>
    public bool Overlaps(DateTime startUtc, DateTime endUtc) =>
        StartUtc < endUtc && startUtc < EndUtc;

    /// <summary>
    /// New requirement: a participant may not have more than
    /// <see cref="MaxActivePerDay"/> active bookings on the same UTC day.
    /// </summary>
    public static Result<Unit> EnsureDailyLimit(int activeBookingsThatDay)
    {
        if (activeBookingsThatDay >= MaxActivePerDay)
            return Result<Unit>.Failure(BookingErrors.DailyLimitExceeded);

        return Result<Unit>.Success(Unit.Value);
    }
}
