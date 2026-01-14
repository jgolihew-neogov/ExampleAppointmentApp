using App.Core.Entities;
using App.Core.Interfaces;

namespace App.Core.Services;

public class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly IClock _clock;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IProviderRepository providerRepository,
        IClock clock)
    {
        _appointmentRepository = appointmentRepository;
        _providerRepository = providerRepository;
        _clock = clock;
    }

    public async Task<Result<Appointment>> BookAppointmentAsync(
        Guid providerId,
        string customerName,
        DateTime startUtc,
        DateTime endUtc)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            return Result<Appointment>.Failure("Customer name is required");

        if (endUtc <= startUtc)
            return Result<Appointment>.Failure("End time must be after start time");

        var durationMinutes = (endUtc - startUtc).TotalMinutes;
        if (durationMinutes < 15 || durationMinutes > 120)
            return Result<Appointment>.Failure("Appointment duration must be between 15 and 120 minutes");

        var provider = await _providerRepository.GetByIdAsync(providerId);
        if (provider == null)
            return Result<Appointment>.Failure("Provider not found", ResultType.NotFound);

        if (!provider.IsActive)
            return Result<Appointment>.Failure("Provider is not active");

        var minimumStartTime = _clock.UtcNow.AddMinutes(30);
        if (startUtc < minimumStartTime)
            return Result<Appointment>.Failure("Appointment must be at least 30 minutes in the future");

        var hasOverlap = await _appointmentRepository.HasOverlappingAppointmentAsync(
            providerId, startUtc, endUtc);
        if (hasOverlap)
            return Result<Appointment>.Failure("Provider has an overlapping appointment", ResultType.Conflict);

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            ProviderId = providerId,
            CustomerName = customerName,
            StartUtc = startUtc,
            EndUtc = endUtc,
            Status = AppointmentStatus.Booked,
            CreatedUtc = _clock.UtcNow
        };

        await _appointmentRepository.AddAsync(appointment);
        await _appointmentRepository.SaveChangesAsync();

        return Result<Appointment>.Success(appointment);
    }

    public async Task<Result> CancelAppointmentAsync(Guid appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment == null)
            return Result.Failure("Appointment not found", ResultType.NotFound);

        if (appointment.Status == AppointmentStatus.Cancelled)
            return Result.Failure("Appointment is already cancelled", ResultType.Conflict);

        appointment.Status = AppointmentStatus.Cancelled;
        await _appointmentRepository.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<List<Appointment>> GetProviderScheduleAsync(Guid providerId, DateTime date)
    {
        var startOfDay = date.Date;
        return await _appointmentRepository.GetProviderScheduleAsync(providerId, startOfDay);
    }
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public ResultType Type { get; }

    protected Result(bool isSuccess, string? error, ResultType type)
    {
        IsSuccess = isSuccess;
        Error = error;
        Type = type;
    }

    public static Result Success() => new(true, null, ResultType.Success);
    public static Result Failure(string error, ResultType type = ResultType.BadRequest) => new(false, error, type);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string? error, ResultType type)
        : base(isSuccess, error, type)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null, ResultType.Success);
    public static new Result<T> Failure(string error, ResultType type = ResultType.BadRequest) => new(false, default, error, type);
}

public enum ResultType
{
    Success,
    BadRequest,
    NotFound,
    Conflict
}
