using Ginecologia.Modules.Citas.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ginecologia.Modules.Citas.Infrastructure;

public static class CitasServiceCollectionExtensions
{
    public static IServiceCollection AddCitasModule(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CitasDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IScheduleExceptionRepository, ScheduleExceptionRepository>();
        services.AddScoped<AppointmentParticipantsValidator>();
        services.AddScoped<BusinessHoursValidator>();
        services.AddScoped<AppointmentDtoMapper>();
        services.AddScoped<CreateAppointmentUseCase>();
        services.AddScoped<UpdateAppointmentUseCase>();
        services.AddScoped<ChangeAppointmentStatusUseCase>();
        services.AddScoped<GetAppointmentUseCase>();
        services.AddScoped<ListAppointmentsUseCase>();
        services.AddScoped<CreateScheduleExceptionUseCase>();
        services.AddScoped<DeleteScheduleExceptionUseCase>();
        services.AddScoped<ListScheduleExceptionsUseCase>();
        return services;
    }
}
