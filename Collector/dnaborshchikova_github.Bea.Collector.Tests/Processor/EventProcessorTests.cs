using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.Processor.Services;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;
using dnaborshchikova_github.Bea.Collector.Tests.Processor.Builders;
using dnaborshchikova_github.Bea.Collector.Tests.Processor.Factories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace dnaborshchikova_github.Bea.Collector.Tests.Processor
{
    public class EventProcessorTests
    {
        #region helpers

        private EventProcessorService CreateSut(Func<string, IProcessor> processor = null
            , IParser parcer = null, AppSettings appSettings = null
            , ILogger<EventProcessorService> logger = null, IFileSelectionStrategy fileSelectionStrategy = null
            , ISendEventLogRepository sendLogRepository = null)
        {
            return new EventProcessorService(processor, parcer, appSettings, logger
                , fileSelectionStrategy, sendLogRepository);
        }
        #endregion

        [Theory]
        [InlineData(1, "Thread")]
        [InlineData(2, "Thread")]
        [InlineData(4, "Thread")]
        [InlineData(1, "Task")]
        [InlineData(2, "Task")]
        [InlineData(4, "Task")]
        public async Task Process_ValidBillEvents_CallsParserAndProcessorOnce(int threadCount, string processType)
        {
            //Arrange
            var generatorSettings = GeneratorSettingsBuilder.Default()
                .Build();

            var processingSettings = ProcessingSettingsBuilder.Default()
                .WithThreadCount(threadCount)
                .WithProcessType(processType)
                .Build();

            var appSettings = new AppSettingsBuilder()
                .WithGeneratorSettings(generatorSettings)
                .WithProcessingSettings(processingSettings)
                .Build();

            var logger = NullLogger<EventProcessorService>.Instance;
            var parserMock = new Mock<IParser>();
            parserMock.Setup(p => p.Parse(It.IsAny<string>()))
                .Returns(new List<BillEvent>
                {
                    BillEventFactory.CreatePaidBillEvent(new DateTime(2024, 1, 2), Guid.NewGuid(), 100m, "INV-001"),
                    BillEventFactory.CreateCancelledBillEvent(new DateTime(2024, 1, 1), Guid.NewGuid(), 50m, "INV-002")
                });

            var processorMock = new Mock<IProcessor>();
            var processorFactoryMock = new Mock<Func<string, IProcessor>>();
            processorFactoryMock.Setup(f => f(processType)).Returns(processorMock.Object);

            var fileSelectionStrategyMock = new Mock<IFileSelectionStrategy>();
            fileSelectionStrategyMock
                .Setup(f => f.GetFiles())
                .Returns(new List<string> { appSettings.ProcessingSettings.FilePath });

            var sendLogRepositoryMock = new Mock<ISendEventLogRepository>();
            sendLogRepositoryMock
                .Setup(r => r.SaveSendResultAsync(It.IsAny<SendEventLog>()))
                .Returns(Task.CompletedTask);

            var service = CreateSut(processorFactoryMock.Object, parserMock.Object
                , appSettings, logger, fileSelectionStrategyMock.Object, sendLogRepositoryMock.Object);

            //Act
            await service.ProcessAsync();

            //Assert
            fileSelectionStrategyMock.Verify(f => f.GetFiles(), Times.Once);
            parserMock.Verify(p => p.Parse(appSettings.ProcessingSettings.FilePath), Times.Once);
            processorFactoryMock.Verify(f => f(appSettings.ProcessingSettings.ProcessType), Times.Once);
            processorMock.Verify(p => p.ProcessAsync(It.IsAny<List<EventProcessRange>>()), Times.Once);
        }
    }
}
