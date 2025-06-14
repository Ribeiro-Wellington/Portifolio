using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CadastroClientes.Application.Queries;
using CadastroClientes.Infrastructure.Data;

namespace CadastroClientes.Application.Handlers
{
    public class ObterEventosQueryHandler : IRequestHandler<ObterEventosQuery, IEnumerable<EventoDto>>
    {
        private readonly ApplicationDbContext _context;

        public ObterEventosQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventoDto>> Handle(ObterEventosQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Events.AsQueryable();

            if (request.AggregateId.HasValue)
            {
                query = query.Where(e => e.AggregateId == request.AggregateId.Value);
            }

            if (!string.IsNullOrEmpty(request.EventType))
            {
                query = query.Where(e => e.EventType.Contains(request.EventType));
            }

            if (request.DataInicio.HasValue)
            {
                query = query.Where(e => e.Timestamp >= request.DataInicio.Value);
            }

            if (request.DataFim.HasValue)
            {
                query = query.Where(e => e.Timestamp <= request.DataFim.Value);
            }

            query = query.OrderByDescending(e => e.Timestamp);

            var limite = request.Limite ?? 100;
            query = query.Take(limite);

            var eventos = await query.ToListAsync(cancellationToken);

            var eventosDto = eventos.Select(e => new EventoDto
            {
                Id = e.Id,
                AggregateId = e.AggregateId,
                EventType = e.EventType,
                EventData = e.EventData,
                Version = e.Version,
                Timestamp = e.Timestamp,
                AggregateType = "Cliente"
            }).ToList();

            foreach (var evento in eventosDto)
            {
                evento.ProcessarEventData();
            }

            return eventosDto;
        }
    }
} 