using System.Collections.Generic;

namespace GreenFlux.Application.DtoModels
{
    public class SuggestionsDto
    {
        public string ErrorMessage { get; set; }

        public int CapacityNeeded { get; set; }

        public bool SuggestionsAreExact { get; set; }

        public IEnumerable<SuggestionDto> Suggestion { get; set; }
    }
}
