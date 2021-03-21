using System;
using System.Collections.Generic;
using System.Linq;
using GreenFlux.Domain.Models;
using GreenFlux.Infrastructure.DatabaseContexts;

namespace GreenFlux.Infrastructure
{
    public interface IRepository
    {
        void Initialize();

        IEnumerable<Group> GetGroups();

        Group GetGroup(Guid identifier);

        bool SaveGroup(Group group);

        bool DeleteGroup(Guid identifier);
        
        bool SaveChargeStation(ChargeStation chargeStationDomainModel);

        bool DeleteChargeStation(Guid chargeStationIdentifier);
    }

    public class Repository: IRepository
    {
        private readonly IGroupDatabaseContext _groupDatabaseContext;
        private readonly IChargeStationDatabaseContext _chargeStationDatabaseContext;
        private readonly IConnectorDatabaseContext _connectorDatabaseContext;

        public Repository(
            IGroupDatabaseContext groupDatabaseContext, 
            IChargeStationDatabaseContext chargeStationDatabaseContext, 
            IConnectorDatabaseContext connectorDatabaseContext)
        {
            _groupDatabaseContext = groupDatabaseContext;
            _chargeStationDatabaseContext = chargeStationDatabaseContext;
            _connectorDatabaseContext = connectorDatabaseContext;
        }

        public void Initialize()
        {
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_winsqlite3());

            _groupDatabaseContext.Initialize();
            _chargeStationDatabaseContext.Initialize();
            _connectorDatabaseContext.Initialize();
        }

        public IEnumerable<Group> GetGroups()
        {
            var groups = _groupDatabaseContext.GetAll();
            var chargeStations = _chargeStationDatabaseContext.GetAll();
            var connectors = _connectorDatabaseContext.GetAll();

            return ToDomainModel(groups, chargeStations, connectors);
        }

        public Group GetGroup(Guid identifier)
        {
            var group = _groupDatabaseContext.GetByGroupIdentifier(identifier.ToString());
            if (group == null)
            {
                return null;
            }
            var chargeStations = _chargeStationDatabaseContext.GetByGroupIdentifier(identifier.ToString());
            var connectors = _connectorDatabaseContext.GetByGroupIdentifier(identifier.ToString());

            return ToDomainModel(group, chargeStations, connectors);
        }

        public bool SaveGroup(Group group)
        {
            var dbGroup = new Models.Group
            {
                Identifier = group.Identifier.ToString(),
                Name = group.Name,
                CapacityInAmps = group.CapacityInAmps
            };
            return 0 < _groupDatabaseContext.Save(dbGroup);
        }

        public bool DeleteGroup(Guid identifier)
        {
            _connectorDatabaseContext.DeleteByGroupIdentifier(identifier.ToString());
            _chargeStationDatabaseContext.DeleteByGroupIdentifier(identifier.ToString());
            return 0 < _groupDatabaseContext.DeleteByGroupIdentifier(identifier.ToString());
        }

        public bool SaveChargeStation(ChargeStation chargeStationDomainModel)
        {
            var dbChargeStation = new Models.ChargeStation
            {
                Identifier = chargeStationDomainModel.Identifier.ToString(),
                GroupIdentifier = chargeStationDomainModel.Group.Identifier.ToString(),
                Name = chargeStationDomainModel.Name
            };

            var dbConnectors = chargeStationDomainModel.Connectors.Select(c => new Models.Connector
            {
                Identifier = c.Identifier,
                ChargeStationIdentifier = c.ChargeStation.Identifier.ToString(),
                GroupIdentifier = c.ChargeStation.Group.Identifier.ToString(),
                MaxCurrentInAmps = c.MaxCurrentInAmps
            });

            return 0 < _chargeStationDatabaseContext.Save(dbChargeStation)
                && 0 < _connectorDatabaseContext.ReplaceChargeStationConnectors(dbConnectors);

        }

        public bool DeleteChargeStation(Guid chargeStationIdentifier)
        {
            _connectorDatabaseContext.DeleteByChargeStationIdentifier(chargeStationIdentifier.ToString());
            return 0 < _chargeStationDatabaseContext.DeleteByChargeStationIdentifier(chargeStationIdentifier.ToString());
        }

        private IEnumerable<Group> ToDomainModel(
            IEnumerable<Models.Group> groups, 
            IEnumerable<Models.ChargeStation> chargeStations, 
            IEnumerable<Models.Connector> connectors)
        {
            var chargeStationsDic = chargeStations
                .GroupBy(c => c.GroupIdentifier)
                .ToDictionary(c => c.Key);

            var connectorsDic = connectors
                .GroupBy(g => g.GroupIdentifier)
                .ToDictionary(c => c.Key);

            foreach (var group in groups)
            {
                chargeStationsDic.TryGetValue(group.Identifier, out var groupChargeStations);
                connectorsDic.TryGetValue(group.Identifier, out var groupConnectors);
                yield return ToDomainModel(group, groupChargeStations, groupConnectors);
            }
        }

        private Group ToDomainModel(Models.Group group,
            IEnumerable<Models.ChargeStation> chargeStations,
            IEnumerable<Models.Connector> connectors)
        {
            var groupDomainModel = new Group(Guid.Parse(group.Identifier), group.Name, group.CapacityInAmps);

            AddChargeStations(groupDomainModel, chargeStations, connectors);

            return groupDomainModel;
        }

        private void AddChargeStations(
            Group groupDomainModel,
            IEnumerable<Models.ChargeStation> chargeStations,
            IEnumerable<Models.Connector> connectors)
        {
            if (chargeStations == null) return;

            var connectorsDic = connectors
                .GroupBy(g => g.ChargeStationIdentifier)
                .ToDictionary(c => c.Key);

            foreach (var chargeStation in chargeStations)
            {
                connectorsDic.TryGetValue(chargeStation.Identifier, out var chargeStationConnectors);
                var chargeStationDomainModel = new ChargeStation(groupDomainModel, Guid.Parse(chargeStation.Identifier), chargeStation.Name);
                groupDomainModel.AddChargeStation(chargeStationDomainModel);
                AddConnectors(chargeStationDomainModel, chargeStationConnectors);
            }
        }

        private void AddConnectors(ChargeStation chargeStationDomainModel, IEnumerable<Models.Connector> connectors)
        {
            if (connectors == null) return;

            foreach (var connector in connectors)
            {
                chargeStationDomainModel.AddConnector(new Connector(chargeStationDomainModel, connector.Identifier)
                {
                    MaxCurrentInAmps = connector.MaxCurrentInAmps
                });
            }
        }
    }
}
