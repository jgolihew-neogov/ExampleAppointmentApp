namespace App.Api.Models;

public class BookAppointmentRequest
{
    public Guid ProviderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
}
