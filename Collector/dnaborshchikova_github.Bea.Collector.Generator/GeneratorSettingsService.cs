using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using Microsoft.Extensions.Configuration;

namespace dnaborshchikova_github.Bea.Generator
{
    public class GeneratorSettingsService
    {
        private readonly IConfiguration _configuration;

        public GeneratorSettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public GeneratorSettings GetSettings()
        {
            var settings = _configuration.GetSection(nameof(GeneratorSettings)).Get<GeneratorSettings>();

            return new GeneratorSettings(settings.FileFormat, settings.PaidBillEventCount
                , settings.CancelledBillEventCount);
        }
    }
}
