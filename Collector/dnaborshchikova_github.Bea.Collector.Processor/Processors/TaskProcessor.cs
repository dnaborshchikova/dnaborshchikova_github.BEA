using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.Collector.Processor.Processors
{
    public class TaskProcessor : IProcessor
    {
        private readonly ILogger<TaskProcessor> _logger;
        private readonly IEventSender _dbSender;

        public TaskProcessor(IEventSender dbSender, ILogger<TaskProcessor> logger)
        {
            _dbSender = dbSender;
            _logger = logger;
        }

        public async Task<bool> ProcessAsync(List<EventProcessRange> ranges)
        {
            var isSendCompleted = true;
            var tasks = ranges.Select(async range =>
            {
                try
                {
                    await _dbSender.SendAsync(range);
                }
                catch (Exception ex)
                {
                    isSendCompleted = false;
                    _logger.LogError(ex, $"Информация об ошибке в " +
                    $"Task Id={Task.CurrentId} при обработке RangeId={range.Id}");
                }
            });

            await Task.WhenAll(tasks);
            return isSendCompleted;
        }
    }
}
