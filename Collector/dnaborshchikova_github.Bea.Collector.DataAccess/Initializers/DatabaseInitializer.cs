using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.Collector.DataAccess.Initializers
{
    public class DatabaseInitializer
    {
        private readonly CollectorDbContext _context;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(CollectorDbContext context, ILogger<DatabaseInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void CreateDatabase()
        {
            try
            {
                _context.Database.EnsureDeleted();
                _logger.LogInformation("Database deleted.");

                _context.Database.EnsureCreated();
                _logger.LogInformation("Database created with tables.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating database.");
                throw;
            }
        }
    }
}
