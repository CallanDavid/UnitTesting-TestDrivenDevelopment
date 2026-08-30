# Flight Booking - TDD

<sub>Documentation drafted with claude.ai</sub>

A flight booking domain written test-first with xUnit. The point of the project was
the process, not the feature set: every rule here started as a failing test.

## Solution layout

- `Domain/` - `Flight`, `Booking`, and the error types (`OverbookingError`, `BookingNotFoundError`)
- `Domain.Tests/` - unit tests for the booking rules
- `Application/` - `BookingService` and its DTOs
- `Application.Tests/` - specification-style tests against the service
- `Data/` - EF Core entities

## Running the tests

    dotnet test

Targets .NET 9.
