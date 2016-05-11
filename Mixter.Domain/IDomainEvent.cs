namespace Mixter.Domain
{
    [Event]
    public interface IDomainEvent
    {
        object GetAggregateId();
    }
}