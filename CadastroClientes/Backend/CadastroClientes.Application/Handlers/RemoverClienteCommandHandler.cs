using System.Threading;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;
using CadastroClientes.Domain.Interfaces;

namespace CadastroClientes.Application.Handlers
{
    public class RemoverClienteCommandHandler : IRequestHandler<Commands.RemoverClienteCommand>
    {
        private readonly IClienteRepository _clienteRepository;

        public RemoverClienteCommandHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task Handle(Commands.RemoverClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(request.Id);
            if (cliente == null)
                throw new ValidationException("Cliente não encontrado.");

            await _clienteRepository.RemoverAsync(cliente);
        }
    }
} 