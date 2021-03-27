using System;
using System.Collections.Generic;
using System.Linq;
using GreenFlux.Domain.Exceptions;

namespace GreenFlux.Domain.Models
{
    public class ChargeStation
    {
        private readonly Dictionary<short, Connector> _connectors = new Dictionary<short, Connector>();
        private string _name;

        private ChargeStation(Group group, Guid id, string name)
        {
            Group = group;
            Id = id;
            Name = name;
        }

        public ChargeStation(Group group, Guid id, string name, IEnumerable<int> connectorCapacities) : this(group, id, name)
        {
            SetAllConnectorCapacities(connectorCapacities);
        }


        public ChargeStation(Group group, Guid id, string name, IReadOnlyDictionary<short, int> connectorCapacities) :this(group, id, name)
        {
            SetAllConnectorCapacities(connectorCapacities);
        }

        public Group Group { get; }

        public Guid Id { get; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new DomainException($"The value of {nameof(Name)} is not allowed be null or empty");
                }
                _name = value;
            }
        }

        public IEnumerable<(short id, int maxCurrentInAmps)> ConnectorCapacities { get { return _connectors.Select(c => (c.Key, c.Value.MaxCurrentInAmps)); } }

        public void SetAllConnectorCapacities(IEnumerable<int> connectorCapacities)
        {
            var dictionary = new Dictionary<short, int>();

            short connectorId = Constants.MinConnectorId;

            foreach (var connectorCapacity in connectorCapacities)
            {
                dictionary.Add(connectorId++, connectorCapacity);
            }

            SetAllConnectorCapacities(dictionary);
        }
        
        public void SetAllConnectorCapacities(IReadOnlyDictionary<short, int> connectorCapacities)
        {
            if (connectorCapacities.Count() < Constants.MinConnectorsInChargeStation)
            {
                throw new DomainException($"At least '{Constants.MinConnectorsInChargeStation}' connectors are required.");
            }

            if (connectorCapacities.Keys.Any(k => k < Constants.MinConnectorId || Constants.MaxConnectorId < k))
            {
                throw new DomainException($"Connector id must be between {Constants.MinConnectorId} and {Constants.MaxConnectorId}");
            }

            var delta = connectorCapacities.Sum(c => c.Value) - GetUsedCapacity();
            var availableCapacity = Group.GetAvailableCapacity();

            if (availableCapacity < delta)
            {
                throw new NotEnoughCapacityException(
                    $"Not enough capacity to set connector capacities to '{string.Join(", ", connectorCapacities)}'.", 
                    delta - availableCapacity);
            }

            for (short connectorId = Constants.MinConnectorId; connectorId <= Constants.MaxConnectorId; connectorId++)
            {
                if (connectorCapacities.TryGetValue(connectorId, out int connectorCapacity))
                {
                    SetConnectorCapacity(connectorId, connectorCapacity);
                }
                else
                {
                    RemoveConnector(connectorId);
                }
            }
        }

        public void SetConnectorCapacity(short connectorId, int maxCapacityInAmps)
        {
            var connector = GetConnector(connectorId);

            if (connector == null)
            {
                AddConnector(new Connector(this, connectorId, maxCapacityInAmps));
            }
            else
            {
                connector.MaxCurrentInAmps = maxCapacityInAmps;
            }
        }

        public short AddConnector(int maxCapacityInAmps)
        {
            var nextAvailableId = GetNextAvailableConnectorId();

            if (nextAvailableId == null)
            {
                throw new DomainException("No valid id available within the charge station");
            }
            
            SetConnectorCapacity(nextAvailableId.Value, maxCapacityInAmps);

            return nextAvailableId.Value;
        }

        public int? GetMaxCapacityInAmps(short connectorId)
        {
            return GetConnector(connectorId)?.MaxCurrentInAmps;
        }

        public bool RemoveConnector(short connectorId)
        {
            if (!_connectors.ContainsKey(connectorId))
            {
                return false;
            }

            if (_connectors.Count == Constants.MinConnectorsInChargeStation)
            {
                throw new DomainException($"The charge station has a minimum of {Constants.MinConnectorsInChargeStation} connectors.");
            }

            return _connectors.Remove(connectorId);
        }

        public short? GetNextAvailableConnectorId()
        {
            for (short i = Constants.MinConnectorId; i <= Constants.MaxConnectorId; i++)
            {
                if (!_connectors.ContainsKey(i))
                {
                    return i;
                }
            }

            return null;
        }

        public int GetUsedCapacity() => _connectors.Values.Sum(c => c.MaxCurrentInAmps);

        private Connector GetConnector(short connectorId)
        {
            return _connectors.TryGetValue(connectorId, out Connector connector) ? connector : null;
        }

        private void AddConnector(Connector connector)
        {
            if (_connectors.Count == Constants.MaxConnectorsInChargeStation)
            {
                throw new DomainException(
                    $"The charge station has a maximum of {Constants.MaxConnectorsInChargeStation} connectors.");
            }

            if (_connectors.ContainsKey(connector.Id))
            {
                throw new DomainException($"A connector with id {connector.Id} already exists within the charge station.");
            }

            _connectors.Add(connector.Id, connector);
        }
        
        private class Connector
        {
            private int _maxCurrentInAmps;
            private short _id;

            public Connector(ChargeStation chargeStation, short id, int maxCurrentInAmps)
            {
                ChargeStation = chargeStation;
                Id = id;
                MaxCurrentInAmps = maxCurrentInAmps;
            }

            public ChargeStation ChargeStation { get; set; }

            public int MaxCurrentInAmps
            {
                get => _maxCurrentInAmps;
                set
                {
                    if (value == 0)
                    {
                        throw new DomainException($"The value of {nameof(MaxCurrentInAmps)} should not be 0");
                    }

                    var delta = value - _maxCurrentInAmps;
                    var availableCapacity = ChargeStation.Group.GetAvailableCapacity();

                    if (availableCapacity < delta)
                    {
                        throw new NotEnoughCapacityException($"Not enough capacity to set connector capacity to {nameof(MaxCurrentInAmps)} to {value}.", delta - availableCapacity);
                    }

                    _maxCurrentInAmps = value;
                }
            }

            public short Id
            {
                get => _id;
                set
                {
                    if (value < Constants.MinConnectorId || Constants.MaxConnectorId < value)
                    {
                        throw new DomainException($"Connector id must be between {Constants.MinConnectorId} and {Constants.MaxConnectorId}");
                    }

                    _id = value;
                }
            }
        }
    }
}
