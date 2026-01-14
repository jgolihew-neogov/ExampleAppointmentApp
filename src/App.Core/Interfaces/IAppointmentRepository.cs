using App.Core.Entities;

namespace App.Core.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<List<Appointment>> GetProviderScheduleAsync(Guid providerId, DateTime dateUtc);
    Task<bool> HasOverlappingAppointmentAsync(Guid providerId, DateTime startUtc, DateTime endUtc, Guid? excludeAppointmentId = null);
    Task AddAsync(Appointment appointment);
    Task SaveChangesAsync();
}
