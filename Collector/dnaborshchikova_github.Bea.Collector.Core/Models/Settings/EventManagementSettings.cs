namespace dnaborshchikova_github.Bea.Collector.Core.Models.Settings
{
    public class EventManagementSettings
    {
        public string BaseUrl { get; init; }

        public EventManagementSettings (string baseUrl)
        {
            this.BaseUrl = baseUrl;
        }
    }
}
