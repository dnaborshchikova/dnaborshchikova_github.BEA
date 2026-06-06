namespace dnaborshchikova_github.Bea.Collector.Core.Models.Settings
{
    public class GeneratorSettings
    {
        public string FileFormat { get; set; }
        public int PaidBillEventCount { get; set; }
        public int CancelledBillEventCount { get; set; }

        public GeneratorSettings()
        {

        }

        public GeneratorSettings(string fileFormat, int paidBillEventCount, int cancelledBillEventCount)
        {
            FileFormat = fileFormat;
            PaidBillEventCount = paidBillEventCount;
            CancelledBillEventCount = cancelledBillEventCount;
        }
    }
}
