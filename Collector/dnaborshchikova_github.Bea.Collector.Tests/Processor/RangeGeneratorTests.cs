using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Processor.Handlers;
using dnaborshchikova_github.Bea.Collector.Tests.Processor.Factories;
using FluentAssertions;
using Xunit;

namespace dnaborshchikova_github.Bea.Collector.Tests.Processor
{
    public class RangeGeneratorTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        public void GenerateParts_ThreadCount_RangesCountEqualsThreadCount(int threadCount)
        {
            //Arrange
            var userId = Guid.NewGuid();
            var buyerId = Guid.NewGuid();
            var billEvents = new List<BillEvent>
            {
                BillEventFactory.CreatePaidBillEvent(new DateTime(2025, 12, 10, 10, 0, 0), userId, 100m, "INV-001", buyerId),
                BillEventFactory.CreatePaidBillEvent(new DateTime(2025, 12, 10, 10, 0, 0), userId, 100m, "INV-001", buyerId),
                BillEventFactory.CreatePaidBillEvent(new DateTime(2025, 12, 11, 9, 0, 0), userId, 150m, "INV-003", Guid.NewGuid()),
                BillEventFactory.CreatePaidBillEvent(new DateTime(2025, 12, 12, 18, 30, 0), userId, 300m, "INV-004", null),
                BillEventFactory.CreatePaidBillEvent(new DateTime(2024, 12, 10, 10, 0, 0), userId, 100m, "INV-001", buyerId),
                BillEventFactory.CreatePaidBillEvent(new DateTime(2025, 07, 11, 9, 0, 0), userId, 150m, "INV-003", Guid.NewGuid()),
                BillEventFactory.CreatePaidBillEvent(new DateTime(2025, 08, 12, 18, 30, 0), userId, 300m, "INV-004", buyerId),
                BillEventFactory.CreatePaidBillEvent(new DateTime(2025, 01, 10, 10, 0, 0), userId, 100m, "INV-001", null),
                BillEventFactory.CreatePaidBillEvent(new DateTime(2025, 03, 11, 9, 0, 0), userId, 150m, "INV-003", buyerId),
                BillEventFactory.CreatePaidBillEvent(new DateTime(2025, 04, 12, 18, 30, 0), userId, 300m, "INV-004", Guid.NewGuid()),
                BillEventFactory.CreatePaidBillEvent(new DateTime(2025, 12, 11, 9, 0, 0), userId, 150m, "INV-003", buyerId),
                BillEventFactory.CreatePaidBillEvent(new DateTime(2025, 12, 12, 18, 30, 0), userId, 300m, "INV-004", null),

                BillEventFactory.CreateCancelledBillEvent(new DateTime(2025, 12, 10, 12, 0, 0), userId, 200m, "INV-002", 50m),
                BillEventFactory.CreateCancelledBillEvent(new DateTime(2024, 12, 10, 12, 0, 0), userId, 200m, "INV-002", 200m),
                BillEventFactory.CreateCancelledBillEvent(new DateTime(2025, 01, 10, 12, 0, 0), userId, 200m, "INV-002", null),
            };

            //Act
            var ranges = RangeGenerator.GenerateParts(billEvents, threadCount);

            //Assert
            ranges.Count.Should().Be(threadCount);
            ranges.ForEach(r => r.SendEvents.Count.Should().NotBe(0));
        }
    }
}
