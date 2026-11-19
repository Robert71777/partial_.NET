using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common;
using RoomHub.Api.Domain;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Bookings.GetBooking;
using RoomHub.Api.Features.Rooms.CreateRoom;
using RoomHub.Api.Features.Rooms.GetRoom;
using RoomHub.Api.Infrastructure.Persistence;
using RoomHub.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// EF Core + SQLite
builder.Services.AddDbContext<RoomHubDbContext>(options =>
    options.UseSqlite("Data Source=roomhub.db"));

// IUnitOfWork implemented by RoomHubDbContext
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RoomHubDbContext>());

// Repositories
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// TimeProvider (R5 — never use DateTime.UtcNow directly)
builder.Services.AddSingleton(TimeProvider.System);

// CQRS handlers — registered automatically by reflection
builder.Services.AddHandlers(typeof(Program).Assembly);

// JSON: camelCase + enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter());
});

var app = builder.Build();

// Ensure the SQLite database and schema exist on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoomHubDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => "RoomHub API is running.");

var rooms = app.MapGroup("/api/rooms").WithTags("Rooms");
CreateRoomEndpoint.Map(rooms);
GetRoomEndpoint.Map(rooms);
CreateBookingEndpoint.Map(rooms);

var bookings = app.MapGroup("/api/bookings").WithTags("Bookings");
GetBookingEndpoint.Map(bookings);
CancelBookingEndpoint.Map(bookings);

app.Run();
