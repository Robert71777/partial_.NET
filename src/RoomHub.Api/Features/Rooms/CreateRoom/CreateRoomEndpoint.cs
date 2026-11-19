using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public static class CreateRoomEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/", Handle).WithName("CreateRoom");

    private static async Task<IResult> Handle(
        CreateRoomCommand command,
        ICommandHandler<CreateRoomCommand, Room> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblem();

        var room = result.Value;
        return Results.Created(
            $"/api/rooms/{room.Id}",
            new { room.Id, room.Name, room.Capacity });
    }
}
