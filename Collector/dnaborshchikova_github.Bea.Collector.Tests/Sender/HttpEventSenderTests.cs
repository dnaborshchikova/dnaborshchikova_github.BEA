using dnaborshchikova_github.Bea.Collector.Senders;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;
using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Tests.Sender
{
    public class HttpEventSenderTests
    {
        [Fact]
        public async Task SendAsync_ShouldSendEventsBatchWithCorrectPayload()
        {
            // Arrange.
            var logger = NullLogger<HttpEventSender>.Instance;

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var messageHandler = new Mock<HttpMessageHandler>();
            messageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post
                    && r.RequestUri == new Uri("https://test.com/api/v1/events/batch")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            var client = CreateHttpClient(messageHandler);
            var sender = new HttpEventSender(client, logger);
            var range = GetEventProcessRange();

            // Act.
            await sender.SendAsync(range);

            // Assert.
            messageHandler.Protected().Verify("SendAsync", Times.Once()
                , ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post 
                    && r.RequestUri == new Uri("https://test.com/api/v1/events/batch")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendAsync_ShouldSendEventsInRequestBody()
        {
            // Arrange.
            var logger = NullLogger<HttpEventSender>.Instance;

            HttpRequestMessage? capturedRequest = null;
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var messageHandler = new Mock<HttpMessageHandler>();
            messageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post
                    && r.RequestUri == new Uri("https://test.com/api/v1/events/batch")), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
                {
                    capturedRequest = request;
                })
                .ReturnsAsync(response);
            var client = CreateHttpClient(messageHandler);
            var sender = new HttpEventSender(client, logger);
            var range = GetEventProcessRange();

            // Act.
            await sender.SendAsync(range);

            // Assert.
            Assert.NotNull(capturedRequest);
            var body = await capturedRequest.Content.ReadAsStringAsync();
            Assert.Contains("bill_payed", body);
            Assert.Contains("Event 1 Data", body);
        }

        [Fact]
        public async Task SendAsync_ShouldCompleteSuccessfully_WhenResponseIsSuccessful()
        {
            // Arragne.
            var logger = NullLogger<HttpEventSender>.Instance;

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var messageHandler = new Mock<HttpMessageHandler>();
            messageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post
                    && r.RequestUri == new Uri("https://test.com/api/v1/events/batch")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            var client = CreateHttpClient(messageHandler);
            var sender = new HttpEventSender(client, logger);
            var range = GetEventProcessRange();

            // Act.
            var exception = await Record.ExceptionAsync(() => sender.SendAsync(range));

            // Assert.
            Assert.Null(exception);
        }

        [Fact]
        public async Task SendAsync_ShouldThrowHttpRequestException_WhenResponseFails()
        {
            // Arragne.
            var logger = NullLogger<HttpEventSender>.Instance;

            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            var messageHandler = new Mock<HttpMessageHandler>();
            messageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post
                    && r.RequestUri == new Uri("https://test.com/api/v1/events/batch")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            var client = CreateHttpClient(messageHandler);
            var sender = new HttpEventSender(client, logger);
            var range = GetEventProcessRange();

            // Act.
            var exception = await Record.ExceptionAsync(() => sender.SendAsync(range));

            // Assert.
            Assert.NotNull(exception);
            Assert.IsType<HttpRequestException>(exception);
        }

        #region Helper методы

        private EventProcessRange GetEventProcessRange()
        {
            var sendEvents = new List<SendEvent>()
            {
                new SendEvent(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "bill_payed", "{\"Data\":\"Event 1 Data\"}"),
                new SendEvent(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "bill_payed", "{\"Nameata\":\"Event 2 Data\"}")
            };
            var range = new EventProcessRange(1, sendEvents);

            return range;
        }

        private HttpClient CreateHttpClient(Mock<HttpMessageHandler> messageHandler)
        {
            return new HttpClient(messageHandler.Object)
            {
                BaseAddress = new Uri("https://test.com/")
            };
        }

        #endregion
    }
}
