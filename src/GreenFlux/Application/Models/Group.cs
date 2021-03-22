using System;

namespace GreenFlux.Application.Models
{
    public class Group : LinkedModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int CapacityInAmps { get; set; }
    }
}
