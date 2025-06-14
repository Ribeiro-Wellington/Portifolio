using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using CadastroClientes.Application.Queries;
using CadastroClientes.Application.Handlers;
using CadastroClientes.Infrastructure.Data;
using CadastroClientes.Domain.Events;

namespace CadastroClientes.Tests.Application.Handlers
{
    public class ObterEventosQueryHandlerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public ObterEventosQueryHandlerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Handle_ComEventosExistentes_DeveRetornarEventosProcessados()
        {
            using var context = new ApplicationDbContext(_options);
            var aggregateId = Guid.NewGuid();

            var eventoCriado = new EventRecord
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                EventType = nameof(ClienteCriadoEvent),
                EventData = "{\"nome\":\"João Silva\",\"documento\":\"123.456.789-00\",\"email\":\"joao@email.com\",\"telefone\":\"(11) 99999-9999\",\"cep\":\"01234-567\",\"endereco\":\"Rua das Flores\",\"numero\":\"123\",\"bairro\":\"Centro\",\"cidade\":\"São Paulo\",\"estado\":\"SP\",\"isPessoaJuridica\":false,\"dataNascimento\":\"1990-01-01T00:00:00\",\"inscricaoEstadual\":null,\"isento\":false,\"id\":\"" + Guid.NewGuid() + "\",\"aggregateId\":\"" + aggregateId + "\",\"timestamp\":\"2024-01-15T10:30:00Z\",\"eventType\":\"ClienteCriadoEvent\"}",
                Version = 1,
                Timestamp = DateTime.UtcNow.AddHours(-1)
            };

            var eventoAtualizado = new EventRecord
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                EventType = nameof(ClienteAtualizadoEvent),
                EventData = "{\"nome\":\"João Silva Santos\",\"telefone\":\"(11) 88888-8888\",\"email\":\"joao.santos@email.com\",\"cep\":\"04567-890\",\"endereco\":\"Av. Paulista\",\"numero\":\"1000\",\"bairro\":\"Bela Vista\",\"cidade\":\"São Paulo\",\"estado\":\"SP\",\"inscricaoEstadual\":null,\"isento\":false,\"id\":\"" + Guid.NewGuid() + "\",\"aggregateId\":\"" + aggregateId + "\",\"timestamp\":\"2024-01-15T11:30:00Z\",\"eventType\":\"ClienteAtualizadoEvent\"}",
                Version = 2,
                Timestamp = DateTime.UtcNow
            };

            context.Events.AddRange(eventoCriado, eventoAtualizado);
            await context.SaveChangesAsync();

            var handler = new ObterEventosQueryHandler(context);

            var query = new ObterEventosQuery { Limite = 10 };
            var result = await handler.Handle(query, CancellationToken.None);

            var eventos = result.ToList();
            eventos.Should().HaveCount(2);
            eventos.Should().BeInDescendingOrder(e => e.Timestamp);

            var eventoRecente = eventos.First();
            eventoRecente.EventType.Should().Be(nameof(ClienteAtualizadoEvent));
            eventoRecente.Descricao.Should().Be("Cliente 'João Silva Santos' foi atualizado");
            eventoRecente.Resumo.Should().Be("Atualização: João Silva Santos");
            eventoRecente.Mudancas.Should().ContainKey("Nome");
            eventoRecente.Mudancas["Nome"].Should().Be("João Silva Santos");

            var eventoAntigo = eventos.Last();
            eventoAntigo.EventType.Should().Be(nameof(ClienteCriadoEvent));
            eventoAntigo.Descricao.Should().Be("Cliente 'João Silva' foi criado");
            eventoAntigo.Resumo.Should().Be("Novo cliente: João Silva (123.456.789-00)");
        }

        [Fact]
        public async Task Handle_ComFiltroPorAggregateId_DeveRetornarApenasEventosDoCliente()
        {
            using var context = new ApplicationDbContext(_options);
            var aggregateId1 = Guid.NewGuid();
            var aggregateId2 = Guid.NewGuid();

            var eventoCliente1 = new EventRecord
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId1,
                EventType = nameof(ClienteCriadoEvent),
                EventData = "{\"nome\":\"João Silva\",\"documento\":\"123.456.789-00\",\"email\":\"joao@email.com\",\"telefone\":\"(11) 99999-9999\",\"cep\":\"01234-567\",\"endereco\":\"Rua das Flores\",\"numero\":\"123\",\"bairro\":\"Centro\",\"cidade\":\"São Paulo\",\"estado\":\"SP\",\"isPessoaJuridica\":false,\"dataNascimento\":\"1990-01-01T00:00:00\",\"inscricaoEstadual\":null,\"isento\":false,\"id\":\"" + Guid.NewGuid() + "\",\"aggregateId\":\"" + aggregateId1 + "\",\"timestamp\":\"2024-01-15T10:30:00Z\",\"eventType\":\"ClienteCriadoEvent\"}",
                Version = 1,
                Timestamp = DateTime.UtcNow
            };

            var eventoCliente2 = new EventRecord
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId2,
                EventType = nameof(ClienteCriadoEvent),
                EventData = "{\"nome\":\"Maria Silva\",\"documento\":\"987.654.321-00\",\"email\":\"maria@email.com\",\"telefone\":\"(11) 88888-8888\",\"cep\":\"04567-890\",\"endereco\":\"Av. Paulista\",\"numero\":\"1000\",\"bairro\":\"Bela Vista\",\"cidade\":\"São Paulo\",\"estado\":\"SP\",\"isPessoaJuridica\":false,\"dataNascimento\":\"1995-01-01T00:00:00\",\"inscricaoEstadual\":null,\"isento\":false,\"id\":\"" + Guid.NewGuid() + "\",\"aggregateId\":\"" + aggregateId2 + "\",\"timestamp\":\"2024-01-15T11:30:00Z\",\"eventType\":\"ClienteCriadoEvent\"}",
                Version = 1,
                Timestamp = DateTime.UtcNow
            };

            context.Events.AddRange(eventoCliente1, eventoCliente2);
            await context.SaveChangesAsync();

            var handler = new ObterEventosQueryHandler(context);

            var query = new ObterEventosQuery { AggregateId = aggregateId1 };
            var result = await handler.Handle(query, CancellationToken.None);

            var eventos = result.ToList();
            eventos.Should().HaveCount(1);
            eventos.First().AggregateId.Should().Be(aggregateId1);
        }

        [Fact]
        public async Task Handle_ComFiltroPorTipoEvento_DeveRetornarApenasEventosDoTipo()
        {
            using var context = new ApplicationDbContext(_options);
            var aggregateId = Guid.NewGuid();

            var eventoCriado = new EventRecord
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                EventType = nameof(ClienteCriadoEvent),
                EventData = "{\"nome\":\"João Silva\",\"documento\":\"123.456.789-00\",\"email\":\"joao@email.com\",\"telefone\":\"(11) 99999-9999\",\"cep\":\"01234-567\",\"endereco\":\"Rua das Flores\",\"numero\":\"123\",\"bairro\":\"Centro\",\"cidade\":\"São Paulo\",\"estado\":\"SP\",\"isPessoaJuridica\":false,\"dataNascimento\":\"1990-01-01T00:00:00\",\"inscricaoEstadual\":null,\"isento\":false,\"id\":\"" + Guid.NewGuid() + "\",\"aggregateId\":\"" + aggregateId + "\",\"timestamp\":\"2024-01-15T10:30:00Z\",\"eventType\":\"ClienteCriadoEvent\"}",
                Version = 1,
                Timestamp = DateTime.UtcNow.AddHours(-1)
            };

            var eventoAtualizado = new EventRecord
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                EventType = nameof(ClienteAtualizadoEvent),
                EventData = "{\"nome\":\"João Silva Santos\",\"telefone\":\"(11) 88888-8888\",\"email\":\"joao.santos@email.com\",\"cep\":\"04567-890\",\"endereco\":\"Av. Paulista\",\"numero\":\"1000\",\"bairro\":\"Bela Vista\",\"cidade\":\"São Paulo\",\"estado\":\"SP\",\"inscricaoEstadual\":null,\"isento\":false,\"id\":\"" + Guid.NewGuid() + "\",\"aggregateId\":\"" + aggregateId + "\",\"timestamp\":\"2024-01-15T11:30:00Z\",\"eventType\":\"ClienteAtualizadoEvent\"}",
                Version = 2,
                Timestamp = DateTime.UtcNow
            };

            context.Events.AddRange(eventoCriado, eventoAtualizado);
            await context.SaveChangesAsync();

            var handler = new ObterEventosQueryHandler(context);

            var query = new ObterEventosQuery { EventType = "Criado" };
            var result = await handler.Handle(query, CancellationToken.None);

            var eventos = result.ToList();
            eventos.Should().HaveCount(1);
            eventos.First().EventType.Should().Be(nameof(ClienteCriadoEvent));
        }

        [Fact]
        public async Task Handle_ComLimiteDefinido_DeveRespeitarOLimite()
        {
            using var context = new ApplicationDbContext(_options);
            var aggregateId = Guid.NewGuid();

            var eventos = new List<EventRecord>();
            for (int i = 1; i <= 5; i++)
            {
                eventos.Add(new EventRecord
                {
                    Id = Guid.NewGuid(),
                    AggregateId = aggregateId,
                    EventType = nameof(ClienteCriadoEvent),
                    EventData = "{\"nome\":\"Cliente " + i + "\",\"documento\":\"123.456.789-0" + i + "\",\"email\":\"cliente" + i + "@email.com\",\"telefone\":\"(11) 99999-999" + i + "\",\"cep\":\"01234-56" + i + "\",\"endereco\":\"Rua " + i + "\",\"numero\":\"" + i + "\",\"bairro\":\"Bairro " + i + "\",\"cidade\":\"Cidade " + i + "\",\"estado\":\"SP\",\"isPessoaJuridica\":false,\"dataNascimento\":\"1990-01-01T00:00:00\",\"inscricaoEstadual\":null,\"isento\":false,\"id\":\"" + Guid.NewGuid() + "\",\"aggregateId\":\"" + aggregateId + "\",\"timestamp\":\"2024-01-15T" + (10 + i) + ":30:00Z\",\"eventType\":\"ClienteCriadoEvent\"}",
                    Version = i,
                    Timestamp = DateTime.UtcNow.AddHours(i)
                });
            }

            context.Events.AddRange(eventos);
            await context.SaveChangesAsync();

            var handler = new ObterEventosQueryHandler(context);

            var query = new ObterEventosQuery { Limite = 3 };
            var result = await handler.Handle(query, CancellationToken.None);

            var eventosResult = result.ToList();
            eventosResult.Should().HaveCount(3);
            eventosResult.Should().BeInDescendingOrder(e => e.Timestamp);
        }

        [Fact]
        public async Task Handle_ComEventDataInvalido_DeveProcessarSemErro()
        {
            using var context = new ApplicationDbContext(_options);
            var aggregateId = Guid.NewGuid();

            var eventoInvalido = new EventRecord
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                EventType = nameof(ClienteCriadoEvent),
                EventData = "{invalid json}",
                Version = 1,
                Timestamp = DateTime.UtcNow
            };

            context.Events.Add(eventoInvalido);
            await context.SaveChangesAsync();

            var handler = new ObterEventosQueryHandler(context);

            var query = new ObterEventosQuery();
            var result = await handler.Handle(query, CancellationToken.None);

            var eventos = result.ToList();
            eventos.Should().HaveCount(1);
            eventos.First().Descricao.Should().Contain("Erro ao processar evento");
        }
    }
} 