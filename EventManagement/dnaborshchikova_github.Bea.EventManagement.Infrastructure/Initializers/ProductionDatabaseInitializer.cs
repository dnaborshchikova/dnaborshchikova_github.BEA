using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.EventManagement.Infrastructure.Initializers
{
    public class ProductionDatabaseInitializer : IDatabaseInitializer
    {
        private readonly EventManagementDbContext _context;
        private readonly ILogger<ProductionDatabaseInitializer> _logger;

        public ProductionDatabaseInitializer(EventManagementDbContext context
            , ILogger<ProductionDatabaseInitializer> logger)
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
                    _context.Database.Migrate();
                    _logger.LogInformation("Database migrated (prod).");
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
