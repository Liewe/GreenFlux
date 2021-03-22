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

        public ChargeStation(Group group, Guid id, string name)
        {
            Group = group;
            Id = id;
            Name = name;
        }

        public ChargeStation(Group group, Guid id, string name, IEnumerable<int> maxCapacitiesInAmps) :this(group, id, name)
        {
            SetAllMaxCapacityInAmps(maxCapacitiesInAmps);
            if (_connectors.Count == Constants.MinConnectorsInChargeStation)
            {
                throw new DomainException(
                    nameof(Connectors), 
                    $"At least '{Constants.MinConnectorsInChargeStation}' connectors are required.");
            }
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
                    throw new DomainException(
                        nameof(Name), 
                        $"The value of {nameof(Name)} is not allowed be null or empty");
                }
                _name = value;
            }
        }

        public IEnumerable<Connector> Connectors { get { return _connectors.Values; } }
        
        public void SetAllMaxCapacityInAmps(IEnumerable<int> maxCapacitiesInAmps)
        {
            var maxCapacitiesInAmpsArray = maxCapacitiesInAmps.ToArray();
            var delta = maxCapacitiesInAmpsArray.Sum() - GetUsedCapacity();
            var availableCapacity = Group.GetAvailableCapacity();

            if (availableCapacity < delta)
            {
                throw new NotEnoughCapacityException(nameof(Connectors), 
                    $"There is not enough capacity to set connectors to '{string.Join(", ", maxCapacitiesInAmpsArray)}'.", 
                    delta - availableCapacity);
            }

            var connectorId = Constants.MinConnectorId;
            var index = 0;

            while (connectorId <= Constants.MaxConnectorId)
            {
                int? maxCapacityInAmps = index < maxCapacitiesInAmpsArray.Length
                    ? maxCapacitiesInAmpsArray[index]
                    : default(int?);

                SetMaxCapacityInAmps(connectorId, maxCapacityInAmps);

                connectorId++;
                index++;
            }
        }

        public void SetMaxCapacityInAmps(short connectorId, int? maxCapacityInAmps)
        {
            var connector = GetConnector(connectorId);

            if (maxCapacityInAmps == null)
            {
                RemoveConnector(connectorId);
            }
            else if (connector == null)
            {
                AddConnector(new Connector(this, connectorId)
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
                throw new DomainException(
                    nameof(Connectors), 
                    $"The charge station already has the maximum of {Constants.MaxConnectorsInChargeStation} connectors.");
            }

            if (_connectors.ContainsKey(connector.Id))
            {
                throw new DomainException(
                    nameof(connector.Id), 
                    $"A connector with id {connector.Id} already exists within the charge station.");
            }

            _connectors.Add(connector.Id, connector);
        }

        public Connector GetConnector(short connectorId)
        {
            return _connectors.TryGetValue(connectorId, out Connector connector) ? connector : null;
        }

        public bool RemoveConnector(short connectorId)
        {
            if (!_connectors.ContainsKey(connectorId))
            {
                return false;
            }

            if (_connectors.Count == Constants.MinConnectorsInChargeStation)
            {
                throw new DomainException(
                    nameof(Connectors), 
                    $"The connector can not be removed, the charge station has a minimum of {Constants.MinConnectorsInChargeStation} connectors.");
            }

            return _connectors.Remove(connectorId);
        }

        public int GetUsedCapacity() => Connectors.Sum(c => c.MaxCurrentInAmps);

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
    }
}
