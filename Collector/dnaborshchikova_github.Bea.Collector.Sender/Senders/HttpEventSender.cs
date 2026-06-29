using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace dnaborshchikova_github.Bea.Collector.Senders
{
    public class HttpEventSender : IEventSender
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpEventSender> _logger;

        public HttpEventSender(HttpClient httpClient, ILogger<HttpEventSender> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public void Send(EventProcessRange range)
        {
            SendAsync(range).GetAwaiter().GetResult();
        }

        public async Task SendAsync(EventProcessRange range)
        {
            foreach (var sendEvent in range.SendEvents)
            {
                var response = await _httpClient.PostAsJsonAsync("api/v1/events",  sendEvent);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to send event. Status: {Status}, Body: {Body}"
                        , response.StatusCode, body);
                    response.EnsureSuccessStatusCode();
                }

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
