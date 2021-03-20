using System.Collections.Generic;

namespace GreenFlux.Application.Models
{
    public class Connectors : LinkedModel
    {
        public IEnumerable<Connector> Values { get; set; }
    }
}
