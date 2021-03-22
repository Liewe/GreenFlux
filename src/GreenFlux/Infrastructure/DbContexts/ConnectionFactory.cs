using System.Data;
using Microsoft.Data.Sqlite;

namespace GreenFlux.Infrastructure.DbContexts
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
