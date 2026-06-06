namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class SendEventLog
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public DateTime RunDateTime { get; set; }
        public string RunSettings  { get; set; }
        public bool IsSendCompleted { get; set; }

        public SendEventLog(string fileName, DateTime runDateTime, string runSettings
            , bool isSendCompleted)
        {
            this.FileName = fileName;
            this.RunDateTime = runDateTime;
            this.RunSettings = runSettings;
            this.IsSendCompleted = isSendCompleted;
        }
    }
}
