using System;

namespace GreenFlux.Infrastructure.Models
{
    public class ChargeStation
    {
        public Guid Identifier { get; }

        public Guid GroupIdentifier { get; set; }

        public string Name { get; set; }
    }
}
