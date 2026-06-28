using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Generator.DataGeneration
{
    public interface IDataGenerator
    {
        public List<BillEvent> GenerateEvents(int paidBillEventRecordCount
            , int cancelledBillEventRecordCount);
    }
}
