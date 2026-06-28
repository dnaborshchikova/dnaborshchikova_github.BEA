using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Core.Interfaces
{
    public interface IEventSender
    {
        public void Send(EventProcessRange range);
        public Task SendAsync(EventProcessRange range);
    }
}
