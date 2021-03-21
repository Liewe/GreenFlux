using System.Collections.Generic;

namespace GreenFlux.Application.Models
{
    public class Suggestions
    {
        public string ErrorMessage { get; set; }

        public int CapacityNeeded { get; set; }

        public bool SuggestionsAreExact { get; set; }

        public IEnumerable<Suggestion> Suggestion { get; set; }
    }
}
