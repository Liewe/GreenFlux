using System;
using System.Collections.Generic;

namespace GreenFlux.Application.DtoModels
{
    public class ChargeStationDto : LinkedDtoBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<ConnectorDto> Connectors { get; set; }
    }
}
