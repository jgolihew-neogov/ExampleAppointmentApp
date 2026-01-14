namespace App.Api.Models;

public class ProviderResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
