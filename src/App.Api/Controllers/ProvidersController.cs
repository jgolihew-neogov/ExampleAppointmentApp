using App.Api.Models;
using App.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvidersController : ControllerBase
{
    private readonly ProviderService _providerService;
    private readonly AppointmentService _appointmentService;

    public ProvidersController(ProviderService providerService, AppointmentService appointmentService)
    {
        _providerService = providerService;
        _appointmentService = appointmentService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProviderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProvider([FromBody] CreateProviderRequest request)
    {
        var result = await _providerService.CreateProviderAsync(request.Name, request.TimeZone);

        if (!result.IsSuccess)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation Error",
                detail: result.Error);
        }

        var response = new ProviderResponse
        {
            Id = result.Value!.Id,
            Name = result.Value.Name,
            TimeZone = result.Value.TimeZone,
            IsActive = result.Value.IsActive
        };

        return CreatedAtAction(nameof(GetProviders), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ProviderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProviders()
    {
        var providers = await _providerService.GetAllProvidersAsync();

        var response = providers.Select(p => new ProviderResponse
        {
            Id = p.Id,
            Name = p.Name,
            TimeZone = p.TimeZone,
            IsActive = p.IsActive
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{providerId}/schedule")]
    [ProducesResponseType(typeof(List<AppointmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProviderSchedule(Guid providerId, [FromQuery] string date)
    {
        if (!DateTime.TryParse(date, out var parsedDate))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation Error",
                detail: "Invalid date format. Use YYYY-MM-DD");
        }

        var appointments = await _appointmentService.GetProviderScheduleAsync(providerId, parsedDate);

        var response = appointments.Select(a => new AppointmentResponse
        {
            Id = a.Id,
            ProviderId = a.ProviderId,
            CustomerName = a.CustomerName,
            StartUtc = a.StartUtc,
            EndUtc = a.EndUtc,
            Status = a.Status.ToString(),
            CreatedUtc = a.CreatedUtc
        }).ToList();

        return Ok(response);
    }
}
