using System;
using System.Collections.Generic;

namespace GreenFlux.Application.Models
{
    public class ChargeStation : LinkedModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Connector> Connectors { get; set; }
    }
}
