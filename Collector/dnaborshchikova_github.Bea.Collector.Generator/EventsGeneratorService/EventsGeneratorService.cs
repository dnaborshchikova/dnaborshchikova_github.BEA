using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Generator.DataGeneration;
using dnaborshchikova_github.Bea.Generator.FileGeneration;

namespace dnaborshchikova_github.Bea.Generator.EventsGeneratorService
{
    public class EventsGeneratorService : IEventsGeneratorService
    {
        private readonly GeneratorSettings _generatorSettings;
        private readonly IDataGenerator _dataGenerator;
        private readonly Func<string, IFileGenerator> _fileGeneratorFactory;

        public EventsGeneratorService(IDataGenerator dataGenerator, Func<string, IFileGenerator> fileGeneratorFactory
            , GeneratorSettings generatorSettings)
        {
            _generatorSettings = generatorSettings;
            _dataGenerator = dataGenerator;
            _fileGeneratorFactory = fileGeneratorFactory;
        }

        public string GenerateEvents(string folderPath)
        {
            var events = _dataGenerator.GenerateEvents(_generatorSettings.PaidBillEventCount
                , _generatorSettings.CancelledBillEventCount);
            var fileGenerator = _fileGeneratorFactory(_generatorSettings.FileFormat);


            var filePath = fileGenerator.GenerateFile(events, folderPath);

            return filePath;
        }
    }
}
