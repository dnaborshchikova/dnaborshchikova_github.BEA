using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace dnaborshchikova_github.Bea.Collector.DataAccess
{
    public class CollectorDbContext : DbContext
    {
        public CollectorDbContext(DbContextOptions<CollectorDbContext> options) 
            : base(options)
        {
        }

        public DbSet<SendEvent> SendEvents { get; set; }
        public DbSet<SendEventLog> SendEventLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CollectorDbContext).Assembly);
        }
    }
}
