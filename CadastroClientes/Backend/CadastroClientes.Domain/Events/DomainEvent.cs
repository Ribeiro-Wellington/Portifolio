using System;

namespace CadastroClientes.Domain.Events
{
    public abstract class DomainEvent
    {
        public Guid Id { get; private set; }
        public Guid AggregateId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string EventType { get; private set; }

        protected DomainEvent(Guid aggregateId)
        {
            Id = Guid.NewGuid();
            AggregateId = aggregateId;
            Timestamp = DateTime.UtcNow;
            EventType = GetType().Name;
        }
    }
} 