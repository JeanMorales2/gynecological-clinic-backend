using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ginecologia.Modules.Prospeccion.Infrastructure;

public sealed class ProspeccionDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProspeccionDbContext>
{
    public ProspeccionDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ProspeccionDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=ginecologia;Username=ginecologia;Password=local-development-only")
            .Options;
        return new ProspeccionDbContext(options);
    }
}
