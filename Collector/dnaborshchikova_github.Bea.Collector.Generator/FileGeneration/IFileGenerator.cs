using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Generator.FileGeneration
{
    public interface IFileGenerator
    {
        public string GenerateFile(List<BillEvent> billEvents, string folderPath);
    }
}
