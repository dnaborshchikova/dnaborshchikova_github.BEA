using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Core.Interfaces
{
    public interface IParser
    {
        public List<BillEvent> Parse(string filePath);
    }
}
