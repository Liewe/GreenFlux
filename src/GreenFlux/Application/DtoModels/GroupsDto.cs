using System.Collections.Generic;

namespace GreenFlux.Application.DtoModels
{
    public class GroupsDto : LinkedDtoBase
    {
        public IEnumerable<GroupDto> Values { get; set; }
    }
}
