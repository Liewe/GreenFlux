using System.Collections.Generic;

namespace GreenFlux.Application.Models
{
    public class ChargeStations : LinkedModel
    {
        public IEnumerable<ChargeStation> Values { get; set; }
    }
}
