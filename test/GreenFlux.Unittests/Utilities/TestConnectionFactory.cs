using System.Data;
using GreenFlux.Infrastructure.DbContexts;
using Microsoft.Data.Sqlite;

namespace GreenFlux.Unittests.Utilities
{
    public class TestConnectionFactory : IConnectionFactory
    {
        private readonly InMemoryDbSqliteConnection _connection = new InMemoryDbSqliteConnection();

        public IDbConnection GetDbConnection()
        {
            return _connection;
        }

        /// <summary>
        /// Creates and opens database to in memory database, the database exists as long as the connection stays open,
        /// this wrapper is created to prevent the connection being closed or disposed.
        /// </summary>
        private class InMemoryDbSqliteConnection : IDbConnection
        {
            private readonly IDbConnection _innerConnection;

            public InMemoryDbSqliteConnection()
            {
                _innerConnection = new SqliteConnection("Data Source=:memory:");
                _innerConnection.Open();
            }

            public void Dispose() { }
            public IDbTransaction BeginTransaction() => _innerConnection.BeginTransaction();
            public IDbTransaction BeginTransaction(IsolationLevel il) => _innerConnection.BeginTransaction(il);
            public void ChangeDatabase(string databaseName) => _innerConnection.ChangeDatabase(databaseName);
            public void Close() { }
            public IDbCommand CreateCommand() => _innerConnection.CreateCommand();
            public void Open() { }
            public string ConnectionString { get => _innerConnection.ConnectionString; set => _innerConnection.ConnectionString = value; }
            public int ConnectionTimeout { get => _innerConnection.ConnectionTimeout; }
            public string Database { get => _innerConnection.Database; }
            public ConnectionState State { get => _innerConnection.State; }
        }
    }
}
