using GreenFlux.Domain.Exceptions;

namespace GreenFlux.Domain.Models
{
    public class Connector
    {
        private int _maxCurrentInAmps;
        private short _id;

        public Connector(ChargeStation chargeStation, short id)
        {
            ChargeStation = chargeStation;
            Id = id;
        }
        
        public ChargeStation ChargeStation { get; set;  }

        public int MaxCurrentInAmps
        {
            get => _maxCurrentInAmps;
            set
            {
                if (value == 0)
                {
                    throw new DomainException(
                        nameof(MaxCurrentInAmps), 
                        $"The value of {nameof(MaxCurrentInAmps)} should not be 0");
                }

                var delta = value - _maxCurrentInAmps;
                var availableCapacity = ChargeStation.Group.GetAvailableCapacity();

                if (availableCapacity < delta)
                {
                    throw new NotEnoughCapacityException(
                        nameof(MaxCurrentInAmps), 
                        $"There is not enough capacity in the group to set {nameof(MaxCurrentInAmps)} to {value}.", delta - availableCapacity);
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
                    throw new DomainException(
                        nameof(Id),
                        $"The value of {nameof(Id)} must be between {Constants.MinConnectorId} and {Constants.MaxConnectorId}");
                }

                _id = value;
            }
        }
    }
}
