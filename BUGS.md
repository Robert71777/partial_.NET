# BUGS

The starter contains behaviors that contradict `ASSIGNMENT.md`. Write here every discrepancy you found.
Fill it in yourself, in your own words. There is no required number: what counts is that they are real.

| Which rule from ASSIGNMENT.md is violated (R1–R8) | Where the problem was in the starter (file, approximate line) | How you discovered it (test, reading, AI) | What you changed |
|---|---|---|---|
| _(fill in)_ | _(fill in)_ | _(fill in)_ | _(fill in)_ |

Bug-urile reale din starter:
R7: BookingService.Create folosea <=, deci intervalele care se ating dădeau 409
R7/R8: rezervările anulate nu erau excluse din verificarea de suprapunere
R8: anularea unei rezervări deja anulate dădea 204, nu 409
R5 și R6: lipseau complet
persistența: repository-uri în memorie, singleton (datele se pierdeau la restart)