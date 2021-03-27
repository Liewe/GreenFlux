using System.Collections.Generic;
using System.Linq;
using GreenFlux.Application.DtoModels;
using GreenFlux.Application.Models;

namespace GreenFlux.Application.Mappers
{
    public interface ISuggestionsDtoMapper
    {
        SuggestionsDto Map(IEnumerable<IEnumerable<ConnectorWrapper>> connectors, int capacityNeeded, bool suggestionsAreExact);
    }

    public class SuggestionsDtoMapper : ISuggestionsDtoMapper
    {
        private readonly ISuggestionDtoMapper _suggestionDtoMapper;

        public SuggestionsDtoMapper(ISuggestionDtoMapper suggestionDtoMapper)
        {
            _suggestionDtoMapper = suggestionDtoMapper;
        }

        public SuggestionsDto Map(IEnumerable<IEnumerable<ConnectorWrapper>> connectorsSets, int capacityNeeded, bool suggestionsAreExact)
        {
            return new SuggestionsDto
            {
                ErrorMessage = "There is not enough capacity to complete the request, here are some suggestions to free up the necessary capacity.",
                CapacityNeeded = capacityNeeded,
                SuggestionsAreExact = suggestionsAreExact,
                Suggestion = connectorsSets.Select(_suggestionDtoMapper.Map)
            };
        }
    }
}
