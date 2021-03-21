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

        public ChargeStation(Group group, Guid identifier, string name)
        {
            Group = group;
            Identifier = identifier;
            Name = name;
        }

        public ChargeStation(Group group, Guid identifier, string name, IEnumerable<int> maxCapacitiesInAmps) :this(group, identifier, name)
        {
            SetAllMaxCapacityInAmps(maxCapacitiesInAmps);
            if (_connectors.Count == Constants.MinConnectorsInChargeStation)
            {
                throw new DomainException(nameof(Connectors), $"At least '{Constants.MinConnectorsInChargeStation}' connectors are required.");
            }
        }

        public Group Group { get; }

        public Guid Identifier { get; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new DomainException(nameof(Name), $"The value of {nameof(Name)} is not allowed be null or empty");
                }
                _name = value;
            }
        }

        public IEnumerable<Connector> Connectors { get { return _connectors.Values; } }
        
        public void SetAllMaxCapacityInAmps(IEnumerable<int> maxCapacitiesInAmps)
        {
            var delta = maxCapacitiesInAmps.Sum() - GetUsedCapacity();
            var availableCapacity = Group.GetAvailableCapacity();

            if (availableCapacity < delta)
            {
                throw new NotEnoughCapicityException(nameof(Connectors), $"There is not enough capacity to set connectors to '{string.Join(", ",maxCapacitiesInAmps)}'.", delta - availableCapacity);
            }

            var connectorIdentifier = Constants.MinConnectorIdentifier;
            var index = 0;
            var maxCapacitiesInAmpsArray = maxCapacitiesInAmps.ToArray();

            while (connectorIdentifier <= Constants.MaxConnectorIdentifier)
            {
                int? maxCapacityInAmps = index < maxCapacitiesInAmpsArray.Length
                    ? maxCapacitiesInAmpsArray[index]
                    : default(int?);

                SetMaxCapacityInAmps(connectorIdentifier, maxCapacityInAmps);

                connectorIdentifier++;
                index++;
            }
        }

        public void SetMaxCapacityInAmps(short connectorIdentifier, int? maxCapacityInAmps)
        {
            var connector = GetConnector(connectorIdentifier);

            if (maxCapacityInAmps == null)
            {
                RemoveConnector(connectorIdentifier);
            }
            else if (connector == null)
            {
                AddConnector(new Connector(this, connectorIdentifier)
                {
                    MaxCurrentInAmps = maxCapacityInAmps.Value
                });
            }
            else
            {
                connector.MaxCurrentInAmps = maxCapacityInAmps.Value;
            }
        }

        public void AddConnector(Connector connector)
        {
            if (_connectors.Count == Constants.MaxConnectorsInChargeStation)
            {
                throw new DomainException(nameof(Connectors), $"The charge station already has the maximum of {Constants.MaxConnectorsInChargeStation} connectors.");
            }

            if (_connectors.ContainsKey(connector.Identifier))
            {
                throw new DomainException(nameof(connector.Identifier), $"A connector with identifier {connector.Identifier} already exists within the charge station.");
            }

            _connectors.Add(connector.Identifier, connector);
        }

        public Connector GetConnector(short connectorIdentifier)
        {
            return _connectors.TryGetValue(connectorIdentifier, out Connector connector) ? connector : null;
        }

        public bool RemoveConnector(short connectorIdentifier)
        {
            if (!_connectors.ContainsKey(connectorIdentifier))
            {
                return false;
            }

            if (_connectors.Count == Constants.MinConnectorsInChargeStation)
            {
                throw new DomainException(nameof(Connectors), $"The connector can not be removed, the charge station has a minimum of {Constants.MinConnectorsInChargeStation} connectors.");
            }

            return _connectors.Remove(connectorIdentifier);
        }

        public int GetUsedCapacity() => Connectors.Sum(c => c.MaxCurrentInAmps);

        public short? GetNextAvailableConnectorIdentifier()
        {
            for (short i = Constants.MinConnectorIdentifier; i <= Constants.MaxConnectorIdentifier; i++)
            {
                if (!_connectors.ContainsKey(i))
                {
                    return i;
                }
            }

            return null;
        }
    }
}
