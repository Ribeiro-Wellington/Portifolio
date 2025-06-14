using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;
using CadastroClientes.Domain.Entities;
using CadastroClientes.Domain.Interfaces;
using CadastroClientes.Domain.Validations;

namespace CadastroClientes.Application.Handlers
{
    public class CriarClienteCommandHandler : IRequestHandler<Commands.CriarClienteCommand, Guid>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ClienteValidator _validator;

        public CriarClienteCommandHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
            _validator = new ClienteValidator();
        }

        public async Task<Guid> Handle(Commands.CriarClienteCommand request, CancellationToken cancellationToken)
        {
            if (await _clienteRepository.DocumentoExisteAsync(request.Documento))
                throw new ValidationException("Já existe um cliente cadastrado com este documento.");

            if (await _clienteRepository.EmailExisteAsync(request.Email))
                throw new ValidationException("Já existe um cliente cadastrado com este e-mail.");

            var cliente = new Cliente(
                request.Nome,
                request.Documento,
                request.IsPessoaJuridica,
                request.DataNascimento,
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
            await _clienteRepository.AdicionarAsync(cliente);

            return cliente.Id;
        }
    }
} 