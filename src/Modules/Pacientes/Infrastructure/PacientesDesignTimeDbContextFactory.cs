using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ginecologia.Modules.Pacientes.Infrastructure;

public sealed class PacientesDesignTimeDbContextFactory : IDesignTimeDbContextFactory<PacientesDbContext>
{
    public PacientesDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PacientesDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=ginecologia;Username=ginecologia;Password=local-development-only")
            .Options;
        return new PacientesDbContext(options);
    }
}
