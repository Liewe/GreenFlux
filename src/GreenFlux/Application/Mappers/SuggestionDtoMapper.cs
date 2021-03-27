using System.Collections.Generic;
using System.Linq;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Models;

namespace GreenFlux.Application.Mappers
{
    public interface ISuggestionDtoMapper
    {
        SuggestionDto Map(IEnumerable<ConnectorWrapper> connectors);
    }

    public class SuggestionDtoMapper : ISuggestionDtoMapper
    {
        private readonly IConnectorDtoMapper _connectorDtoMapper;

        public SuggestionDtoMapper(IConnectorDtoMapper connectorDtoMapper)
        {
            _connectorDtoMapper = connectorDtoMapper;
        }
        
        public SuggestionDto Map(IEnumerable<ConnectorWrapper> connectors)
        {
            return new SuggestionDto
            {
                Connectors = connectors.Select(c => _connectorDtoMapper.Map(c.ChargeStation, c.ConnectorId))
            };
        }
    }
}
