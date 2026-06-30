using dnaborshchikova_github.Bea.EventManagement.Core.Interfaces;
using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using dnaborshchikova_github.Bea.EventManagement.Core.Models.Exceptions;
using dnaborshchikova_github.Bea.EventManagement.Infrastructure.ExceptionExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.EventManagement.Infrastructure
{
    public class EventRepository : IEventRepository
    {
        private readonly EventManagementDbContext _dbContext;
        private readonly ILogger<EventRepository> _logger;

        public EventRepository(EventManagementDbContext dbContext, ILogger<EventRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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

        public async Task SaveBatchAsync(List<CashRegisterEvent> events)
        {
            //if (events == null || events.Count == 0)
            //    return;

            //var sql = new StringBuilder();
            //sql.Append(@"INSERT INTO ""Events"" (""Id"", ""Date"", ""UserId"", ""EventType"", ""Data"") VALUES ");

            //var parameters = new List<NpgsqlParameter>();
            //for (int i = 0; i < events.Count; i++)
            //{
            //    var e = events[i];

            //    sql.Append($"(@id{i}, @date{i}, @userId{i}, @type{i}, @data{i})");

            //    if (i < events.Count - 1)
            //        sql.Append(",");

            //    parameters.Add(new NpgsqlParameter($"id{i}", e.Id));
            //    parameters.Add(new NpgsqlParameter($"date{i}", e.Date));
            //    parameters.Add(new NpgsqlParameter($"userId{i}", e.UserId));
            //    parameters.Add(new NpgsqlParameter($"type{i}", e.EventType));
            //    parameters.Add(new NpgsqlParameter($"data{i}", (object?)e.Data ?? DBNull.Value));
            //}

            //sql.Append(@" ON CONFLICT (""Id"") DO NOTHING;");

            //await _dbContext.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());

            try
            {
                _dbContext.Events.AddRange(events);
                foreach (var entry in _dbContext.ChangeTracker.Entries<CashRegisterEvent>())
                {
                    _logger.LogInformation(
                        "Id={Id}, State={State}",
                        entry.Entity.Id,
                        entry.State);
                }

                var count = await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Saved {Count}", count);

                var total = await _dbContext.Events.CountAsync();
                _logger.LogInformation("Total in DB: {Count}", total);
            }
            catch (DbUpdateException ex) when (ex.IsDuplicateKey())
            {
                // тут батч уже частично записан
                // просто считаем это "best effort"
            }
        }
    }
}
