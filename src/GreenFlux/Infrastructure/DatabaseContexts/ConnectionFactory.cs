using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace GreenFlux.Infrastructure.DatabaseContexts
{
    public interface IConnectionFactory
    {
        DbConnection GetDbConnection();
    }

    public class ConnectionFactory : IConnectionFactory
    {
        public DbConnection GetDbConnection()
        {
            return new SqliteConnection("Data Source=GreenFlux.db");
        }
    }
}
