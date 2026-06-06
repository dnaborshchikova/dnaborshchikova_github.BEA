using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;
using dnaborshchikova_github.Bea.Collector.Processor.Handlers;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;
using System.Text.Json;

namespace dnaborshchikova_github.Bea.Collector.Processor.Services
{
    public class EventProcessorService : IEventProcessor
    {
        private readonly AppSettings _appSettings;
        private readonly Func<string, IProcessor> _processor;
        private readonly IParser _parser;
        private readonly ILogger<EventProcessorService> _logger;
        private readonly IFileSelectionStrategy _fileSelectionStrategy;
        private readonly ISendEventLogRepository _sendLogRepository;

        public EventProcessorService(Func<string, IProcessor> processor, IParser parser
            , AppSettings appSettings, ILogger<EventProcessorService> logger
            , IFileSelectionStrategy fileSelectionStrategy, ISendEventLogRepository sendLogRepository)
        {
            _parser = parser;
            _processor = processor;
            _appSettings = appSettings;
            _logger = logger;
            _fileSelectionStrategy = fileSelectionStrategy;
            _sendLogRepository = sendLogRepository;
        }

        public async Task ProcessAsync()
        {
            _logger.LogInformation($"Start processing {DateTime.Now}.");
            var filePaths = _fileSelectionStrategy.GetFiles();
            foreach (var filePath in filePaths)
            {
                await ProcessFile(filePath);
            }
        }

        private async Task ProcessFile(string filePath)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                _logger.LogInformation($"Parse file. File path: {filePath}");
                var billEvents = _parser.Parse(filePath).OrderBy(e => e.OperationDateTime).ToList();

                _logger.LogInformation($"Start generate event ranges.");
                var ranges = RangeGenerator.GenerateParts(billEvents, _appSettings.ProcessingSettings.ThreadCount);
                ranges.ForEach(r => _logger.LogInformation($"Generate event range end. Range id: {r.Id}." +
                    $"Events count: {r.BillEvents.Count}."));
                _logger.LogInformation($"End generate event ranges.");

                var processor = _processor(_appSettings.ProcessingSettings.ProcessType);
                var isSendCompleted = await processor.ProcessAsync(ranges);
                await SaveSendResultAsync(isSendCompleted, filePath);
            }
            catch (Exception ex)
            {
                throw new ProcessingException("Error process file.", ex);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation($"End processing {DateTime.Now}." +
                    $"Total processing time: {stopwatch.ElapsedMilliseconds} ms.");
            }
        }

        private async Task SaveSendResultAsync(bool isSendCompleted, string filePath)
        {
            var runSettings = JsonSerializer.Serialize(_appSettings);
            var currentDate = DateTime.Now;

            var utcRunDateTime = DateTime.SpecifyKind(currentDate, DateTimeKind.Utc);
            var fileName = Path.GetFileName(filePath);
            var workerServiceSendLog = new SendEventLog(fileName, utcRunDateTime, runSettings, isSendCompleted);

            await _sendLogRepository.SaveSendResultAsync(workerServiceSendLog);
        }
    }
}
