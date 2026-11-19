using RoomHub.Api.Common;

namespace RoomHub.Api.Domain.Rooms;

public sealed class Room
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }

    private Room() { }

    /// <summary>R1: name non-empty (trimmed), capacity strictly positive.</summary>
    public static Result<Room> Create(string? name, int capacity)
    {
        var trimmed = name?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(trimmed))
            return Result<Room>.Failure(RoomErrors.NameRequired);

        if (capacity <= 0)
            return Result<Room>.Failure(RoomErrors.CapacityMustBePositive);

        return Result<Room>.Success(new Room
        {
            Id = Guid.NewGuid(),
            Name = trimmed,
            Capacity = capacity
        });
    }
}
