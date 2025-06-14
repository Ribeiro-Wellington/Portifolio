using System;
using FluentAssertions;
using Xunit;
using CadastroClientes.Domain.Entities;
using CadastroClientes.Domain.Events;

namespace CadastroClientes.Tests.Domain
{
    public class ClienteTests
    {
        [Fact]
        public void CriarCliente_ComDadosValidos_DeveCriarClienteComEvento()
        {
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

            var cliente = new Cliente(
                nome, documento, false, dataNascimento, telefone, email,
                cep, endereco, numero, bairro, cidade, estado);

            cliente.Should().NotBeNull();
            cliente.Id.Should().NotBeEmpty();
            cliente.Nome.Should().Be(nome);
            cliente.Documento.Should().Be(documento);
            cliente.IsPessoaJuridica.Should().BeFalse();
            cliente.Telefone.Should().Be(telefone);
            cliente.Email.Should().Be(email);
            cliente.Cep.Should().Be(cep);
            cliente.Endereco.Should().Be(endereco);
            cliente.Numero.Should().Be(numero);
            cliente.Bairro.Should().Be(bairro);
            cliente.Cidade.Should().Be(cidade);
            cliente.Estado.Should().Be(estado);
            cliente.DataCadastro.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            cliente.Version.Should().Be(0);

            cliente.DomainEvents.Should().HaveCount(1);
            cliente.DomainEvents.Should().ContainSingle(e => e is ClienteCriadoEvent);
        }

        [Fact]
        public void CriarClientePessoaJuridica_ComDadosValidos_DeveCriarClienteCorretamente()
        {
            var nome = "Empresa LTDA";
            var documento = "12.345.678/0001-90";
            var inscricaoEstadual = "123.456.789.012";

            var cliente = new Cliente(
                nome, documento, true, DateTime.Now, "(11) 99999-9999", "empresa@email.com",
                "01234-567", "Rua das Flores", "123", "Centro", "São Paulo", "SP",
                inscricaoEstadual, false);

            cliente.IsPessoaJuridica.Should().BeTrue();
            cliente.InscricaoEstadual.Should().Be(inscricaoEstadual);
            cliente.Isento.Should().BeFalse();
        }

        [Fact]
        public void AtualizarCliente_ComDadosValidos_DeveAtualizarClienteEAdicionarEvento()
        {
            var cliente = new Cliente(
                "João Silva", "123.456.789-00", false, DateTime.Now, "(11) 99999-9999", "joao@email.com",
                "01234-567", "Rua das Flores", "123", "Centro", "São Paulo", "SP");

            var novoNome = "João Silva Santos";
            var novoTelefone = "(11) 88888-8888";
            var novoEmail = "joao.santos@email.com";
            var novoCep = "04567-890";
            var novoEndereco = "Av. Paulista";
            var novoNumero = "1000";
            var novoBairro = "Bela Vista";
            var novaCidade = "São Paulo";
            var novoEstado = "SP";

            cliente.Atualizar(novoNome, novoTelefone, novoEmail, novoCep, novoEndereco,
                novoNumero, novoBairro, novaCidade, novoEstado);

            cliente.Nome.Should().Be(novoNome);
            cliente.Telefone.Should().Be(novoTelefone);
            cliente.Email.Should().Be(novoEmail);
            cliente.Cep.Should().Be(novoCep);
            cliente.Endereco.Should().Be(novoEndereco);
            cliente.Numero.Should().Be(novoNumero);
            cliente.Bairro.Should().Be(novoBairro);
            cliente.Cidade.Should().Be(novaCidade);
            cliente.Estado.Should().Be(novoEstado);
            cliente.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            cliente.Version.Should().Be(1);

            cliente.DomainEvents.Should().HaveCount(2);
            cliente.DomainEvents.Should().ContainSingle(e => e is ClienteAtualizadoEvent);
        }

        [Fact]
        public void RemoverCliente_DeveAdicionarEventoDeRemocao()
        {
            var cliente = new Cliente(
                "João Silva", "123.456.789-00", false, DateTime.Now, "(11) 99999-9999", "joao@email.com",
                "01234-567", "Rua das Flores", "123", "Centro", "São Paulo", "SP");

            cliente.Remover();

            cliente.DomainEvents.Should().HaveCount(2);
            cliente.DomainEvents.Should().ContainSingle(e => e is ClienteRemovidoEvent);
        }

        [Fact]
        public void AplicarEventoClienteCriado_DeveReconstruirClienteCorretamente()
        {
            var clienteId = Guid.NewGuid();
            var evento = new ClienteCriadoEvent(
                clienteId, "João Silva", "123.456.789-00", false, DateTime.Now,
                "(11) 99999-9999", "joao@email.com", "01234-567", "Rua das Flores",
                "123", "Centro", "São Paulo", "SP");

            var cliente = new Cliente();

            cliente.AplicarEvento(evento);

            cliente.Id.Should().Be(clienteId);
            cliente.Nome.Should().Be("João Silva");
            cliente.Documento.Should().Be("123.456.789-00");
            cliente.Version.Should().Be(1);
        }

        [Fact]
        public void AplicarEventoClienteAtualizado_DeveAtualizarClienteCorretamente()
        {
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente();
            
            var eventoCriacao = new ClienteCriadoEvent(
                clienteId, "João Silva", "123.456.789-00", false, DateTime.Now,
                "(11) 99999-9999", "joao@email.com", "01234-567", "Rua das Flores",
                "123", "Centro", "São Paulo", "SP");
            cliente.AplicarEvento(eventoCriacao);

            var eventoAtualizacao = new ClienteAtualizadoEvent(
                clienteId, "João Silva Santos", "(11) 88888-8888", "joao.santos@email.com",
                "04567-890", "Av. Paulista", "1000", "Bela Vista", "São Paulo", "SP");

            cliente.AplicarEvento(eventoAtualizacao);

            cliente.Nome.Should().Be("João Silva Santos");
            cliente.Telefone.Should().Be("(11) 88888-8888");
            cliente.Email.Should().Be("joao.santos@email.com");
            cliente.Version.Should().Be(2);
        }

        [Fact]
        public void ClearDomainEvents_DeveLimparListaDeEventos()
        {
            var cliente = new Cliente(
                "João Silva", "123.456.789-00", false, DateTime.Now, "(11) 99999-9999", "joao@email.com",
                "01234-567", "Rua das Flores", "123", "Centro", "São Paulo", "SP");

            cliente.DomainEvents.Should().HaveCount(1);

            cliente.ClearDomainEvents();

            cliente.DomainEvents.Should().BeEmpty();
        }
    }
} 