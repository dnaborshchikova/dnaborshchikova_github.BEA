using dnaborshchikova_github.Bea.EventManagement.Core.Interfaces;
using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using dnaborshchikova_github.Bea.EventManagement.Core.Models.Exceptions;
using dnaborshchikova_github.Bea.EventManagement.Infrastructure.ExceptionExtensions;
using Microsoft.EntityFrameworkCore;

namespace dnaborshchikova_github.Bea.EventManagement.Infrastructure
{
    public class EventRepository : IEventRepository
    {
        private readonly EventManagementDbContext _dbContext;

        public EventRepository(EventManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsEventExists(CashRegisterEvent сashRegisterEvent)
        {
            return _dbContext.Events.Any(e => e.Id == сashRegisterEvent.Id);
        }

        public async Task SaveAsync(CashRegisterEvent сashRegisterEvent)
        {
            try
            {
                _dbContext.Events.Add(сashRegisterEvent);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.IsDuplicateKey())
            {
                throw new DuplicateEventException(сashRegisterEvent.Id);
            }
        }
    }
}
