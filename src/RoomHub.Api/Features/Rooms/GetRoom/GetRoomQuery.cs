using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record GetRoomQuery(Guid Id) : IQuery<Room?>;
