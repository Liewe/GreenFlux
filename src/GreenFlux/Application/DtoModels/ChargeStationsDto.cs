using System.Collections.Generic;

namespace GreenFlux.Application.DtoModels
{
    public class ChargeStationsDto : LinkedDtoBase
    {
        public IEnumerable<ChargeStationDto> Values { get; set; }
    }
}
