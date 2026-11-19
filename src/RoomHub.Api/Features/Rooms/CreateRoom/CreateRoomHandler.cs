using RoomHub.Api.Common;
using RoomHub.Api.Domain;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed class CreateRoomHandler(
    IRoomRepository roomRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateRoomCommand, Room>
{
    public async Task<Result<Room>> HandleAsync(
        CreateRoomCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = Room.Create(command.Name, command.Capacity);
        if (result.IsFailure)
            return Result<Room>.Failure(result.Error);

        await roomRepository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return result;
    }
}
