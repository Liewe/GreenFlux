using System;
using System.Collections.Generic;
using System.Linq;
using GreenFlux.Domain.Exceptions;

namespace GreenFlux.Domain.Models
{
    public class Group
    {
        private readonly Dictionary<Guid, ChargeStation> _chargeStations = new Dictionary<Guid, ChargeStation>();
        private string _name;
        private int _capacityInAmps;

        public Group(Guid id, string name, int capacityInAmps)
        {
            Id = id;
            Name = name;
            CapacityInAmps = capacityInAmps;
        }
      
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

        public int CapacityInAmps
        {
            get => _capacityInAmps;
            set
            {
                if (value == 0)
                {
                    throw new DomainException($"The value of {nameof(CapacityInAmps)} is not allowed to be 0");
                }

                var usedCapacity = GetUsedCapacity();

                if (value < usedCapacity)
                {
                    throw new DomainException($"Cannot set capacity to {value} the sum of the capacity of the connectors is {usedCapacity}");
                }

                _capacityInAmps = value;
            }
        }

        public int GetUsedCapacity() => ChargeStations.Sum(c => c.GetUsedCapacity());

        public int GetAvailableCapacity() => CapacityInAmps - GetUsedCapacity();

        public IEnumerable<ChargeStation> ChargeStations { get => _chargeStations.Values; } 
        
        public void AddChargeStation(ChargeStation chargeStation)
        {
            _chargeStations.Add(chargeStation.Id, chargeStation);
        }

        public bool RemoveChargeStation(Guid chargeStationId)
        {
            return _chargeStations.Remove(chargeStationId);
        }
    }
}
