using System.Threading;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;
using CadastroClientes.Domain.Interfaces;
using CadastroClientes.Domain.Validations;

namespace CadastroClientes.Application.Handlers
{
    public class AtualizarClienteCommandHandler : IRequestHandler<Commands.AtualizarClienteCommand>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ClienteValidator _validator;

        public AtualizarClienteCommandHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
            _validator = new ClienteValidator();
        }

        public async Task Handle(Commands.AtualizarClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(request.Id);
            if (cliente == null)
                throw new ValidationException("Cliente não encontrado.");

            var clienteComMesmoEmail = await _clienteRepository.ObterPorEmailAsync(request.Email);
            if (clienteComMesmoEmail != null && clienteComMesmoEmail.Id != request.Id)
                throw new ValidationException("Já existe um cliente cadastrado com este e-mail.");

            cliente.Atualizar(
                request.Nome,
                request.Telefone,
                request.Email,
                request.Cep,
                request.Endereco,
                request.Numero,
                request.Bairro,
                request.Cidade,
                request.Estado,
                request.InscricaoEstadual,
                request.Isento
            );

            await _validator.ValidateAndThrowAsync(cliente, cancellationToken);
            await _clienteRepository.AtualizarAsync(cliente);
        }
    }
} 