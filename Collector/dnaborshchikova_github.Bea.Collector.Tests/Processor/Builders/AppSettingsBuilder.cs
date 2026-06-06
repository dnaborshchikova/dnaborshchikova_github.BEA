using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;

namespace dnaborshchikova_github.Bea.Collector.Tests.Processor.Builders
{
    public class AppSettingsBuilder
    {
        private AppSettings appSettings = new AppSettings();

        public AppSettingsBuilder WithProcessingSettings(ProcessingSettings processingSettings)
        {
            appSettings.ProcessingSettings = processingSettings;

            return this;
        }

        public AppSettingsBuilder WithGeneratorSettings(GeneratorSettings generatorSettings)
        {
            appSettings.GeneratorSettings = generatorSettings;

            return this;
        }

        public AppSettings Build()
        {
            return appSettings;
        }
    }
}
