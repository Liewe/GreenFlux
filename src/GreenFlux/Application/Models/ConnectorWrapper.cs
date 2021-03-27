using GreenFlux.Domain.Models;

namespace GreenFlux.Application.Models
{
    public interface IValue
    {
        public int Value { get; }
    }

    public class ConnectorWrapper : IValue
    {
        public ConnectorWrapper(ChargeStation chargeStation, short connectorId, int maxCapactiry)
        {
            ChargeStation = chargeStation;
            ConnectorId = connectorId;
            MaxCurrentInAmps = maxCapactiry;
        }

        public ChargeStation ChargeStation { get; }
        public short ConnectorId { get; }
        public int MaxCurrentInAmps { get; }

        public int Value
        {
            get => MaxCurrentInAmps;
        }
    }
}
