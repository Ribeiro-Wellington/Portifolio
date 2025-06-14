using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using CadastroClientes.Application.Commands;
using CadastroClientes.Application.Handlers;
using CadastroClientes.Domain.Entities;
using CadastroClientes.Domain.Interfaces;
using CadastroClientes.Domain.Events;
using FluentValidation;

namespace CadastroClientes.Tests.Application.Handlers
{
    public class CriarClienteCommandHandlerTests
    {
        private readonly Mock<IClienteRepository> _mockRepository;

        public CriarClienteCommandHandlerTests()
        {
            _mockRepository = new Mock<IClienteRepository>();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveCriarClienteERetornarId()
        {
            var command = new CriarClienteCommand
            {
                Nome = "João Silva",
                Documento = "123.456.789-00",
                IsPessoaJuridica = false,
                DataNascimento = new DateTime(1990, 1, 1),
                Telefone = "(11) 99999-9999",
                Email = "joao@email.com",
                Cep = "01234-567",
                Endereco = "Rua das Flores",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP"
            };

            _mockRepository.Setup(r => r.EmailExisteAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _mockRepository.Setup(r => r.DocumentoExisteAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            var handler = new CriarClienteCommandHandler(_mockRepository.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeEmpty();

            _mockRepository.Verify(r => r.EmailExisteAsync(command.Email), Times.Once);
            _mockRepository.Verify(r => r.DocumentoExisteAsync(command.Documento), Times.Once);
            _mockRepository.Verify(r => r.AdicionarAsync(It.IsAny<Cliente>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ComEmailJaExistente_DeveLancarValidationException()
        {
            var command = new CriarClienteCommand
            {
                Nome = "João Silva",
                Documento = "123.456.789-00",
                IsPessoaJuridica = false,
                DataNascimento = new DateTime(1990, 1, 1),
                Telefone = "(11) 99999-9999",
                Email = "joao@email.com",
                Cep = "01234-567",
                Endereco = "Rua das Flores",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP"
            };

            _mockRepository.Setup(r => r.EmailExisteAsync(command.Email))
                .ReturnsAsync(true);

            var handler = new CriarClienteCommandHandler(_mockRepository.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain("e-mail");

            _mockRepository.Verify(r => r.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ComDocumentoJaExistente_DeveLancarValidationException()
        {
            var command = new CriarClienteCommand
            {
                Nome = "João Silva",
                Documento = "123.456.789-00",
                IsPessoaJuridica = false,
                DataNascimento = new DateTime(1990, 1, 1),
                Telefone = "(11) 99999-9999",
                Email = "joao@email.com",
                Cep = "01234-567",
                Endereco = "Rua das Flores",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP"
            };

            _mockRepository.Setup(r => r.EmailExisteAsync(command.Email))
                .ReturnsAsync(false);
            _mockRepository.Setup(r => r.DocumentoExisteAsync(command.Documento))
                .ReturnsAsync(true);

            var handler = new CriarClienteCommandHandler(_mockRepository.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain("documento");

            _mockRepository.Verify(r => r.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ComPessoaJuridica_DeveCriarClienteCorretamente()
        {
            var command = new CriarClienteCommand
            {
                Nome = "Empresa LTDA",
                Documento = "40689782000132",
                IsPessoaJuridica = true,
                DataNascimento = new DateTime(2000, 1, 1),
                Telefone = "(11) 99999-9999",
                Email = "empresa@email.com",
                Cep = "01234-567",
                Endereco = "Rua das Flores",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                InscricaoEstadual = "123.456.789.012",
                Isento = false
            };

            _mockRepository.Setup(r => r.EmailExisteAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _mockRepository.Setup(r => r.DocumentoExisteAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            var handler = new CriarClienteCommandHandler(_mockRepository.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeEmpty();

            _mockRepository.Verify(r => r.AdicionarAsync(It.Is<Cliente>(c => 
                c.IsPessoaJuridica == true && 
                c.InscricaoEstadual == command.InscricaoEstadual && 
                c.Isento == command.Isento)), Times.Once);
        }

        [Fact]
        public async Task Handle_ComClienteCriado_DeveTerEventoDeCriacao()
        {
            var command = new CriarClienteCommand
            {
                Nome = "João Silva",
                Documento = "123.456.789-00",
                IsPessoaJuridica = false,
                DataNascimento = new DateTime(1990, 1, 1),
                Telefone = "(11) 99999-9999",
                Email = "joao@email.com",
                Cep = "01234-567",
                Endereco = "Rua das Flores",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP"
            };

            _mockRepository.Setup(r => r.EmailExisteAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _mockRepository.Setup(r => r.DocumentoExisteAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            var handler = new CriarClienteCommandHandler(_mockRepository.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeEmpty();

            _mockRepository.Verify(r => r.AdicionarAsync(It.Is<Cliente>(c => 
                c.DomainEvents.Count == 1 && 
                c.DomainEvents.Any(e => e is ClienteCriadoEvent))), Times.Once);
        }
    }
} 