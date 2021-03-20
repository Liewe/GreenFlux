namespace GreenFlux.Application.Models
{
    public class Link
    {
        public Link(string rel, string href, string action)
        {
            Rel = rel;
            Href = href;
            Action = action;
        }

        public string Rel { get; }
        public string Href { get; }
        public string Action { get; }
    }
}
