using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.Core.Validators;
using dnaborshchikova_github.Bea.Collector.WorkerService.Models;

namespace dnaborshchikova_github.Bea.Collector.WorkerService.Validators
{
    public class WorkerSettingsValidator : SettingsValidatorBase
    {
        public void Validate(GeneratorSettings generatorSettings, ProcessingSettings processingSettings,
            WorkerServiceSettings workerSettings)
        {
            ValidateGeneratorSettings(generatorSettings);
            ValidateProcessingSettings(processingSettings);
            ValidateWorkerServiceSettings(workerSettings);

            if (processingSettings.RunMode == "ScheduledService")
            {
                if (workerSettings == null || workerSettings.IntervalHours <= 0)
                    throw new InvalidOperationException(
                        "Для ScheduledService необходимо указать IntervalHours > 0");
            }
        }

        public void ValidateWorkerServiceSettings(WorkerServiceSettings settings)
        {
            if (settings.IntervalHours <= 0)
                throw new InvalidOperationException($"{nameof(settings.IntervalHours)} должен быть > 0");
        }

        public override void ValidateProcessingSettings(ProcessingSettings processingSettings)
        {
            if (processingSettings == null)
                throw new ArgumentNullException(nameof(processingSettings));

            if (string.IsNullOrEmpty(processingSettings.FilePath) && string.IsNullOrEmpty(processingSettings.InputFolder))
                throw new InvalidOperationException("Для Worker необходимо указать FilePath или InputFolder");

            if (processingSettings.ThreadCount <= 0)
                throw new InvalidOperationException("ThreadCount должен быть > 0");

            if (string.IsNullOrEmpty(processingSettings.ProcessType)
                || (processingSettings.ProcessType != "Thread" && processingSettings.ProcessType != "Task"))
                throw new InvalidOperationException("ProcessType должен быть 'Thread' или 'Task'");

            if (string.IsNullOrEmpty(processingSettings.RunMode) 
                || (processingSettings.RunMode != "OneTime" && processingSettings.RunMode != "ScheduledService"))
                throw new InvalidOperationException("RunMode должен быть 'OneTime' или 'ScheduledService'");
        }
    }
}
