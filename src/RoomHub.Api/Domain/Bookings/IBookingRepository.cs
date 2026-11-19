namespace RoomHub.Api.Domain.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);

    /// <summary>Returns only Active bookings for the given room.</summary>
    Task<IReadOnlyList<Booking>> ListActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts Active bookings for <paramref name="bookedBy"/> whose startUtc falls
    /// on the UTC date of <paramref name="dayUtc"/>, across all rooms.
    /// </summary>
    Task<int> CountActiveForBookerOnDayAsync(string bookedBy, DateTime dayUtc, CancellationToken cancellationToken = default);
}
