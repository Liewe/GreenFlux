using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace GreenFlux.Infrastructure.DatabaseContexts
{
    public interface IConnectionFactory
    {
        IDbConnection GetDbConnection();
    }

    public class ConnectionFactory : IConnectionFactory
    {
        public IDbConnection GetDbConnection()
        {
            return new SqliteConnection("Data Source=GreenFlux.db");
        }
    }
}
