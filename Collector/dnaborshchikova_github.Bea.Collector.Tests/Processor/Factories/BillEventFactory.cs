using dnaborshchikova_github.Bea.Collector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace dnaborshchikova_github.Bea.Collector.Tests.Processor.Factories
{
    public static class BillEventFactory
    {
        public static BillEvent CreatePaidBillEvent(DateTime date, Guid userId, decimal amount, string number)
        {
            return new BillEvent(Guid.NewGuid(), date, userId, "bill_payed", amount, number);
        }

        public static BillEvent CreateCancelledBillEvent(DateTime date, Guid userId, decimal amount, string number)
        {
            return new BillEvent(Guid.NewGuid(), date, userId, "bill_canceled", amount, number);
        }
    }
}
