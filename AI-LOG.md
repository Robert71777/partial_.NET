# AI-LOG

Write down, in order, what you asked the AI. It is not a punishment: a missing or false log lowers the level D grade.

| Time (approx.) | What you asked (briefly) | What you kept | What you changed or rejected |
|---|---|---|---|
| _(fill in)_ | _(fill in)_ | _(fill in)_ | _(fill in)_ |


Am cerut să citească ASSIGNMENT.md și steering-ul și să rezume regulile încălcate. Am păstrat rezumatul (R5, R6, R7, R8 încălcate, fără Result, fără EF). N-am schimbat nimic.   9:03
Am cerut Result, Error, interfețele CQRS, AddHandlers, ToProblem. Am păstrat tot. Terminalul lui Kiro dubla caracterele, așa că build-ul l-am rulat eu. 9:07
Am cerut domeniul rich (Room, Booking, erori, interfețe). Am păstrat. Am verificat singur build-ul. 9:13
Am cerut cele 5 slice-uri. Kiro a adăugat singur repository-uri în memorie și NoOpUnitOfWork ca să compileze, apoi le-a înlocuit la pasul următor. 9:18
Am cerut EF Core + SQLite. Am păstrat configurările, repository-urile și EnsureCreated. 9:25
Cerința nouă (maxim 2 rezervări active pe zi per bookedBy). Am păstrat MaxActivePerDay, EnsureDailyLimit și CountActiveForBookerOnDayAsync. 9:30
Am cerut repararea warning-ului CS0114. Prima propunere (override, apoi ștergerea metodei) a dat eroarea CS0738, deoarece IUnitOfWork întoarce Task, nu Task<int>. 9:32
Am cerut repararea CS0738 cu implementare explicită async Task IUnitOfWork.SaveChangesAsync. 9:34
R10 (limita de 2h la capacitate > 10). Am păstrat eroarea nouă DurationExceedsRoomLimit și pe cea veche pentru 4h. 9:37
R11 (anulare doar cu 60 min înainte). Am păstrat Cancel(DateTime nowUtc) și CancellationTooLate. 9:41