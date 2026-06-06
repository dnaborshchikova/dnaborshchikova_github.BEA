using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Processor.Handlers
{
    public static class RangeGenerator
    {
        public static List<EventProcessRange> GenerateParts(List<BillEvent> billEvents, int threadCount)
        {
            var dateRanges = GetDataRanges(billEvents, threadCount);
            var eventRanges = new List<EventProcessRange>();

            for (int i = 0; i < dateRanges.Count; i++)
            {
                var nextRangeIndex = i + 1;
                var events = new List<BillEvent>();
                if (nextRangeIndex < dateRanges.Count)
                {
                    events = billEvents.Where(e => e.OperationDateTime >= dateRanges[i]
                        && e.OperationDateTime < dateRanges[nextRangeIndex]).ToList();
                }
                else
                {
                    events = billEvents.Where(e => e.OperationDateTime >= dateRanges[i]).ToList();
                }

                var range = new EventProcessRange(i + 1, events);
                eventRanges.Add(range);
            }

            return eventRanges;
        }

        private static List<DateTime> GetDataRanges(List<BillEvent> billEvents, int threadCount)
        {
            var dates = billEvents.Select(e => e.OperationDateTime).ToList();
            var minDate = dates.Min();
            var maxDate = dates.Max();
            var total = maxDate - minDate;
            var step = TimeSpan.FromTicks(total.Ticks / threadCount);
            var dateRanges = new List<DateTime>();

            if (threadCount != 1)
            {
                for (int i = 0; i < threadCount; i++)
                    dateRanges.Add(minDate + TimeSpan.FromTicks(step.Ticks * i));
            }
            else
            {
                dateRanges = new List<DateTime> { minDate };
            }

            return dateRanges;
        }
    }
}
