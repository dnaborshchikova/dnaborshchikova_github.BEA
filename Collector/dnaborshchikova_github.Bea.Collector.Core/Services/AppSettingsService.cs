using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.Core.Validators;

namespace dnaborshchikova_github.Bea.Collector.Core.Services
{
    public class AppSettingsService
    {
        public AppSettings CreateAppSettings(GeneratorSettings generatorSettings
            , ProcessingSettings processingSettings)
        {
            if (generatorSettings == null)
                throw new Exception("Не заполнена секция GeneratorSettings в конфигурационном файле.");
            if (processingSettings == null)
                throw new Exception("Не заполнена секция ProcessingSettings в конфигурационном файле.");

            return new AppSettings(processingSettings, generatorSettings);
        }
    }
}
