using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;

namespace dnaborshchikova_github.Bea.Collector.Tests.Processor.Builders
{
    public class GeneratorSettingsBuilder
    {
        private GeneratorSettings _generatorSettings;

        public static GeneratorSettingsBuilder Default()
        {
            return new GeneratorSettingsBuilder
            {
                _generatorSettings = new GeneratorSettings
                {
                    FileFormat = "csv",
                    PaidBillEventCount = 700_000,
                    CancelledBillEventCount = 300_000
                }
            };
        }

        public GeneratorSettingsBuilder WithPaidBillEvents(int count)
        {
            _generatorSettings.PaidBillEventCount = count;
            return this;
        }

        public GeneratorSettingsBuilder WithCancelledBillEvents(int count)
        {
            _generatorSettings.CancelledBillEventCount = count;
            return this;
        }

        public GeneratorSettings Build()
        {
            return _generatorSettings;
        }
    }
}
