using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;
using System.Globalization;

namespace dnaborshchikova_github.Bea.Collector.Processor.Handlers
{
    public class WorkerFileSelectionStrategy : IFileSelectionStrategy
    {
        private readonly ISendEventLogRepository _workerServiceLogRepository;
        private readonly AppSettings _appSettings;

        public WorkerFileSelectionStrategy(ISendEventLogRepository workerServiceLogRepository
            , AppSettings appSettings)
        {
            _workerServiceLogRepository = workerServiceLogRepository;
            _appSettings = appSettings;
        }

        public List<string> GetFiles()
        {
            var currentDate = GetCurrentDate();
            var filePaths = new List<string>();
            var previuosDayFileName = GetFileName(currentDate.AddDays(-1));
            var previuosDaySendLog = _workerServiceLogRepository.GetPreviousDaySendLog(previuosDayFileName);
            if (previuosDaySendLog != null && !previuosDaySendLog.IsSendCompleted)
            {
                var filePath = BuildFullPath(previuosDayFileName);
                if (IsFileExists(filePath))
                {
                    filePaths.Add(filePath);
                }
            }
            var currentDayFileName = GetFileName(currentDate);
            var currentDayFilePath = BuildFullPath(currentDayFileName);
            if (IsFileExists(currentDayFilePath))
            {
                filePaths.Add(currentDayFilePath);
            }

            return filePaths;
        }

        private string BuildFullPath(string fileName)
        {
            var basePath = AppContext.BaseDirectory;
            var inputFolderName = _appSettings.ProcessingSettings.InputFolder;
            var inputFolderPath = Path.Combine(basePath, inputFolderName, fileName);

            return inputFolderPath;
        }

        private string GetFileName(DateTime date)
        {
            return date.ToString("dd.MM.yyyy") + "_BillEvent.csv";
        }

        private bool IsFileExists(string filePath)
        {
            var isFileExists = File.Exists(filePath);
            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException($"File {filePath} does not exist.");
            }

            return isFileExists;
        }

        private DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }
    }
}
