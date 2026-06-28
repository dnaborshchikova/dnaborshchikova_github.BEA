using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using dnaborshchikova_github.Bea.EventManagement.Core.Interfaces;

namespace dnaborshchikova_github.Bea.EventManagement.Infrastructure
{
    public class EventRepository : IEventRepository
    {
        private readonly EventManagementDbContext _dbContext;

        public EventRepository(EventManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveAsync(CashRegisterEvent сashRegisterEvent)
        {
            await _dbContext.Events.AddAsync(сashRegisterEvent);
            await _dbContext.SaveChangesAsync();
        }
    }
}
