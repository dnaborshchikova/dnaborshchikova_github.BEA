using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace dnaborshchikova_github.Bea.EventManagement.Infrastructure
{
    public class EventManagementDbContext : DbContext
    {
        public EventManagementDbContext(DbContextOptions<EventManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<CashRegisterEvent> Events { get; set; }
    }
}
