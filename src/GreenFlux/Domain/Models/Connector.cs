using System;
using GreenFlux.Domain.Exceptions;

namespace GreenFlux.Domain.Models
{
    public class Connector
    {
        private int _maxCurrentInAmps;
        private short _identifier;

        public Connector(ChargeStation chargeStation, short identifier)
        {
            ChargeStation = chargeStation;
            Identifier = identifier;
        }
        
        public ChargeStation ChargeStation { get; set;  }

        public int MaxCurrentInAmps
        {
            get => _maxCurrentInAmps;
            set
            {
                if (value == 0)
                {
                    throw new DomainException(nameof(MaxCurrentInAmps), $"The value of {nameof(MaxCurrentInAmps)} should not be 0");
                }

                var delta = value - _maxCurrentInAmps;
                var availableCapacity = ChargeStation.Group.GetAvailableCapacity();

                if (availableCapacity < delta)
                {
                    throw new NotEnoughCapicityException(nameof(MaxCurrentInAmps), $"There is not enough capacity in the group to set {nameof(MaxCurrentInAmps)} to {value}.", delta - availableCapacity);
                }

                _maxCurrentInAmps = value;
            }
        }

        public short Identifier
        {
            get => _identifier;
            set
            {
                if (value < Constants.MinConnectorIdentifier || Constants.MaxConnectorIdentifier < value)
                {
                    throw new DomainException(
                        nameof(Identifier),
                        $"The value of {nameof(Identifier)} must be between {Constants.MinConnectorIdentifier} and {Constants.MaxConnectorIdentifier}");
                }

                _identifier = value;
            }
        }
    }
}
