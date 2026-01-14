using App.Core.Entities;
using App.Core.Interfaces;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        return await _context.Appointments
            .Include(a => a.Provider)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Appointment>> GetProviderScheduleAsync(Guid providerId, DateTime dateUtc)
    {
        var startOfDay = dateUtc.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.Appointments
            .Include(a => a.Provider)
            .Where(a => a.ProviderId == providerId
                && a.StartUtc >= startOfDay
                && a.StartUtc < endOfDay)
            .OrderBy(a => a.StartUtc)
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingAppointmentAsync(
        Guid providerId,
        DateTime startUtc,
        DateTime endUtc,
        Guid? excludeAppointmentId = null)
    {
        var query = _context.Appointments
            .Where(a => a.ProviderId == providerId
                && a.Status == AppointmentStatus.Booked
                && a.StartUtc < endUtc
                && a.EndUtc > startUtc);

        if (excludeAppointmentId.HasValue)
        {
            query = query.Where(a => a.Id != excludeAppointmentId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
