using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.Core.Validators;

namespace dnaborshchikova_github.Bea.Collector.App.Validators
{
    public class AppSettingsValidator : SettingsValidatorBase
    {
        public void Validate(GeneratorSettings generatorSettings, ProcessingSettings processingSettings)
        {
            ValidateGeneratorSettings(generatorSettings);
            ValidateProcessingSettings(processingSettings);
        }

        public override void ValidateProcessingSettings(ProcessingSettings processingSettings)
        {
            if (processingSettings == null)
                throw new ArgumentNullException(nameof(processingSettings));

            if (processingSettings.GenerateFile && string.IsNullOrEmpty(processingSettings.FilePath))
                throw new InvalidOperationException("FilePath обязателен при генерации файла");

            if (!processingSettings.GenerateFile && string.IsNullOrEmpty(processingSettings.FilePath))
                throw new InvalidOperationException("FilePath обязателен для обработки существующего файла");

            if (processingSettings.ThreadCount <= 0)
                throw new InvalidOperationException("ThreadCount должен быть > 0");

            if (string.IsNullOrEmpty(processingSettings.ProcessType)
                || (processingSettings.ProcessType != "Thread" && processingSettings.ProcessType != "Task"))
                throw new InvalidOperationException("ProcessType должен быть 'Thread' или 'Task'");
        }
    }
}
