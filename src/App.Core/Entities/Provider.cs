namespace App.Core.Entities;

public class Provider
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TimeZone { get; set; } = "UTC";
    public bool IsActive { get; set; } = true;

    public List<Appointment> Appointments { get; set; } = new();
}
