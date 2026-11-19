# DECISIONS

Answer **yourself**, in your own words, in 3–5 sentences per question.
Refer to concrete files and classes from your own repo (for example `Features/Bookings/CreateBooking/...`).
An unanswered question gives 0 points.

## 1. A rule you moved
Pick one business rule (R1–R8). In which file did it live in the starter and in which file does it live now? Why there?

R7 din BookingService.Create (starter) în Booking.Overlaps și CreateBookingHandler pentru ca regula ține de domeniu, nu de endpoint sau service.

## 2. Result instead of an exception
Why is `Result` a better choice than an exception for "the room is occupied"? What happens in your code when an `Error` is returned?

Camera ocupată e un caz normal de business, nu o eroare neașteptată. Handler-ul întoarce Result<T> cu Error, iar endpoint-ul îl mapează la 409 prin ToProblem.

## 3. Who knows about EF Core?
Which classes in your project know about EF Core and which do not? What do you gain and what do you pay with Repository + Unit of Work?

RoomHubDbContext, configurările și repository-urile din Infrastructure; domeniul și handler-ele nu. Câștig: testabilitate și domeniu curat. Cost: mai multe fișiere și interfețe.

## 4. The flow of a request
Draw in text (ASCII arrows) the path of a `POST /api/rooms/{roomId}/bookings` request through your project, with the **real** names of files and classes, from the endpoint to the database.

_CreateBookingEndpoint → CreateBookingHandler → IRoomRepository (404) → Booking.Create → IBookingRepository (suprapunere, limită zilnică) → IUnitOfWork.SaveChangesAsync → RoomHubDbContext → SQLite.

## 5. What you rejected from what the AI gave you
Give a concrete example of code or an idea proposed by the AI that you corrected or rejected, and say why.

warning-ul/erroarea CS0738

## 6. If you had 30 more minutes
What would you improve and what risk did you leave in the project?

teste automate, validator pe command-uri, concurență la rezervări simultane (două cereri în același timp pot trece de verificarea de suprapunere), index în DB.
