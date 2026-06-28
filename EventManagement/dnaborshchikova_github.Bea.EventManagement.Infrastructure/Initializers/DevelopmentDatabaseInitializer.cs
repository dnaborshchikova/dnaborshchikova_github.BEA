using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.EventManagement.Infrastructure.Initializers
{
    public class DevelopmentDatabaseInitializer : IDatabaseInitializer
    {
        private readonly EventManagementDbContext _context;
        private readonly ILogger<DevelopmentDatabaseInitializer> _logger;

        public DevelopmentDatabaseInitializer(EventManagementDbContext context
            , ILogger<DevelopmentDatabaseInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Initialize()
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    _context.Database.EnsureDeleted();
                    _logger.LogInformation("Database deleted (dev).");

                    _context.Database.EnsureCreated();
                    _logger.LogInformation("Database created with tables (dev).");
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Retry {i}: DB not ready");
                    Thread.Sleep(3000);
                }
            }

            throw new Exception("Database not available");
        }
    }
}
