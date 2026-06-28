using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;

namespace dnaborshchikova_github.Bea.Collector.Processor.Handlers
{
    public class ConsoleFileSelectionStrategy : IFileSelectionStrategy
    {
        private readonly AppSettings _appSettings;
        
        public ConsoleFileSelectionStrategy(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public List<string> GetFiles()
        {
            var filePath = _appSettings.ProcessingSettings.FilePath;

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            return new List<string> { filePath };
        }
    }
}
