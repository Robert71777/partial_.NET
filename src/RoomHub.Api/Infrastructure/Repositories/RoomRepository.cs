using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Infrastructure.Repositories;

public sealed class RoomRepository(RoomHubDbContext db) : IRoomRepository
{
    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await db.Rooms.FindAsync([id], cancellationToken);

    public async Task AddAsync(Room room, CancellationToken cancellationToken = default) =>
        await db.Rooms.AddAsync(room, cancellationToken);
}
