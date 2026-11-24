using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ginecologia.Modules.HistoriaClinica.Infrastructure;

public sealed class HistoriaClinicaDesignTimeDbContextFactory : IDesignTimeDbContextFactory<HistoriaClinicaDbContext>
{
    public HistoriaClinicaDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<HistoriaClinicaDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=ginecologia;Username=ginecologia;Password=local-development-only", npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "historia_clinica"))
            .Options;
        return new HistoriaClinicaDbContext(options);
    }
}
