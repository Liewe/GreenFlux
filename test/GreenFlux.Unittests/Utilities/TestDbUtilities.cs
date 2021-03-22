using System;
using Dapper;
using GreenFlux.Infrastructure;
using GreenFlux.Infrastructure.DbContexts;
using GreenFlux.Infrastructure.Models;

namespace GreenFlux.Unittests.Utilities
{
    public interface ITestDbUtilities
    {
        void DeleteAll();
        void DeleteAllGroups();
        void AddGroup(Guid id, string name, int capacityInAmps);
        void DeleteAllChargeStations();
        void AddChargeStation(Guid id, Guid groupId, string name);
        void DeleteAllConnectors();
        void AddConnector(short id, Guid chargeStationId, Guid groupId, int maxCurrentInAmps);
    }

    public class TestDbUtilities : ITestDbUtilities
    {
        private readonly IConnectionFactory _connectionFactory;

        public TestDbUtilities(IConnectionFactory connectionFactory, IRepository repository)
        {
            _connectionFactory = connectionFactory;
            repository.Initialize();
        }
        
        public void DeleteAll()
        {
            DeleteAllConnectors();
            DeleteAllChargeStations();
            DeleteAllGroups();
        }

        public void DeleteAllGroups()
        {
            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute($@"DELETE FROM [{GroupDbContext.TableName}]");
        }

        public void AddGroup(Guid id, string name, int capacityInAmps)
        {
            var sql = $@"           
                INSERT INTO [{GroupDbContext.TableName}](
                    [{nameof(Group.Id)}],
                    [{nameof(Group.Name)}],
                    [{nameof(Group.CapacityInAmps)}])
                VALUES(
                    @{nameof(Group.Id)},
                    @{nameof(Group.Name)},
                    @{nameof(Group.CapacityInAmps)})";

            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute(sql, new Group{
                Id = id.ToString(),
                Name = name,
                CapacityInAmps = capacityInAmps
            });
        }
        public void DeleteAllChargeStations()
        {
            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute($@"DELETE FROM [{ChargeStationDbContext.TableName}]");
        }

        public void AddChargeStation(Guid id, Guid groupId, string name)
        {
            var sql = $@"           
                INSERT INTO [{ChargeStationDbContext.TableName}](
                    [{nameof(ChargeStation.Id)}],
                    [{nameof(ChargeStation.GroupId)}],
                    [{nameof(ChargeStation.Name)}])
                VALUES(
                    @{nameof(ChargeStation.Id)},
                    @{nameof(ChargeStation.GroupId)},
                    @{nameof(ChargeStation.Name)})";

            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute(sql, new ChargeStation
            {
                Id = id.ToString(),
                GroupId = groupId.ToString(),
                Name = name
            });
        }

        public void DeleteAllConnectors()
        {
            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute($@"DELETE FROM [{ConnectorDbContext.TableName}]");
        }

        public void AddConnector(short id, Guid chargeStationId, Guid groupId, int maxCurrentInAmps)
        {
            var insertSql = $@"           
                INSERT INTO [{ConnectorDbContext.TableName}](
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
            connection.Execute(insertSql, new Connector
            {
                Id = id,
                ChargeStationId = chargeStationId.ToString(),
                GroupId = groupId.ToString(),
                MaxCurrentInAmps = maxCurrentInAmps
            });
        }
    }
}
