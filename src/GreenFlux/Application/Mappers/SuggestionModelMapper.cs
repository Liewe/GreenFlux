using System.Collections.Generic;
using System.Linq;
using GreenFlux.Application.Models;

namespace GreenFlux.Application.Mappers
{
    public interface ISuggestionModelMapper
    {
        Suggestion Map(IEnumerable<Domain.Models.Connector> connectors);
    }

    public class SuggestionModelMapper : ISuggestionModelMapper
    {
        private readonly IConnectorModelMapper _connectorModelMapper;

        public SuggestionModelMapper(IConnectorModelMapper connectorModelMapper)
        {
            _connectorModelMapper = connectorModelMapper;
        }
        
        public Suggestion Map(IEnumerable<Domain.Models.Connector> connectors)
        {
            return new Suggestion
            {
                Connectors = connectors.Select(_connectorModelMapper.Map)
            };
        }
    }
}
