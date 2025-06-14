using System;
using MediatR;

namespace CadastroClientes.Application.Commands
{
    public class RemoverClienteCommand : IRequest
    {
        public Guid Id { get; set; }
    }
} 