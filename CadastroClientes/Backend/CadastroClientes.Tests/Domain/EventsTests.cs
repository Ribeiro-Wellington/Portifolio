using System;
using FluentAssertions;
using Xunit;
using CadastroClientes.Domain.Events;

namespace CadastroClientes.Tests.Domain
{
    public class EventsTests
    {
        [Fact]
        public void ClienteCriadoEvent_DeveTerPropriedadesCorretas()
        {
            var aggregateId = Guid.NewGuid();
            var nome = "João Silva";
            var documento = "123.456.789-00";
            var dataNascimento = new DateTime(1990, 1, 1);
            var telefone = "(11) 99999-9999";
            var email = "joao@email.com";
            var cep = "01234-567";
            var endereco = "Rua das Flores";
            var numero = "123";
            var bairro = "Centro";
            var cidade = "São Paulo";
            var estado = "SP";

            var evento = new ClienteCriadoEvent(
                aggregateId, nome, documento, false, dataNascimento, telefone, email,
                cep, endereco, numero, bairro, cidade, estado);

            evento.Should().NotBeNull();
            evento.Id.Should().NotBeEmpty();
            evento.AggregateId.Should().Be(aggregateId);
            evento.EventType.Should().Be(nameof(ClienteCriadoEvent));
            evento.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            evento.Nome.Should().Be(nome);
            evento.Documento.Should().Be(documento);
            evento.Telefone.Should().Be(telefone);
            evento.Email.Should().Be(email);
            evento.Cep.Should().Be(cep);
            evento.Endereco.Should().Be(endereco);
            evento.Numero.Should().Be(numero);
            evento.Bairro.Should().Be(bairro);
            evento.Cidade.Should().Be(cidade);
            evento.Estado.Should().Be(estado);
        }

        [Fact]
        public void ClienteAtualizadoEvent_DeveTerPropriedadesCorretas()
        {
            var aggregateId = Guid.NewGuid();
            var nome = "João Silva Santos";
            var telefone = "(11) 88888-8888";
            var email = "joao.santos@email.com";
            var cep = "04567-890";
            var endereco = "Av. Paulista";
            var numero = "1000";
            var bairro = "Bela Vista";
            var cidade = "São Paulo";
            var estado = "SP";

            var evento = new ClienteAtualizadoEvent(
                aggregateId, nome, telefone, email, cep, endereco, numero, bairro, cidade, estado,
                "123.456.789.012", false);

            evento.Should().NotBeNull();
            evento.Id.Should().NotBeEmpty();
            evento.AggregateId.Should().Be(aggregateId);
            evento.EventType.Should().Be(nameof(ClienteAtualizadoEvent));
            evento.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            evento.Nome.Should().Be(nome);
            evento.Telefone.Should().Be(telefone);
            evento.Email.Should().Be(email);
            evento.Cep.Should().Be(cep);
            evento.Endereco.Should().Be(endereco);
            evento.Numero.Should().Be(numero);
            evento.Bairro.Should().Be(bairro);
            evento.Cidade.Should().Be(cidade);
            evento.Estado.Should().Be(estado);
        }

        [Fact]
        public void ClienteRemovidoEvent_DeveTerPropriedadesCorretas()
        {
            var aggregateId = Guid.NewGuid();
            var nome = "João Silva";
            var email = "joao@email.com";

            var evento = new ClienteRemovidoEvent(aggregateId, nome, email);

            evento.Should().NotBeNull();
            evento.Id.Should().NotBeEmpty();
            evento.AggregateId.Should().Be(aggregateId);
            evento.EventType.Should().Be(nameof(ClienteRemovidoEvent));
            evento.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            evento.Nome.Should().Be(nome);
            evento.Email.Should().Be(email);
        }

        [Fact]
        public void DomainEvent_DeveTerPropriedadesBaseCorretas()
        {
            var aggregateId = Guid.NewGuid();
            var evento = new ClienteCriadoEvent(
                aggregateId, "Teste", "123.456.789-00", false, DateTime.Now,
                "(11) 99999-9999", "teste@email.com", "01234-567", "Rua Teste",
                "123", "Centro", "São Paulo", "SP");

            evento.Should().NotBeNull();
            evento.Id.Should().NotBeEmpty();
            evento.AggregateId.Should().Be(aggregateId);
            evento.EventType.Should().Be(nameof(ClienteCriadoEvent));
            evento.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }
} 