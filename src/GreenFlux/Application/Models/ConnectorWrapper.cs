using GreenFlux.Domain.Models;

namespace GreenFlux.Application.Models
{
    public class ConnectorWrapper
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

        public void Deconstruct(out ChargeStation chargeStation, out short connectorId)
        {
            chargeStation = ChargeStation;
            connectorId = ConnectorId;
        }
    }
}
