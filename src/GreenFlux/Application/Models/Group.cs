using System;

namespace GreenFlux.Application.Models
{
    public class Group : LinkedModel
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public int CapacityInAmps { get; set; }
    }
}
