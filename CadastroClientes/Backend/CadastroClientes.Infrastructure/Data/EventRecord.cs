using System;

namespace CadastroClientes.Infrastructure.Data
{
    public class EventRecord
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string EventData { get; set; } = string.Empty;
        public int Version { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 