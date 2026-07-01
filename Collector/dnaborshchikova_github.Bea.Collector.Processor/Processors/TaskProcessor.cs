using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.Collector.Processor.Processors
{
    public class TaskProcessor : IProcessor
    {
        private readonly ILogger<TaskProcessor> _logger;
        private readonly IEventSender _sender;

        public TaskProcessor(IEventSender sender, ILogger<TaskProcessor> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task<bool> ProcessAsync(List<EventProcessRange> ranges)
        {
            var isSendCompleted = true;
            var tasks = ranges.Select(async range =>
            {
                try
                {
                    await _sender.SendAsync(range);
                }
                catch (Exception ex)
                {
                    isSendCompleted = false;
                    _logger.LogError(ex, $"Информация об ошибке в " +
                    $"Task Id={Task.CurrentId} при обработке RangeId={range.Id}. {ex}");
                }
            });

            await Task.WhenAll(tasks);
            return isSendCompleted;
        }
    }
}
