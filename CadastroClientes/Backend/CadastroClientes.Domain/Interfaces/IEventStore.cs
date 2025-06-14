using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CadastroClientes.Domain.Events;

namespace CadastroClientes.Domain.Interfaces
{
    public interface IEventStore
    {
        Task SaveEventsAsync(Guid aggregateId, IEnumerable<DomainEvent> events, int expectedVersion);
        Task<IEnumerable<DomainEvent>> GetEventsAsync(Guid aggregateId);
        Task<IEnumerable<DomainEvent>> GetEventsAsync(Guid aggregateId, DateTime until);
        Task<int> GetLastVersionAsync(Guid aggregateId);
    }
} 