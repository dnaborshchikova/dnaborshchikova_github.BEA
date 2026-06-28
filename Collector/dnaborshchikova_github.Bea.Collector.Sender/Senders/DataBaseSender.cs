using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using EFCore.BulkExtensions;
using dnaborshchikova_github.Bea.Collector.DataAccess;

namespace dnaborshchikova_github.Bea.Collector.Sender.Handlers
{
    public class DataBaseSender : IEventSender
    {
        private readonly ILogger<DataBaseSender> _logger;
        private readonly IDbContextFactory<CollectorDbContext> _contextFactory;

        public DataBaseSender(ILogger<DataBaseSender> logger
            , IDbContextFactory<CollectorDbContext> contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public void Send(EventProcessRange range)
        {
            _logger.LogInformation($"Start save events. Range id: {range.Id}. Event count: {range.BillEvents.Count}. " +
                $"Thread id: {Thread.CurrentThread.ManagedThreadId}.");
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var batchSize = 5000;
            var count = 0;
            using var dbContext = _contextFactory.CreateDbContext();
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            try
            {
                foreach (var billEvent in range.BillEvents)
                {
                    var billData = billEvent switch
                    {
                        PaidBillEvent paid => JsonSerializer.Serialize(paid),
                        CancelledBillEvent cancelled => JsonSerializer.Serialize(cancelled)
                    };
                    var utcTime = DateTime.SpecifyKind(billEvent.OperationDateTime, DateTimeKind.Utc);

                    var sendEvent = new SendEvent(billEvent.Id, utcTime,
                        billEvent.UserId, billEvent.EventType, billData);

                    dbContext.SendEvents.Add(sendEvent);
                    count++;

                    if (count % batchSize == 0)
                    {
                        dbContext.SaveChanges();
                        dbContext.ChangeTracker.Clear();
                    }
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }

            stopwatch.Stop();
            _logger.LogInformation($"End save events. Range id: {range.Id}. Event count: {range.BillEvents.Count}. "
                + $"Thread id: {Thread.CurrentThread.ManagedThreadId}. "
                + $"Work time: {stopwatch.ElapsedMilliseconds} ms.");
        }

        public async Task SendAsync(EventProcessRange range)
        {
            _logger.LogInformation($"Start save events. Range id: {range.Id}. Event count: {range.BillEvents.Count}. " +
               $"Thread id: {Thread.CurrentThread.ManagedThreadId}.");
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using var dbContext = _contextFactory.CreateDbContext();
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            var sendEvents = range.BillEvents.Select(e =>
            {
                var billData = e switch
                {
                    PaidBillEvent paid => JsonSerializer.Serialize(paid),
                    CancelledBillEvent cancelled => JsonSerializer.Serialize(cancelled)
                };
                var utcTime = DateTime.SpecifyKind(e.OperationDateTime, DateTimeKind.Utc);

                return new SendEvent(e.Id, utcTime, e.UserId, e.EventType, billData);
            });
            await dbContext.BulkInsertOrUpdateAsync(sendEvents, new BulkConfig
            {
                BatchSize = 50000, 
                PreserveInsertOrder = false,
                SetOutputIdentity = false,
                BulkCopyTimeout = 0,
                UpdateByProperties = new List<string> { "Id" },
                PropertiesToIncludeOnUpdate = new List<string>()
            });

            stopwatch.Stop();
            _logger.LogInformation($"End save events. Range id: {range.Id}. Event count: {range.BillEvents.Count}. "
                + $"Thread id: {Thread.CurrentThread.ManagedThreadId}. "
                + $"Work time: {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}
