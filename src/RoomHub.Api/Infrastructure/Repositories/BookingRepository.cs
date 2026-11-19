using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Infrastructure.Repositories;

public sealed class BookingRepository(RoomHubDbContext db) : IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await db.Bookings.FindAsync([id], cancellationToken);

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default) =>
        await db.Bookings.AddAsync(booking, cancellationToken);

    public async Task<IReadOnlyList<Booking>> ListActiveForRoomAsync(
        Guid roomId,
        CancellationToken cancellationToken = default) =>
        await db.Bookings
                .Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active)
                .ToListAsync(cancellationToken);

    public async Task<int> CountActiveForBookerOnDayAsync(
        string bookedBy,
        DateTime dayUtc,
        CancellationToken cancellationToken = default)
    {
        var dayStart = dayUtc.Date;
        var dayEnd   = dayStart.AddDays(1);

        return await db.Bookings
                       .CountAsync(b => b.Status     == BookingStatus.Active
                                     && b.BookedBy   == bookedBy
                                     && b.StartUtc   >= dayStart
                                     && b.StartUtc   <  dayEnd,
                                   cancellationToken);
    }
}
