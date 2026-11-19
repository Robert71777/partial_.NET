using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public static class GetRoomEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapGet("/{id:guid}", Handle).WithName("GetRoom");

    private static async Task<IResult> Handle(
        Guid id,
        IQueryHandler<GetRoomQuery, Room?> handler,
        CancellationToken cancellationToken)
    {
        var room = await handler.HandleAsync(new GetRoomQuery(id), cancellationToken);

        if (room is null)
            return RoomErrors.NotFound.ToProblem();

        return Results.Ok(new { room.Id, room.Name, room.Capacity });
    }
}
