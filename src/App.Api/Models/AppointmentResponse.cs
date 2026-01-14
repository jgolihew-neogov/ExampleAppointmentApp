namespace App.Api.Models;

public class AppointmentResponse
{
    public Guid Id { get; set; }
    public Guid ProviderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; }
}
