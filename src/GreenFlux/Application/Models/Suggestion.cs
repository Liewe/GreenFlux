using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenFlux.Application.Models
{
    public class Suggestion
    {
        public IEnumerable<Connector> Connectors { get; set; }
    }
}
