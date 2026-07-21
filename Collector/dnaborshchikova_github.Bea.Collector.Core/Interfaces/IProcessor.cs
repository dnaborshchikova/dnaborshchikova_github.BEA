using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Core.Interfaces
{
    public interface IProcessor
    {
        public Task<bool> ProcessAsync(List<EventProcessRange> ranges);
    }
}
