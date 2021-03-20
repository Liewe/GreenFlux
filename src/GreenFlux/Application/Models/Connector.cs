namespace GreenFlux.Application.Models
{
    public class Connector : LinkedModel
    {
        public short Identifier { get; set; }

        public int MaxCurrentInAmps { get; set; }
    }
}
