# CampusHub — Arhitectură și Convenții (.NET 2026)

Acest fișier descrie pattern-urile și convențiile folosite în proiect.
Respectă-le la orice cod generat sau modificat.

---

## 1. Result Pattern

Erori de business sunt reprezentate prin `Result<T>`, nu excepții.

```csharp
// IsSuccess când _error e null, IsFailure altfel
public sealed class Result<T>
{
    public bool IsSuccess => _error is null;
    public bool IsFailure => _error is not null;

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException(...);
    public Error Error => IsFailure ? _error! : throw new InvalidOperationException(...);

    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(Error error) => new(default, error);

    public static implicit operator Result<T>(T value) => Success(value);
}
```

**Unit** — echivalentul `void` când comanda nu returnează date:
```csharp
public sealed class Unit
{
    public static readonly Unit Value = new();
    private Unit() { }
}
```

---

## 2. Error + ErrorType

```csharp
public enum ErrorType { Validation, NotFound, Conflict }

public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
    public static Error NotFound(string code, string message)   => new(code, message, ErrorType.NotFound);
    public static Error Conflict(string code, string message)   => new(code, message, ErrorType.Conflict);
}
```

Erorile pentru o entitate se pun într-o clasă statică dedicată:
```csharp
// Domain/Events/EventErrors.cs
public static class EventErrors
{
    public static readonly Error TitleRequired         = Error.Validation("Event.TitleRequired", "...");
    public static readonly Error EventAlreadyPublished = Error.Conflict("Event.AlreadyPublished", "...");
    public static readonly Error EventAlreadyCancelled = Error.Conflict("Event.AlreadyCancelled", "...");
}
```

---

## 3. CQRS manual (fără MediatR)

```csharp
public interface ICommand<TResult> { }
public interface IQuery<TResult>   { }

// Handler comandă — returnează Result<TResult>
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

// Handler query — returnează direct TResult (fără Result<>)
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
```

Handler-ele se înregistrează automat prin reflection — nu le adăuga manual în DI:
```csharp
builder.Services.AddHandlers(typeof(Program).Assembly);
```

---

## 4. Value Object — reguli

- `sealed record`, constructor `private`, factory static care returnează `Result<T>`.
- Exemplu: `DateRange.Create(startDate, endDate)` convertește la UTC și validează că start < end.
- EF Core îl mapează ca `ComplexProperty`, nu ca entitate separată.

---

## 5. Domain Entity — reguli

- `sealed class`, constructor `private` (pentru EF + encapsulare).
- Toate proprietățile au `private set`.
- Colecții expuse ca `IReadOnlyCollection<T>` — backing field `List<T>` privat.
- Factory static `Create(...)` returnează `Result<Entity>`.
- Metode de business returnează `Result<Unit>` sau `Result<T>`.

---

## 6. Vertical Slice — structura unui feature

```
Feature/
  Events/
    CreateEvent/
      CreateEventCommand.cs    ← record ce implementează ICommand<Event>
      CreateEventHandler.cs    ← ICommandHandler<CreateEventCommand, Event>
      CreateEventEndpoint.cs   ← static class, Map(RouteGroupBuilder)
    PublishEvent/
      ...
  Registrations/
    RegisterForEvent/
      ...
```

---

## 7. Minimal API Endpoint — pattern

```csharp
public static class CreateEventEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/", Handle).WithName("CreateEvent");

    private static async Task<IResult> Handle(
        CreateEventCommand command,
        ICommandHandler<CreateEventCommand, Event> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return Results.BadRequest(new { result.Error.Code, result.Error.Message });

        return Results.Created($"/events/{result.Value.Id}", result.Value);
    }
}
```

Mapare ErrorType → HTTP:

| ErrorType  | HTTP Result            |
|------------|------------------------|
| Validation | `Results.BadRequest()` |
| NotFound   | `Results.NotFound()`   |
| Conflict   | `Results.Conflict()`   |

Când sunt mai multe tipuri de erori posibile, folosește `switch expression`:
```csharp
return result.Error.Type switch
{
    ErrorType.NotFound => Results.NotFound(new { result.Error.Code, result.Error.Message }),
    _                  => Results.BadRequest(new { result.Error.Code, result.Error.Message })
};
```

---

## 8. Command Handler — template

```csharp
public sealed record CreateEventCommand(
    string Title, string? Description,
    DateTime StartDate, DateTime EndDate,
    EventType Type, int Capacity) : ICommand<Event>;

public sealed class CreateEventHandler(
    IEventRepository eventRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateEventCommand, Event>
{
    public async Task<Result<Event>> HandleAsync(
        CreateEventCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Validare / creare value objects
        var dateRangeResult = DateRange.Create(command.StartDate, command.EndDate);
        if (dateRangeResult.IsFailure)
            return Result<Event>.Failure(dateRangeResult.Error);

        // 2. Creare entitate
        var eventResult = Event.Create(command.Title, command.Description,
            dateRangeResult.Value, command.Type, command.Capacity);
        if (eventResult.IsFailure)
            return Result<Event>.Failure(eventResult.Error);

        // 3. Persist + SaveChanges
        await eventRepository.AddEventAsync(eventResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Event>.Success(eventResult.Value);
    }
}
```

---

## 9. Repository Pattern

- Interfața în **Domain**, implementarea în **Infrastructure**.
- Repository-ul face Add/Update/Delete **fără** `SaveChanges`.
- `SaveChangesAsync` se apelează în **handler**, prin `IUnitOfWork`.

---

## 10. Unit of Work

`CampusHubDbContext` implementează `IUnitOfWork`. Înregistrat în DI ca:
```csharp
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CampusHubDbContext>());
```

---

## 11. EF Core — IEntityTypeConfiguration

- Fiecare entitate are clasa ei separată de configurare.
- Enum-uri stocate ca string: `.HasConversion<string>()`.
- Value Object-uri mapate ca `ComplexProperty` (EF8).
- Toate configurările aplicate prin `modelBuilder.ApplyConfigurationsFromAssembly(...)`.

---

## 12. Checklist feature nou

1. **Command/Query** — `sealed record` ce implementează `ICommand<T>` sau `IQuery<T>`.
2. **Handler** — `sealed class` cu primary constructor, implementează `ICommandHandler<,>` sau `IQueryHandler<,>`.
3. **Endpoint** — `static class` cu `Map(RouteGroupBuilder)` și `Handle(...)` privat.
4. **Înregistrare rută** — `XEndpoint.Map(group)` în `Program.cs`.
5. Handler-ele se înregistrează **automat** — nu le adăuga manual în DI.
6. `SaveChangesAsync` se apelează **în handler**, nu în repository.

---

## 13. Particularități și capcane

- `Registration.Create(...)` este `internal` — apelabil doar din domain, nu din Feature.
- `Registration.Cancel()` și `Registration.PromoteToConfirmed()` sunt `internal` — același motiv.
- `Participant.Create` validează lungimea **înainte** de `IsNullOrWhiteSpace` — `null` trece prima verificare `.Length` (bug cunoscut).
- `GetEventByIdEndpoint` apelează direct `IEventRepository`, nu CQRS — inconsistență față de restul proiectului.
- `DateRange` este `sealed record` cu constructor privat — EF Core îl mapează ca `ComplexProperty`, nu entitate.
- Enum-urile (`EventType`, `EventStatus`, `RegistrationStatus`) sunt stocate ca **string** în DB.
