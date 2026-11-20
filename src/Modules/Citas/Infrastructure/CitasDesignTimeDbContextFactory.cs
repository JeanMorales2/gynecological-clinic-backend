using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ginecologia.Modules.Citas.Infrastructure;

public sealed class CitasDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CitasDbContext>
{
    public CitasDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CitasDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=ginecologia;Username=ginecologia;Password=local-development-only")
            .Options;
        return new CitasDbContext(options);
    }
}
