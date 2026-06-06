namespace dnaborshchikova_github.Bea.Collector.Core.Models.Settings
{
    public class AppSettings
    {
        public ProcessingSettings ProcessingSettings { set; get; }
        public GeneratorSettings GeneratorSettings { set; get; }

        public AppSettings()
        {

        }

        public AppSettings(ProcessingSettings processingSettings, GeneratorSettings generatorSettings)
        {
            this.ProcessingSettings = processingSettings
                ?? throw new ArgumentNullException(nameof(processingSettings));
            this.GeneratorSettings = generatorSettings
                ?? throw new ArgumentNullException(nameof(generatorSettings));
        }
    }
}
