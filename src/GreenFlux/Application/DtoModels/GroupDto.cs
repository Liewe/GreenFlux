using System;

namespace GreenFlux.Application.DtoModels
{
    public class GroupDto : LinkedDtoBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int CapacityInAmps { get; set; }
    }
}
