using System;

namespace GreenFlux.Infrastructure.Models
{
    public class Connector
    {
        public string GroupIdentifier { get; set; }

        public string ChargeStationIdentifier { get; set; }

        public short Identifier { get; set; }

        public int MaxCurrentInAmps { get; set; }

    }
}
