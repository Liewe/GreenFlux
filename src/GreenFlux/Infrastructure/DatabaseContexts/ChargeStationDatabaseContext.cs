using System.Collections.Generic;
using Dapper;
using GreenFlux.Infrastructure.Models;

namespace GreenFlux.Infrastructure.DatabaseContexts
{
    public interface IChargeStationDatabaseContext
    {
        void Initialize();
        IEnumerable<ChargeStation> GetByGroupIdentifier(string groupIdentifier);
        IEnumerable<ChargeStation> GetAll();
        int Save(ChargeStation chargeStation);
        int DeleteByChargeStationIdentifier(string chargeStationIdentifier);
        int DeleteByGroupIdentifier(string groupIdentifier);
    }

    public class ChargeStationDatabaseContext : IChargeStationDatabaseContext
    {
        private readonly IConnectionFactory _connectionFactory;

        public const string TableName = "ChargeStations";

        public ChargeStationDatabaseContext(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Initialize()
        {
            var sql = $@"
                CREATE TABLE IF NOT EXISTS [{TableName}] 
                (
                    [{nameof(ChargeStation.Identifier)}] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
                    [{nameof(ChargeStation.GroupIdentifier)}] UNIQUEIDENTIFIER NOT NULL,
                    [{nameof(ChargeStation.Name)}] TEXT NOT NULL,
                    FOREIGN KEY([{nameof(ChargeStation.GroupIdentifier)}]) REFERENCES [{GroupDatabaseContext.TableName}]([{nameof(Group.Identifier)}])
                );

                CREATE INDEX IF NOT EXISTS [FK_{TableName}_{nameof(ChargeStation.GroupIdentifier)}] 
                ON [{TableName}]([{nameof(ChargeStation.GroupIdentifier)}]);

                CREATE UNIQUE INDEX IF NOT EXISTS [PK_{TableName}] 
                ON [{TableName}]([{nameof(ChargeStation.Identifier)}]);";

            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute(sql);
        }

        public IEnumerable<ChargeStation> GetByGroupIdentifier(string groupIdentifier)
        {
            var sql = $@"
                SELECT * FROM [{TableName}]
                WHERE [{nameof(ChargeStation.GroupIdentifier)}] = @{nameof(groupIdentifier)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Query<ChargeStation>(sql, new { groupIdentifier });
        }

        public IEnumerable<ChargeStation> GetAll()
        {
            var sql = $@"SELECT * FROM [{TableName}]";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Query<ChargeStation>(sql);
        }

        public int Save(ChargeStation chargeStation)
        {
            var sql = $@"           
                INSERT INTO [{TableName}](
                    [{nameof(ChargeStation.Identifier)}],
                    [{nameof(ChargeStation.GroupIdentifier)}],
                    [{nameof(ChargeStation.Name)}])
                VALUES(
                    @{nameof(ChargeStation.Identifier)},
                    @{nameof(ChargeStation.GroupIdentifier)},
                    @{nameof(ChargeStation.Name)})
                ON CONFLICT([{nameof(ChargeStation.Identifier)}])
                DO UPDATE SET 
                    [{nameof(ChargeStation.Name)}] = @{nameof(ChargeStation.Name)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, chargeStation);
        }

        public int DeleteByChargeStationIdentifier(string chargeStationIdentifier)
        {
            var sql = $@"           
                DELETE FROM [{TableName}]
                WHERE [{nameof(ChargeStation.Identifier)}] = @{nameof(chargeStationIdentifier)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, new { chargeStationIdentifier });
        }

        public int DeleteByGroupIdentifier(string groupIdentifier)
        {
            var sql = $@"           
                DELETE FROM [{TableName}]
                WHERE [{nameof(ChargeStation.GroupIdentifier)}] = @{nameof(groupIdentifier)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, new { groupIdentifier });
        }
    }
}
