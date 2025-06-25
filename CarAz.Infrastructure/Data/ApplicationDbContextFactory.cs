using CarAz.Core.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CarAz.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Try both possible locations for appsettings.json
        string[] possiblePaths =
        {
            Path.Combine(Directory.GetCurrentDirectory(), "../CarAz.Presentation/appsettings.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "CarAz.Presentation/appsettings.json")
        };
        string? configPath = possiblePaths.FirstOrDefault(File.Exists);
        if (configPath == null)
            throw new FileNotFoundException("Could not find appsettings.json for design-time DbContext creation.");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(configPath)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
} 