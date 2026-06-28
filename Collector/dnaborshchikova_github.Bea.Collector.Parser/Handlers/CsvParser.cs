using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using System.Globalization;

namespace dnaborshchikova_github.Bea.Collector.Parser.Handlers
{
    public class CsvParser : IParser
    {
        public List<BillEvent> Parse(string filePath)
        {
            if (!File.Exists(filePath))
                throw new InvalidOperationException($"Не найден файл по пути {filePath}");

            var extension = Path.GetExtension(filePath);
            const string expectedFileExtension = ".csv";
            if (!string.Equals(extension, expectedFileExtension, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Неверное расширение файла. Ожидалось csv.");

            var billEvents = ParseBillEvents(filePath);
            return billEvents;
        }

        private List<BillEvent> ParseBillEvents(string filePath)
        {
            var billEvents = new List<BillEvent>();

            using (var stream = new StreamReader(filePath))
            {
                var header = stream.ReadLine();

                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    var billEvent = ParseLine(line);
                    billEvents.Add(billEvent);
                }
            }

            return billEvents;
        }

        private BillEvent ParseLine(string line)
        {
            var stringParts = line.Split(',');
            var operationId = Guid.Parse(stringParts[0]);
            var operationDate = DateTime.ParseExact(stringParts[1], "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture);
            var userId = Guid.Parse(stringParts[2]);
            var eventType = stringParts[3];
            var amount = Decimal.Parse(stringParts[4], CultureInfo.InvariantCulture);
            var number = stringParts[5];

            const string billPayed = "bill_payed";
            if (eventType == billPayed)
            {
                var buyerId = Guid.Parse(stringParts[6]);
                var payedBillEvent = new PaidBillEvent(operationId, operationDate, userId,
                    eventType, amount, number, buyerId);

                return payedBillEvent;
            }
            else
            {
                var cancellAmount = Decimal.Parse(stringParts[4], CultureInfo.InvariantCulture);
                var cancelledBillEvent = new CancelledBillEvent(operationId, operationDate, userId,
                    eventType, amount, number, cancellAmount);

                return cancelledBillEvent;
            }
        }
    }
}
