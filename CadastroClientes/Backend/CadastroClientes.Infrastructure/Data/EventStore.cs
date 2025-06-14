using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CadastroClientes.Domain.Events;
using CadastroClientes.Domain.Interfaces;

namespace CadastroClientes.Infrastructure.Data
{
    public class EventStore : IEventStore
    {
        private readonly ApplicationDbContext _context;
        private readonly JsonSerializerOptions _jsonOptions;

        public EventStore(ApplicationDbContext context)
        {
            _context = context;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                IncludeFields = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            };
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<DomainEvent> events, int expectedVersion)
        {
            var eventList = events.ToList();
            var lastVersion = await GetLastVersionAsync(aggregateId);

            if (lastVersion != expectedVersion)
            {
                throw new ConcurrencyException($"Expected version {expectedVersion}, but found {lastVersion}");
            }

            var eventRecords = eventList.Select((e, index) => new EventRecord
            {
                Id = e.Id,
                AggregateId = e.AggregateId,
                EventType = e.EventType,
                EventData = JsonSerializer.Serialize(e, e.GetType(), _jsonOptions),
                Version = expectedVersion + index + 1,
                Timestamp = e.Timestamp
            });

            _context.Events.AddRange(eventRecords);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventRecords = await _context.Events
                .Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.Version)
                .ToListAsync();

            return eventRecords.Select(DeserializeEvent).Where(e => e != null)!;
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsAsync(Guid aggregateId, DateTime until)
        {
            var eventRecords = await _context.Events
                .Where(e => e.AggregateId == aggregateId && e.Timestamp <= until)
                .OrderBy(e => e.Version)
                .ToListAsync();

            return eventRecords.Select(DeserializeEvent).Where(e => e != null)!;
        }

        public async Task<int> GetLastVersionAsync(Guid aggregateId)
        {
            var lastEvent = await _context.Events
                .Where(e => e.AggregateId == aggregateId)
                .OrderByDescending(e => e.Version)
                .FirstOrDefaultAsync();

            return lastEvent?.Version ?? 0;
        }

        private DomainEvent? DeserializeEvent(EventRecord record)
        {
            return record.EventType switch
            {
                nameof(ClienteCriadoEvent) => JsonSerializer.Deserialize<ClienteCriadoEvent>(record.EventData, _jsonOptions),
                nameof(ClienteAtualizadoEvent) => JsonSerializer.Deserialize<ClienteAtualizadoEvent>(record.EventData, _jsonOptions),
                nameof(ClienteRemovidoEvent) => JsonSerializer.Deserialize<ClienteRemovidoEvent>(record.EventData, _jsonOptions),
                _ => throw new InvalidOperationException($"Unknown event type: {record.EventType}")
            };
        }
    }

    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message) : base(message) { }
    }
} 