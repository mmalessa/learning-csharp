using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pizzeria.Infrastructure;

public sealed class DesignTimeFactory : IDesignTimeDbContextFactory<PizzeriaDbContext>
{
    public PizzeriaDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("PIZZERIA_DB")
                   ?? "Server=localhost,1433;Database=Pizzeria;User Id=sa;Password=MsSql12345;TrustServerCertificate=True";
        var opts = new DbContextOptionsBuilder<PizzeriaDbContext>()
            .UseSqlServer(conn)
            .Options;
        return new PizzeriaDbContext(opts);
    }
}