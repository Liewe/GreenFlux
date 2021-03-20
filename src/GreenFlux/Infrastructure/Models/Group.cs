using System;

namespace GreenFlux.Infrastructure.Models
{
    public class Group
    {
        public Guid Identifier { get; set; }

        public string Name { get; set; }

        public int CapacityInAmps { get; set; }
    }
}
