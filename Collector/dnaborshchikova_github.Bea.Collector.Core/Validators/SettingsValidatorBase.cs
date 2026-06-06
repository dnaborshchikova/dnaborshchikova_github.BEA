using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;

namespace dnaborshchikova_github.Bea.Collector.Core.Validators
{
    public abstract class SettingsValidatorBase
    {
        public void ValidateGeneratorSettings(GeneratorSettings generatorSettings)
        {
            if (generatorSettings == null) 
                throw new ArgumentNullException(nameof(generatorSettings));

            if (string.IsNullOrEmpty(generatorSettings.FileFormat))
                throw new InvalidOperationException("Не указано значение параметра FileFormat.");

            if (generatorSettings.PaidBillEventCount <= 0)
                throw new InvalidOperationException("Не указано значение параметра PaidBillEventCount.");

            if (generatorSettings.CancelledBillEventCount <= 0)
                throw new InvalidOperationException("Не указано значение параметра CancelledBillEventCount.");
        }

        public abstract void ValidateProcessingSettings(ProcessingSettings processingSettings);
    }
}
