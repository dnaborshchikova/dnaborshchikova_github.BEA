using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace dnaborshchikova_github.Bea.EventManagement.Infrastructure.ExceptionExtensions
{
    public static class DbUpdateExceptionExtensions
    {
        public static bool IsDuplicateKey(this DbUpdateException ex)
        {
            return ex.GetBaseException() is PostgresException pgEx &&
                pgEx.SqlState == "23505";
        }
    }
}
