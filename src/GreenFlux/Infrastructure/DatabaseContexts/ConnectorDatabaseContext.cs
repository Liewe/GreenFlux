using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using GreenFlux.Infrastructure.Models;

namespace GreenFlux.Infrastructure.DatabaseContexts
{
    public interface IConnectorDatabaseContext
    {
        void Initialize();
        IEnumerable<Connector> GetByGroupIdentifier(string groupIdentifier);
        IEnumerable<Connector> GetAll();
        int ReplaceChargeStationConnectors(IEnumerable<Connector> connectors);
        int DeleteByChargeStationIdentifier(string chargeStationIdentifier);
        int DeleteByGroupIdentifier(string groupIdentifier);
    }

    public class ConnectorDatabaseContext : IConnectorDatabaseContext
    {
        private readonly IConnectionFactory _connectionFactory;

        public const string TableName = "Connectors";

        public ConnectorDatabaseContext(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Initialize()
        {
            var sql = $@"
                CREATE TABLE IF NOT EXISTS [{TableName}] 
                (
                    [{nameof(Connector.Identifier)}] INTEGER NOT NULL,
                    [{nameof(Connector.ChargeStationIdentifier)}] UNIQUEIDENTIFIER NOT NULL,
                    [{nameof(Connector.GroupIdentifier)}] UNIQUEIDENTIFIER NOT NULL,
                    [{nameof(Connector.MaxCurrentInAmps)}] INTEGER NOT NULL,
                    PRIMARY KEY([{nameof(Connector.ChargeStationIdentifier)}], [{nameof(Connector.Identifier)}])
                    FOREIGN KEY([{nameof(Connector.ChargeStationIdentifier)}]) REFERENCES [{ChargeStationDatabaseContext.TableName}]([{nameof(ChargeStation.Identifier)}]),
                    FOREIGN KEY([{nameof(Connector.GroupIdentifier)}]) REFERENCES [{GroupDatabaseContext.TableName}]([{nameof(Group.Identifier)}])
                );

                CREATE INDEX IF NOT EXISTS [FK_{TableName}_{nameof(Connector.ChargeStationIdentifier)}] 
                ON {TableName}([{nameof(Connector.GroupIdentifier)}]);

                CREATE INDEX IF NOT EXISTS [FK_{TableName}_{nameof(Connector.GroupIdentifier)}] 
                ON {TableName}([{nameof(Connector.GroupIdentifier)}]);";

            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute(sql);
        }
        
        public IEnumerable<Connector> GetByGroupIdentifier(string groupIdentifier)
        {
            var sql = $@"
                SELECT * FROM [{TableName}]
                WHERE [{nameof(Connector.GroupIdentifier)}] = @{nameof(groupIdentifier)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Query<Connector>(sql, new { groupIdentifier });
        }

        public IEnumerable<Connector> GetAll()
        {
            var sql = $@"SELECT * FROM [{TableName}]";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Query<Connector>(sql);
        }

        public int ReplaceChargeStationConnectors(IEnumerable<Connector> connectors)
        {
            var connectorsList = connectors.ToList();

            if (connectorsList == null || !connectorsList.Any())
            {
                throw new ArgumentException("Array should contain at least one connector.", nameof(connectorsList));
            }

            if (connectorsList.Select(c => c.ChargeStationIdentifier).Distinct().Count() != 1)
            {
                throw new ArgumentException("Not all connectors have the same charge station identifier set.", nameof(connectorsList));
            }

            var chargeStationIdentifier = connectorsList.First().ChargeStationIdentifier;

            var deleteSql = $@"           
                DELETE FROM [{TableName}]
                WHERE [{nameof(Connector.ChargeStationIdentifier)}] = @{nameof(chargeStationIdentifier)}";

            var insertSql = $@"           
                INSERT INTO [{TableName}](
                    [{nameof(Connector.Identifier)}],
                    [{nameof(Connector.ChargeStationIdentifier)}],
                    [{nameof(Connector.GroupIdentifier)}],
                    [{nameof(Connector.MaxCurrentInAmps)}])
                VALUES(
                    @{nameof(Connector.Identifier)},
                    @{nameof(Connector.ChargeStationIdentifier)},
                    @{nameof(Connector.GroupIdentifier)},
                    @{nameof(Connector.MaxCurrentInAmps)})";

            using var connection = _connectionFactory.GetDbConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            connection.Execute(deleteSql, new { chargeStationIdentifier}, transaction);
            var rowsAffected = connection.Execute(insertSql, connectorsList, transaction);

            transaction.Commit();

            return rowsAffected;
        }

        public int DeleteByChargeStationIdentifier(string chargeStationIdentifier)
        {
            var sql = $@"           
                DELETE FROM [{TableName}]
                WHERE [{nameof(Connector.ChargeStationIdentifier)}] = @{nameof(chargeStationIdentifier)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, new { chargeStationIdentifier });
        }

        public int DeleteByGroupIdentifier(string groupIdentifier)
        {
            var sql = $@"           
                DELETE FROM [{TableName}]
                WHERE [{nameof(Connector.GroupIdentifier)}] = @{nameof(groupIdentifier)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, new { groupIdentifier });
        }
    }
}
