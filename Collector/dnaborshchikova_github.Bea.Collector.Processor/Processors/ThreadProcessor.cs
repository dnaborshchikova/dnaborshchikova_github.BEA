using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace dnaborshchikova_github.Bea.Collector.Processor.Processors
{
    public class ThreadProcessor : IProcessor
    {
        private readonly IEventSender _dbSender;
        private readonly ILogger<ThreadProcessor> _logger;

        public ThreadProcessor(ILogger<ThreadProcessor> logger, IEventSender dbSender)
        {
            _dbSender = dbSender;
            _logger = logger;
        }

        public async Task<bool> ProcessAsync(List<EventProcessRange> ranges)
        {
            using var countdown = new CountdownEvent(ranges.Count);
            var exceptions = new ConcurrentQueue<(int rangeId, int threadId, Exception ex)>();
            var isSendCompleted = true;
            foreach (var range in ranges)
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        _dbSender.Send(range);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error save events. Range id: {range.Id}. " +
                            $"Thread id: {Thread.CurrentThread.ManagedThreadId}. Error: {ex.Message}");
                        exceptions.Enqueue((range.Id, Thread.CurrentThread.ManagedThreadId, ex));
                    }
                    finally
                    {
                        countdown.Signal();
                    }
                });

                thread.Start();
            }

            countdown.Wait();

            if (!exceptions.IsEmpty)
            {
                foreach (var (rangeId, threadId, ex) in exceptions)
                {
                    _logger.LogError(ex, $"Подробная информация об ошибке в " +
                        $"ThreadId={Thread.CurrentThread.ManagedThreadId} при обработке RangeId={rangeId}");
                }
                isSendCompleted = false;
            }

            return isSendCompleted;
        }
    }
}
