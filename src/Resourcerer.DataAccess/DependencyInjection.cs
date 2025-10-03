using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;
using System.Diagnostics;

namespace Resourcerer.DataAccess;

public static class DependencyInjection
{
    public static void Register(IServiceCollection services, IWebHostEnvironment env)
    {
        var dbPath = Path.Combine(env.WebRootPath, "database.db3");
        var prefix = "Datasource=";

        services.AddDbContext<AppDbContext>(cfg =>
            cfg.UseSqlite($"{prefix}{dbPath}"));

        // pass identity data from jwt to DBContext
        services.AddTransient(x =>
        {
            var service = x.GetRequiredService<IAppIdentityService<AppIdentity>>();
            if (service == null)
                throw new InvalidOperationException($"{typeof(IAppIdentityService<AppIdentity>)} not found");

            return service.Identity;
        });
    }
}
