namespace GreenFlux.Domain.Exceptions
{
    public class NotEnoughCapacityException : DomainException
    {
        public NotEnoughCapacityException(string message, int capacityNeeded) : base(message)
        {
            CapacityNeeded = capacityNeeded;
        }
        public int CapacityNeeded { get; }
    }
}
