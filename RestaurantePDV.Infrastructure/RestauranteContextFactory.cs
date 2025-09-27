using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RestaurantePDV.Infrastructure.Data;
using System.IO;

namespace RestaurantePDV.Infrastructure;

public class RestauranteContextFactory : IDesignTimeDbContextFactory<RestauranteContext>
{
    public RestauranteContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<RestauranteContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                              "Host=localhost;Database=RestaurantePDV;Username=postgres;Password=password";
        
        optionsBuilder.UseNpgsql(connectionString);

        return new RestauranteContext(optionsBuilder.Options);
    }
}