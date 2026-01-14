using App.Core.Entities;
using App.Core.Interfaces;
using App.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IClock _clock;
    private readonly IWebHostEnvironment _environment;

    public SeedController(AppDbContext context, IClock clock, IWebHostEnvironment environment)
    {
        _context = context;
        _clock = clock;
        _environment = environment;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SeedData()
    {
        if (!_environment.IsDevelopment())
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Not Allowed",
                detail: "Seed endpoint is only available in Development environment");
        }

        var hasData = await _context.Providers.AnyAsync();
        if (hasData)
        {
            return Ok(new { message = "Database already has data. Skipping seed." });
        }

        var provider1 = new Provider
        {
            Id = Guid.NewGuid(),
            Name = "Dr. Sarah Johnson",
            TimeZone = "UTC",
            IsActive = true
        };

        var provider2 = new Provider
        {
            Id = Guid.NewGuid(),
            Name = "Dr. Michael Chen",
            TimeZone = "UTC",
            IsActive = true
        };

        _context.Providers.AddRange(provider1, provider2);

        var tomorrow = _clock.UtcNow.Date.AddDays(1).AddHours(9);

        var appointment1 = new Appointment
        {
            Id = Guid.NewGuid(),
            ProviderId = provider1.Id,
            CustomerName = "John Smith",
            StartUtc = tomorrow,
            EndUtc = tomorrow.AddMinutes(30),
            Status = AppointmentStatus.Booked,
            CreatedUtc = _clock.UtcNow
        };

        var appointment2 = new Appointment
        {
            Id = Guid.NewGuid(),
            ProviderId = provider1.Id,
            CustomerName = "Jane Doe",
            StartUtc = tomorrow.AddHours(1),
            EndUtc = tomorrow.AddHours(1).AddMinutes(45),
            Status = AppointmentStatus.Booked,
            CreatedUtc = _clock.UtcNow
        };

        _context.Appointments.AddRange(appointment1, appointment2);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Sample data seeded successfully", providers = 2, appointments = 2 });
    }
}
