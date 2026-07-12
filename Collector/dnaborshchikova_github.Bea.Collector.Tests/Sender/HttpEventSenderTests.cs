using dnaborshchikova_github.Bea.Collector.Senders;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Sender;

namespace dnaborshchikova_github.Bea.Collector.Tests.Sender
{
    public class ApiSenderTests
    {
        [Fact]
        public async Task SendAsync_ShouldSendEventsBatch()
        {
            // Arrange.
            var logger = NullLogger<ApiSender>.Instance;
            var client = new Mock<IEventsClient>();
            client
                .Setup(x => x.BatchAsync(It.IsAny<IEnumerable<CashRegisterEventDto>>()))
                .Returns(Task.CompletedTask);
            var apiSender = new ApiSender(client.Object, logger);

            var eventRange = GetEventProcessRange();

            // Act.
            await apiSender.SendAsync(eventRange);

            // Assert.
            client.Verify(x 
                => x.BatchAsync(It.Is<IEnumerable<CashRegisterEventDto>>(events => events.Count() == 1)), Times.Once);
        }

        [Fact]
        public async Task SendAsync_ShouldThrow_WhenApiFails()
        {
            // Arrange.
            var logger = NullLogger<ApiSender>.Instance;
            var client = new Mock<IEventsClient>();
            client
                .Setup(x => x.BatchAsync(It.IsAny<IEnumerable<CashRegisterEventDto>>()))
                .ThrowsAsync(new ApiException("Server error", 500, "", null, null));
            var sender = new ApiSender(client.Object, logger);
            var eventRange = GetEventProcessRange();

            // Act.
            var exception = await Record.ExceptionAsync(() => sender.SendAsync(eventRange));

            // Assert.
            Assert.NotNull(exception);
            Assert.IsType<ApiException>(exception);
        }

        #region Helper методы

        private EventProcessRange GetEventProcessRange()
        {
            var sendEvents = new List<SendEvent>()
            {
                new SendEvent(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "bill_payed", "{\"Data\":\"Event 1 Data\"}"),
            };
            var range = new EventProcessRange(1, sendEvents);

            return range;
        }

        #endregion
    }
}
