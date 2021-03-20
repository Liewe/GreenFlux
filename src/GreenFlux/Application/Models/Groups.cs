using System.Collections.Generic;

namespace GreenFlux.Application.Models
{
    public class Groups : LinkedModel
    {
        public IEnumerable<Group> Values { get; set; }
    }
}
