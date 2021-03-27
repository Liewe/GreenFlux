using System;
using System.Collections.Generic;
using System.Linq;
using GreenFlux.Domain.Models;
using GreenFlux.Infrastructure.DbContexts;

namespace GreenFlux.Infrastructure
{
    public interface IRepository
    {
        void Initialize();

        IEnumerable<Group> GetGroups();

        Group GetGroup(Guid id);

        bool SaveGroup(Group group);

        bool DeleteGroup(Guid id);
        
        bool SaveChargeStation(ChargeStation chargeStationDomainModel);

        bool DeleteChargeStation(Guid chargeStationId);
    }

    public class Repository: IRepository
    {
        private readonly IGroupDbContext _groupDbContext;
        private readonly IChargeStationDbContext _chargeStationDbContext;
        private readonly IConnectorDbContext _connectorDbContext;

        public Repository(
            IGroupDbContext groupDbContext, 
            IChargeStationDbContext chargeStationDbContext, 
            IConnectorDbContext connectorDbContext)
        {
            _groupDbContext = groupDbContext;
            _chargeStationDbContext = chargeStationDbContext;
            _connectorDbContext = connectorDbContext;
        }

        public void Initialize()
        {
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_winsqlite3());

            _groupDbContext.Initialize();
            _chargeStationDbContext.Initialize();
            _connectorDbContext.Initialize();
        }

        public IEnumerable<Group> GetGroups()
        {
            var groups = _groupDbContext.GetAll();
            var chargeStations = _chargeStationDbContext.GetAll();
            var connectors = _connectorDbContext.GetAll();

            return ToDomainModel(groups, chargeStations, connectors);
        }

        public Group GetGroup(Guid id)
        {
            var group = _groupDbContext.GetByGroupId(id.ToString());
            if (group == null)
            {
                return null;
            }
            var chargeStations = _chargeStationDbContext.GetByGroupId(id.ToString());
            var connectors = _connectorDbContext.GetByGroupId(id.ToString());

            return ToDomainModel(group, chargeStations, connectors);
        }

        public bool SaveGroup(Group group)
        {
            var dbGroup = new Models.Group
            {
                Id = group.Id.ToString(),
                Name = group.Name,
                CapacityInAmps = group.CapacityInAmps
            };
            return 0 < _groupDbContext.Save(dbGroup);
        }

        public bool DeleteGroup(Guid id)
        {
            _connectorDbContext.DeleteByGroupId(id.ToString());
            _chargeStationDbContext.DeleteByGroupId(id.ToString());
            return 0 < _groupDbContext.DeleteByGroupId(id.ToString());
        }

        public bool SaveChargeStation(ChargeStation chargeStationDomainModel)
        {
            var dbChargeStation = new Models.ChargeStation
            {
                Id = chargeStationDomainModel.Id.ToString(),
                GroupId = chargeStationDomainModel.Group.Id.ToString(),
                Name = chargeStationDomainModel.Name
            };

            var dbConnectors = chargeStationDomainModel.ConnectorCapacities.Select(c => new Models.Connector
            {
                Id = c.id,
                ChargeStationId = chargeStationDomainModel.Id.ToString(),
                GroupId = chargeStationDomainModel.Group.Id.ToString(),
                MaxCurrentInAmps = c.maxCurrentInAmps
            });

            return 0 < _chargeStationDbContext.Save(dbChargeStation)
                && 0 < _connectorDbContext.ReplaceChargeStationConnectors(dbConnectors);

        }

        public bool DeleteChargeStation(Guid chargeStationId)
        {
            _connectorDbContext.DeleteByChargeStationId(chargeStationId.ToString());
            return 0 < _chargeStationDbContext.DeleteByChargeStationId(chargeStationId.ToString());
        }

        private IEnumerable<Group> ToDomainModel(
            IEnumerable<Models.Group> groups, 
            IEnumerable<Models.ChargeStation> chargeStations, 
            IEnumerable<Models.Connector> connectors)
        {
            var chargeStationsDic = chargeStations
                .GroupBy(c => c.GroupId)
                .ToDictionary(c => c.Key);

            var connectorsDic = connectors
                .GroupBy(g => g.GroupId)
                .ToDictionary(c => c.Key);

            foreach (var group in groups)
            {
                chargeStationsDic.TryGetValue(group.Id, out var groupChargeStations);
                connectorsDic.TryGetValue(group.Id, out var groupConnectors);
                yield return ToDomainModel(group, groupChargeStations, groupConnectors);
            }
        }

        private Group ToDomainModel(Models.Group group,
            IEnumerable<Models.ChargeStation> chargeStations,
            IEnumerable<Models.Connector> connectors)
        {
            var groupDomainModel = new Group(Guid.Parse(group.Id), group.Name, group.CapacityInAmps);

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
                .GroupBy(g => g.ChargeStationId)
                .ToDictionary(c => c.Key);

            foreach (var chargeStation in chargeStations)
            {
                connectorsDic.TryGetValue(chargeStation.Id, out var chargeStationConnectors);

                var connectionCapacities = chargeStationConnectors.ToDictionary(c => c.Id, c => c.MaxCurrentInAmps);
                var chargeStationDomainModel = new ChargeStation(groupDomainModel, Guid.Parse(chargeStation.Id), chargeStation.Name, connectionCapacities);
                groupDomainModel.AddChargeStation(chargeStationDomainModel);
            }
        }
    }
}
