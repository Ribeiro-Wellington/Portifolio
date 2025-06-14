using System.Collections.Generic;
using MediatR;
using CadastroClientes.Application.ViewModels;

namespace CadastroClientes.Application.Queries
{
    public class ObterClientesQuery : IRequest<IEnumerable<ClienteViewModel>>
    {
    }
} 