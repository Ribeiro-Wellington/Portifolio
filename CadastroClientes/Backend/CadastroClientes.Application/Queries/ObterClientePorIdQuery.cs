using System;
using MediatR;
using CadastroClientes.Application.ViewModels;

namespace CadastroClientes.Application.Queries
{
    public class ObterClientePorIdQuery : IRequest<ClienteViewModel>
    {
        public Guid Id { get; set; }
    }
} 