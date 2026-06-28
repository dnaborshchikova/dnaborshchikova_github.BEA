using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Core.Interfaces
{
    [Obsolete("This interface is obsolete.")]
    public interface ICompositeEventSender
    {
        void Send(EventProcessRange range);
        Task SendAsync(EventProcessRange range);
    }
}
