using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces
{
    public interface ISendEventLogRepository
    {
        Task SaveSendResultAsync(SendEventLog sendEventLog);
        SendEventLog GetPreviousDaySendLog(string filePath);
    }
}
