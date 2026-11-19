using RoomHub.Api.Common;
using RoomHub.Api.Domain;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed class CancelBookingHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<CancelBookingCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(
        CancelBookingCommand command,
        CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(command.Id, cancellationToken);
        if (booking is null)
            return Result<Unit>.Failure(BookingErrors.NotFound);

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var cancelResult = booking.Cancel(nowUtc);
        if (cancelResult.IsFailure)
            return cancelResult;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return cancelResult;
    }
}
