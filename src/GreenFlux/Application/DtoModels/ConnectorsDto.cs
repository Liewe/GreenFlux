using System.Collections.Generic;

namespace GreenFlux.Application.DtoModels
{
    public class ConnectorsDto : LinkedDtoBase
    {
        public IEnumerable<ConnectorDto> Values { get; set; }
    }
}
