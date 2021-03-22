using System;
using Dapper;
using GreenFlux.Infrastructure;
using GreenFlux.Infrastructure.DatabaseContexts;
using GreenFlux.Infrastructure.Models;

namespace GreenFlux.Unittests.Utilities
{
    public interface ITestDbUtilities
    {
        void DeleteAll();
        void DeleteAllGroups();
        void AddGroup(Guid identifier, string name, int capacityInAmps);
        void DeleteAllChargeStations();
        void AddChargeStation(Guid identifier, Guid groupIdentifier, string name);
        void DeleteAllConnectors();
        void AddConnector(short identifier, Guid chargeStationIdentifier, Guid groupIdentifier, int maxCurrentInAmps);
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
            connection.Execute($@"DELETE FROM [{GroupDatabaseContext.TableName}]");
        }

        public void AddGroup(Guid identifier, string name, int capacityInAmps)
        {
            var sql = $@"           
                INSERT INTO [{GroupDatabaseContext.TableName}](
                    [{nameof(Group.Identifier)}],
                    [{nameof(Group.Name)}],
                    [{nameof(Group.CapacityInAmps)}])
                VALUES(
                    @{nameof(Group.Identifier)},
                    @{nameof(Group.Name)},
                    @{nameof(Group.CapacityInAmps)})";

            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute(sql, new Group{
                Identifier = identifier.ToString(),
                Name = name,
                CapacityInAmps = capacityInAmps
            });
        }
        public void DeleteAllChargeStations()
        {
            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute($@"DELETE FROM [{ChargeStationDatabaseContext.TableName}]");
        }

        public void AddChargeStation(Guid identifier, Guid groupIdentifier, string name)
        {
            var sql = $@"           
                INSERT INTO [{ChargeStationDatabaseContext.TableName}](
                    [{nameof(ChargeStation.Identifier)}],
                    [{nameof(ChargeStation.GroupIdentifier)}],
                    [{nameof(ChargeStation.Name)}])
                VALUES(
                    @{nameof(ChargeStation.Identifier)},
                    @{nameof(ChargeStation.GroupIdentifier)},
                    @{nameof(ChargeStation.Name)})";

            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute(sql, new ChargeStation
            {
                Identifier = identifier.ToString(),
                GroupIdentifier = groupIdentifier.ToString(),
                Name = name
            });
        }

        public void DeleteAllConnectors()
        {
            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute($@"DELETE FROM [{ConnectorDatabaseContext.TableName}]");
        }

        public void AddConnector(short identifier, Guid chargeStationIdentifier, Guid groupIdentifier, int maxCurrentInAmps)
        {
            var insertSql = $@"           
                INSERT INTO [{ConnectorDatabaseContext.TableName}](
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
            connection.Execute(insertSql, new Connector
            {
                Identifier = identifier,
                ChargeStationIdentifier = chargeStationIdentifier.ToString(),
                GroupIdentifier = groupIdentifier.ToString(),
                MaxCurrentInAmps = maxCurrentInAmps
            });
        }
    }
}
