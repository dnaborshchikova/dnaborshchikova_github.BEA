using dnaborshchikova_github.Bea.Collector.Core.Models;
using System.Globalization;
using System.Text;

namespace dnaborshchikova_github.Bea.Generator.FileGeneration
{
    public class CsvFileGenerator : IFileGenerator
    {
        public string GenerateFile(List<BillEvent> billEvents, string folderPath)
        {
            var fileName = $"{DateTime.Now:dd.MM.yyyy}_BillEvent.csv";
            var filePath = Path.Combine(folderPath, fileName);

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                var billEventProperties = typeof(BillEvent).GetProperties();
                var paidBillEventProperties = typeof(PaidBillEvent).GetProperties();
                var cancelledBillEventProperties = typeof(CancelledBillEvent).GetProperties();
                var allProperties = billEventProperties
                    .Concat(paidBillEventProperties)
                    .Concat(cancelledBillEventProperties)
                    .GroupBy(p => p.Name)
                    .Select(g => g.First())
                    .ToList();

                writer.WriteLine(string.Join(",", allProperties.Select(p => p.Name)));

                foreach (var bill in billEvents)
                {
                    var billRecord = allProperties.Select(p =>
                    {
                        var property = bill.GetType().GetProperty(p.Name);
                        if (property == null)
                            return string.Empty;

                        var value = p.GetValue(bill);

                        if (value == null)
                            return string.Empty;

                        if (value is decimal decimalValue)
                            return decimalValue.ToString(CultureInfo.InvariantCulture);

                        if (value is DateTime dateTimeValue)
                            return dateTimeValue.ToString("dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture);

                        return value.ToString();
                    });

                    writer.WriteLine(string.Join(",", billRecord));
                }
            }

            return filePath;
        }
    }
}
