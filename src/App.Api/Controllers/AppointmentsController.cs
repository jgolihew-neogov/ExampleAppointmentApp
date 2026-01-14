using App.Api.Models;
using App.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentService _appointmentService;

    public AppointmentsController(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequest request)
    {
        var result = await _appointmentService.BookAppointmentAsync(
            request.ProviderId,
            request.CustomerName,
            request.StartUtc,
            request.EndUtc);

        if (!result.IsSuccess)
        {
            var statusCode = result.Type switch
            {
                ResultType.NotFound => StatusCodes.Status404NotFound,
                ResultType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            };

            return Problem(
                statusCode: statusCode,
                title: result.Type.ToString(),
                detail: result.Error);
        }

        var response = new AppointmentResponse
        {
            Id = result.Value!.Id,
            ProviderId = result.Value.ProviderId,
            CustomerName = result.Value.CustomerName,
            StartUtc = result.Value.StartUtc,
            EndUtc = result.Value.EndUtc,
            Status = result.Value.Status.ToString(),
            CreatedUtc = result.Value.CreatedUtc
        };

        return CreatedAtAction(nameof(BookAppointment), new { id = response.Id }, response);
    }

    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelAppointment(Guid id)
    {
        var result = await _appointmentService.CancelAppointmentAsync(id);

        if (!result.IsSuccess)
        {
            var statusCode = result.Type switch
            {
                ResultType.NotFound => StatusCodes.Status404NotFound,
                ResultType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            };

            return Problem(
                statusCode: statusCode,
                title: result.Type.ToString(),
                detail: result.Error);
        }

        return NoContent();
    }
}
