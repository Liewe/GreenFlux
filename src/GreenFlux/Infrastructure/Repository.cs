using System;
using System.Collections.Generic;
using System.Linq;
using GreenFlux.Domain.Models;

namespace GreenFlux.Infrastructure
{
    public interface IRepository
    {
        IEnumerable<Group> GetGroups();

        Group GetGroup(Guid identifier);

        bool SaveGroup(Group group);

        bool DeleteGroup(Guid identifier);
        
        bool SaveChargeStation(ChargeStation chargeStationDomainModel);

        bool DeleteChargeStation(Guid groupIdentifier, Guid chargeStationIdentifier);
    }

    public class Repository: IRepository
    {
        private readonly Dictionary<Guid, Group> _thisIsADatabaseReally = new Dictionary<Guid, Group>();

        public IEnumerable<Group> GetGroups()
        {
            return _thisIsADatabaseReally.Values;
        }

        public Group GetGroup(Guid identifier)
        {
            return _thisIsADatabaseReally.TryGetValue(identifier, out Group result) ? result : null;
        }

        public bool SaveGroup(Group group)
        {
            _thisIsADatabaseReally[group.Identifier] = group;
            return true;
        }

        public bool DeleteGroup(Guid identifier)
        {
            return _thisIsADatabaseReally.Remove(identifier);
        }

        public bool SaveChargeStation(ChargeStation chargeStationDomainModel)
        {
            return true;
        }

        public bool DeleteChargeStation(Guid groupIdentifier, Guid chargeStationIdentifier)
        {
            if (!_thisIsADatabaseReally.TryGetValue(groupIdentifier, out Group group))
            {
                return false;
            }

            var chargeStationToDelete = group.ChargeStations.FirstOrDefault(c => c.Identifier == chargeStationIdentifier);
            return group.RemoveChargeStation(chargeStationToDelete);
        }
    }
}
