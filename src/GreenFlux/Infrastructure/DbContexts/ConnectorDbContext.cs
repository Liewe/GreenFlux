using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using GreenFlux.Infrastructure.Models;

namespace GreenFlux.Infrastructure.DbContexts
{
    public interface IConnectorDbContext
    {
        void Initialize();
        IEnumerable<Connector> GetByGroupId(string groupId);
        IEnumerable<Connector> GetAll();
        int ReplaceChargeStationConnectors(IEnumerable<Connector> connectors);
        int DeleteByChargeStationId(string chargeStationId);
        int DeleteByGroupId(string groupId);
    }

    public class ConnectorDbContext : IConnectorDbContext
    {
        private readonly IConnectionFactory _connectionFactory;

        public const string TableName = "Connectors";

        public ConnectorDbContext(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Initialize()
        {
            var sql = $@"
                CREATE TABLE IF NOT EXISTS [{TableName}] 
                (
                    [{nameof(Connector.Id)}] INTEGER NOT NULL,
                    [{nameof(Connector.ChargeStationId)}] UNIQUEIDENTIFIER NOT NULL,
                    [{nameof(Connector.GroupId)}] UNIQUEIDENTIFIER NOT NULL,
                    [{nameof(Connector.MaxCurrentInAmps)}] INTEGER NOT NULL,
                    PRIMARY KEY([{nameof(Connector.ChargeStationId)}], [{nameof(Connector.Id)}])
                    FOREIGN KEY([{nameof(Connector.ChargeStationId)}]) REFERENCES [{ChargeStationDbContext.TableName}]([{nameof(ChargeStation.Id)}]),
                    FOREIGN KEY([{nameof(Connector.GroupId)}]) REFERENCES [{GroupDbContext.TableName}]([{nameof(Group.Id)}])
                );

                CREATE INDEX IF NOT EXISTS [FK_{TableName}_{nameof(Connector.ChargeStationId)}] 
                ON {TableName}([{nameof(Connector.GroupId)}]);

                CREATE INDEX IF NOT EXISTS [FK_{TableName}_{nameof(Connector.GroupId)}] 
                ON {TableName}([{nameof(Connector.GroupId)}]);";

            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute(sql);
        }
        
        public IEnumerable<Connector> GetByGroupId(string groupId)
        {
            var sql = $@"
                SELECT * FROM [{TableName}]
                WHERE [{nameof(Connector.GroupId)}] = @{nameof(groupId)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Query<Connector>(sql, new { groupId });
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

            if (connectorsList.Select(c => c.ChargeStationId).Distinct().Count() != 1)
            {
                throw new ArgumentException("Not all connectors have the same charge station id set.", nameof(connectorsList));
            }

            var chargeStationId = connectorsList.First().ChargeStationId;

            var deleteSql = $@"           
                DELETE FROM [{TableName}]
                WHERE [{nameof(Connector.ChargeStationId)}] = @{nameof(chargeStationId)}";

            var insertSql = $@"           
                INSERT INTO [{TableName}](
                    [{nameof(Connector.Id)}],
                    [{nameof(Connector.ChargeStationId)}],
                    [{nameof(Connector.GroupId)}],
                    [{nameof(Connector.MaxCurrentInAmps)}])
                VALUES(
                    @{nameof(Connector.Id)},
                    @{nameof(Connector.ChargeStationId)},
                    @{nameof(Connector.GroupId)},
                    @{nameof(Connector.MaxCurrentInAmps)})";

            using var connection = _connectionFactory.GetDbConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            connection.Execute(deleteSql, new { chargeStationId}, transaction);
            var rowsAffected = connection.Execute(insertSql, connectorsList, transaction);

            transaction.Commit();

            return rowsAffected;
        }

        public int DeleteByChargeStationId(string chargeStationId)
        {
            var sql = $@"           
                DELETE FROM [{TableName}]
                WHERE [{nameof(Connector.ChargeStationId)}] = @{nameof(chargeStationId)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, new { chargeStationId });
        }

        public int DeleteByGroupId(string groupId)
        {
            var sql = $@"           
                DELETE FROM [{TableName}]
                WHERE [{nameof(Connector.GroupId)}] = @{nameof(groupId)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, new { groupId });
        }
    }
}
