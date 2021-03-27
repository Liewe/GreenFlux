using System.Collections.Generic;

namespace GreenFlux.Application.DtoModels
{
    public class SuggestionDto
    {
        public IEnumerable<ConnectorDto> Connectors { get; set; }
    }
}
