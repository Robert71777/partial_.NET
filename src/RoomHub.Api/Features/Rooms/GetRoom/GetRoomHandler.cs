using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed class GetRoomHandler(IRoomRepository roomRepository) : IQueryHandler<GetRoomQuery, Room?>
{
    public async Task<Room?> HandleAsync(
        GetRoomQuery query,
        CancellationToken cancellationToken = default) =>
        await roomRepository.GetByIdAsync(query.Id, cancellationToken);
}
