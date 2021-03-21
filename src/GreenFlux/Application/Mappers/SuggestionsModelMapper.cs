using System.Collections.Generic;
using System.Linq;
using GreenFlux.Application.Models;

namespace GreenFlux.Application.Mappers
{
    public interface ISuggestionsModelMapper
    {
        Suggestions Map(IEnumerable<IEnumerable<Domain.Models.Connector>> connectors, int capacityNeeded, bool suggestionsAreExact);
    }

    public class SuggestionsModelMapper : ISuggestionsModelMapper
    {
        private readonly ISuggestionModelMapper _suggestionModelMapper;

        public SuggestionsModelMapper(ISuggestionModelMapper suggestionModelMapper)
        {
            _suggestionModelMapper = suggestionModelMapper;
        }

        public Suggestions Map(IEnumerable<IEnumerable<Domain.Models.Connector>> connectorsSets, int capacityNeeded, bool suggestionsAreExact)
        {
            return new Suggestions
            {
                ErrorMessage = "There is not enough capacity to complete the request, here are some suggestions to free up the necessary capicity.",
                CapacityNeeded = capacityNeeded,
                SuggestionsAreExact = suggestionsAreExact,
                Suggestion = connectorsSets.Select(_suggestionModelMapper.Map)
            };
        }
    }
}
