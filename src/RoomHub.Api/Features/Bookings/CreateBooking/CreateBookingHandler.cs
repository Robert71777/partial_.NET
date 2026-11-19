using RoomHub.Api.Common;
using RoomHub.Api.Domain;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed class CreateBookingHandler(
    IRoomRepository roomRepository,
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<CreateBookingCommand, Booking>
{
    public async Task<Result<Booking>> HandleAsync(
        CreateBookingCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Load room — 404 before any validation (per contract)
        var room = await roomRepository.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return Result<Booking>.Failure(RoomErrors.NotFound);

        // 2. Create booking (enforces R2–R6)
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var bookingResult = Booking.Create(
            room,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            nowUtc);

        if (bookingResult.IsFailure)
            return Result<Booking>.Failure(bookingResult.Error);

        // 3. Check R7: no overlap with existing active bookings
        var activeBookings = await bookingRepository.ListActiveForRoomAsync(command.RoomId, cancellationToken);
        var hasOverlap = activeBookings.Any(b => b.Overlaps(command.StartUtc, command.EndUtc));
        if (hasOverlap)
            return Result<Booking>.Failure(BookingErrors.RoomOccupied);

        // 4. Check new requirement: max 2 active bookings per bookedBy per UTC day
        var bookedBy = command.BookedBy!.Trim();
        var activeOnDay = await bookingRepository.CountActiveForBookerOnDayAsync(
            bookedBy, command.StartUtc, cancellationToken);
        var limitResult = Booking.EnsureDailyLimit(activeOnDay);
        if (limitResult.IsFailure)
            return Result<Booking>.Failure(limitResult.Error);

        // 5. Persist
        await bookingRepository.AddAsync(bookingResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return bookingResult;
    }
}
