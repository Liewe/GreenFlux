namespace GreenFlux.Application.DtoModels
{
    public class LinkDto
    {
        public LinkDto(string rel, string href, string action)
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
