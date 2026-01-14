namespace App.Api.Models;

public class CreateProviderRequest
{
    public string Name { get; set; } = string.Empty;
    public string? TimeZone { get; set; }
}
