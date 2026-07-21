using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Sender.Handlers
{
    [Obsolete("This class is obsolete.")]
    public class CompositeEventSender : ICompositeEventSender
    {
        private readonly IEnumerable<IEventSender> _senders;

        public CompositeEventSender(IEnumerable<IEventSender> senders)
        {
            _senders = senders;
        }

        public void Send(EventProcessRange range)
        {
            foreach (var sender in _senders)
            {
                sender.Send(range);
            }
        }

        public async Task SendAsync(EventProcessRange range)
        {
            var tasks = _senders.Select(sender => sender.SendAsync(range)).ToList();

            await Task.WhenAll(tasks);
        }
    }
}
