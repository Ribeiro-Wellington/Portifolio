using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using CadastroClientes.Application.Queries;

namespace CadastroClientes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Consulta eventos do Event Sourcing
        /// </summary>
        /// <param name="aggregateId">ID do agregado (cliente) para filtrar</param>
        /// <param name="eventType">Tipo do evento para filtrar (ex: ClienteCriadoEvent)</param>
        /// <param name="dataInicio">Data de início para filtrar</param>
        /// <param name="dataFim">Data de fim para filtrar</param>
        /// <param name="limite">Limite de registros retornados (padrão: 100)</param>
        /// <returns>Lista de eventos</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoDto>>> ObterEventos(
            [FromQuery] Guid? aggregateId = null,
            [FromQuery] string? eventType = null,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null,
            [FromQuery] int? limite = 100)
        {
            var query = new ObterEventosQuery
            {
                AggregateId = aggregateId,
                EventType = eventType,
                DataInicio = dataInicio,
                DataFim = dataFim,
                Limite = limite
            };

            var eventos = await _mediator.Send(query);
            return Ok(eventos);
        }

        /// <summary>
        /// Consulta eventos de um cliente específico
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <returns>Lista de eventos do cliente</returns>
        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<EventoDto>>> ObterEventosPorCliente(Guid clienteId)
        {
            var query = new ObterEventosQuery
            {
                AggregateId = clienteId,
                Limite = 1000 // Mais eventos para um cliente específico
            };

            var eventos = await _mediator.Send(query);
            return Ok(eventos);
        }

        /// <summary>
        /// Consulta histórico detalhado de mudanças de um cliente
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <returns>Histórico detalhado das mudanças</returns>
        [HttpGet("cliente/{clienteId}/historico")]
        public async Task<ActionResult<object>> ObterHistoricoCliente(Guid clienteId)
        {
            var query = new ObterEventosQuery
            {
                AggregateId = clienteId,
                Limite = 1000
            };

            var eventos = await _mediator.Send(query);
            var eventosList = eventos.OrderBy(e => e.Timestamp).ToList();

            if (!eventosList.Any())
            {
                return NotFound($"Nenhum evento encontrado para o cliente {clienteId}");
            }

            var historico = new
            {
                ClienteId = clienteId,
                TotalEventos = eventosList.Count,
                PrimeiroEvento = eventosList.First(),
                UltimoEvento = eventosList.Last(),
                Timeline = eventosList.Select((e, index) => new
                {
                    Ordem = index + 1,
                    Versao = e.Version,
                    DataHora = e.Timestamp,
                    Tipo = e.EventType,
                    Descricao = e.Descricao,
                    Resumo = e.Resumo,
                    Mudancas = e.Mudancas,
                    DadosCompletos = e.EventData
                }).ToList()
            };

            return Ok(historico);
        }

        /// <summary>
        /// Consulta estatísticas dos eventos
        /// </summary>
        /// <returns>Estatísticas dos eventos</returns>
        [HttpGet("estatisticas")]
        public async Task<ActionResult<object>> ObterEstatisticas()
        {
            var query = new ObterEventosQuery
            {
                Limite = 10000 // Buscar mais eventos para estatísticas
            };

            var eventos = await _mediator.Send(query);
            var eventosList = eventos.ToList();

            var estatisticas = new
            {
                TotalEventos = eventosList.Count,
                TiposEventos = eventosList.GroupBy(e => e.EventType)
                    .Select(g => new { Tipo = g.Key, Quantidade = g.Count() })
                    .OrderByDescending(x => x.Quantidade),
                ClientesUnicos = eventosList.Select(e => e.AggregateId).Distinct().Count(),
                EventoMaisRecente = eventosList.OrderByDescending(e => e.Timestamp).FirstOrDefault(),
                EventoMaisAntigo = eventosList.OrderBy(e => e.Timestamp).FirstOrDefault()
            };

            return Ok(estatisticas);
        }
    }
} 