using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.Collector.Processor.Processors
{
    public class ThreadProcessorWithLock : IProcessor
    {
        private readonly ILogger<ThreadProcessor> _logger;
        private readonly IEventSender _eventSender;
        private readonly object locker = new object();
        private int completedThreads;

        public ThreadProcessorWithLock(ILogger<ThreadProcessor> logger, IEventSender eventSender)
        {
            _logger = logger;
            _eventSender = eventSender;
        }

        public async Task<bool> ProcessAsync(List<EventProcessRange> ranges)
        {
            completedThreads = ranges.Count;
            var exceptions = new List<Exception>();
            var isSendCompleted = true;
            foreach (var range in ranges)
            {
                var thread = new Thread(() => ProcessRange(range, exceptions));
                thread.Start();
            }

            lock (locker)
            {
                while (completedThreads > 0)
                {
                    Monitor.Wait(locker);
                }
            }

            if (exceptions.Count > 0)
            {
                _logger.LogInformation($"При обработке данных возникли ошибки:\n" +
                    $"{string.Join(";\n", exceptions)}");
                isSendCompleted = false;
            }

            return isSendCompleted;
        }

        private void ProcessRange(EventProcessRange range, List<Exception> exceptions)
        {
            try
            {
                _eventSender.Send(range);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"Ошибка в потоке обработки диапазона." +
                    $"Thread id: {Thread.CurrentThread.ManagedThreadId}. Range id {range.Id}");

                lock (locker)
                {
                    exceptions.Add(ex);
                }
            }
            finally
            {
                lock (locker)
                {
                    completedThreads--;
                    Monitor.Pulse(locker);
                }
            }
        }
    }
}
