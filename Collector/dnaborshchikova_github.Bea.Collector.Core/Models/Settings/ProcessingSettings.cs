namespace dnaborshchikova_github.Bea.Collector.Core.Models.Settings
{
    public class ProcessingSettings
    {
        public bool GeneratorRunAsProcess { get; set; }
        public bool GenerateFile { get; set; }
        public string FilePath { get; set; }
        public int ThreadCount { get; set; }
        public string ProcessType { get; set; }
        public string InputFolder { get; set; }
        public string RunMode { get; set; }
    }
}
