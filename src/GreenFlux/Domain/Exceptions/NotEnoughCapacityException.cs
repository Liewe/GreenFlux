namespace GreenFlux.Domain.Exceptions
{
    public class NotEnoughCapacityException : DomainException
    {
        public NotEnoughCapacityException(string key, string message, int capacityNeeded) : base(key, message)
        {
            CapacityNeeded = capacityNeeded;
        }
        public int CapacityNeeded { get; }
    }
}
