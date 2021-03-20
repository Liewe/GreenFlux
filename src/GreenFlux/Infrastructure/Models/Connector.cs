using System;

namespace GreenFlux.Infrastructure.Models
{
    public class Connector
    {
        public Guid GroupIdentifier { get; set; }

        public Guid ChargeStationIdentifier { get; set; }

        public short Identifier { get; set; }

        public int MaxCurrentInAmps { get; set; }

    }
}
