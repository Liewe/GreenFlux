using System;
using System.Collections.Generic;
using System.Linq;
using GreenFlux.Domain.Exceptions;

namespace GreenFlux.Domain.Models
{
    public class Group
    {
        private readonly List<ChargeStation> _chargeStations = new List<ChargeStation>();
        private string _name;
        private int _capacityInAmps;

        public Group(Guid identifier, string name, int capacityInAmps)
        {
            Identifier = identifier;
            Name = name;
            CapacityInAmps = capacityInAmps;
        }
      
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

        public int CapacityInAmps
        {
            get => _capacityInAmps;
            set
            {
                if (value == 0)
                {
                    throw new DomainException(nameof(CapacityInAmps), $"The value of {nameof(CapacityInAmps)} is not allowed to be 0");
                }

                var usedCapacity = GetUsedCapacity();

                if (value < usedCapacity)
                {
                    throw new DomainException(nameof(CapacityInAmps), $"Cannot set capacity to {value} the sum of the capacity of the connectors is {usedCapacity}");
                }

                _capacityInAmps = value;
            }
        }

        public int GetUsedCapacity() => ChargeStations
            .SelectMany(c => c.Connectors)
            .Sum(c => c.MaxCurrentInAmps);

        public int GetAvailableCapacity() => CapacityInAmps - GetUsedCapacity();

        public IEnumerable<ChargeStation> ChargeStations { get => _chargeStations; } 
        
        public void AddChargeStation(ChargeStation chargeStation)
        {
            _chargeStations.Add(chargeStation);
        }

        public bool RemoveChargeStation(ChargeStation chargeStationToDelete)
        {
            return _chargeStations.Remove(chargeStationToDelete);
        }
    }
}
