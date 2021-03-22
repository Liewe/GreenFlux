using System.Collections.Generic;
using Dapper;
using GreenFlux.Infrastructure.Models;

namespace GreenFlux.Infrastructure.DbContexts
{
    public interface IChargeStationDbContext
    {
        void Initialize();
        IEnumerable<ChargeStation> GetByGroupId(string groupId);
        IEnumerable<ChargeStation> GetAll();
        int Save(ChargeStation chargeStation);
        int DeleteByChargeStationId(string chargeStationId);
        int DeleteByGroupId(string groupId);
    }

    public class ChargeStationDbContext : IChargeStationDbContext
    {
        private readonly IConnectionFactory _connectionFactory;

        public const string TableName = "ChargeStations";

        public ChargeStationDbContext(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Initialize()
        {
            var sql = $@"
                CREATE TABLE IF NOT EXISTS [{TableName}] 
                (
                    [{nameof(ChargeStation.Id)}] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
                    [{nameof(ChargeStation.GroupId)}] UNIQUEIDENTIFIER NOT NULL,
                    [{nameof(ChargeStation.Name)}] TEXT NOT NULL,
                    FOREIGN KEY([{nameof(ChargeStation.GroupId)}]) REFERENCES [{GroupDbContext.TableName}]([{nameof(Group.Id)}])
                );

                CREATE INDEX IF NOT EXISTS [FK_{TableName}_{nameof(ChargeStation.GroupId)}] 
                ON [{TableName}]([{nameof(ChargeStation.GroupId)}]);

                CREATE UNIQUE INDEX IF NOT EXISTS [PK_{TableName}] 
                ON [{TableName}]([{nameof(ChargeStation.Id)}]);";

            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute(sql);
        }

        public IEnumerable<ChargeStation> GetByGroupId(string groupId)
        {
            var sql = $@"
                SELECT * FROM [{TableName}]
                WHERE [{nameof(ChargeStation.GroupId)}] = @{nameof(groupId)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Query<ChargeStation>(sql, new { groupId });
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
                    [{nameof(ChargeStation.Id)}],
                    [{nameof(ChargeStation.GroupId)}],
                    [{nameof(ChargeStation.Name)}])
                VALUES(
                    @{nameof(ChargeStation.Id)},
                    @{nameof(ChargeStation.GroupId)},
                    @{nameof(ChargeStation.Name)})
                ON CONFLICT([{nameof(ChargeStation.Id)}])
                DO UPDATE SET 
                    [{nameof(ChargeStation.Name)}] = @{nameof(ChargeStation.Name)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, chargeStation);
        }

        public int DeleteByChargeStationId(string chargeStationId)
        {
            var sql = $@"           
                DELETE FROM [{TableName}]
                WHERE [{nameof(ChargeStation.Id)}] = @{nameof(chargeStationId)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, new { chargeStationId });
        }

        public int DeleteByGroupId(string groupId)
        {
            var sql = $@"           
                DELETE FROM [{TableName}]
                WHERE [{nameof(ChargeStation.GroupId)}] = @{nameof(groupId)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, new { groupId });
        }
    }
}
