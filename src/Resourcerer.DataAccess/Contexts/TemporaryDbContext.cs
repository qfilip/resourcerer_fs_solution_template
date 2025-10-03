using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace Resourcerer.DataAccess.Contexts;
public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    // https://medium.com/@speedforcerun/implementing-idesigntimedbcontextfactory-in-asp-net-core-2-0-2-1-3718bba6db84
    private readonly IConfiguration _configuration;
    private readonly string? _connectionString;

    public TemporaryDbContextFactory()
    {
        var projectDirectory = Directory.GetCurrentDirectory();
        var wwwroot = Path.Combine(projectDirectory, "wwwroot");
        if(!Directory.Exists(wwwroot))
            Directory.CreateDirectory(wwwroot);

        var dbPath = Path.Combine(wwwroot, "database.db3");
        var prefix = "Datasource=";
        _connectionString = prefix + dbPath;
    }
    public TemporaryDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseSqlite(_connectionString,
        optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(AppDbContext).GetTypeInfo().Assembly.GetName().Name));
        return new AppDbContext(builder.Options, new Identity.Models.AppIdentity(Guid.Empty, "system", "email"));
    }
}
