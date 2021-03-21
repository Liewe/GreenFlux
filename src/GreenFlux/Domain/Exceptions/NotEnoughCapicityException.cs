namespace GreenFlux.Domain.Exceptions
{
    public class NotEnoughCapicityException : DomainException
    {
        public NotEnoughCapicityException(string key, string message, int capacityNeeded) : base(key, message)
        {
            CapacityNeeded = capacityNeeded;
        }
        public int CapacityNeeded { get; }
    }
}
