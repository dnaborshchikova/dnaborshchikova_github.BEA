using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace dnaborshchikova_github.Bea.Collector.DataAccess
{
    public class CollectorDbContextFactory : IDesignTimeDbContextFactory<CollectorDbContext>
    {
        public CollectorDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<CollectorDbContext>();

            optionsBuilder.UseNpgsql(config.GetConnectionString("Default"));

            return new CollectorDbContext(optionsBuilder.Options);
        }
    }
}