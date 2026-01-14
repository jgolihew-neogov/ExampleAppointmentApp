namespace App.Core.Interfaces;

public interface IClock
{
    DateTime UtcNow { get; }
}
