using App.Core.Interfaces;

namespace App.Infrastructure.Services;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
