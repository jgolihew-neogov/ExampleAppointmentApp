namespace App.Core.Entities;

public class Appointment
{
    public Guid Id { get; set; }
    public Guid ProviderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public AppointmentStatus Status { get; set; }
    public DateTime CreatedUtc { get; set; }

    public Provider Provider { get; set; } = null!;
}

public enum AppointmentStatus
{
    Booked,
    Cancelled
}
