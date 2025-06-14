using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CadastroClientes.Domain.Interfaces;
using CadastroClientes.Application.ViewModels;

namespace CadastroClientes.Application.Handlers
{
    public class ObterClientesQueryHandler : IRequestHandler<Queries.ObterClientesQuery, IEnumerable<ClienteViewModel>>
    {
        private readonly IClienteRepository _clienteRepository;

        public ObterClientesQueryHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<IEnumerable<ClienteViewModel>> Handle(Queries.ObterClientesQuery request, CancellationToken cancellationToken)
        {
            var clientes = await _clienteRepository.ObterTodosAsync();

            return clientes.Select(c => new ClienteViewModel
            {
                Id = c.Id,
                Nome = c.Nome,
                Documento = c.Documento,
                IsPessoaJuridica = c.IsPessoaJuridica,
                InscricaoEstadual = c.InscricaoEstadual,
                Isento = c.Isento,
                DataNascimento = c.DataNascimento,
                Telefone = c.Telefone,
                Email = c.Email,
                Cep = c.Cep,
                Endereco = c.Endereco,
                Numero = c.Numero,
                Bairro = c.Bairro,
                Cidade = c.Cidade,
                Estado = c.Estado,
                DataCadastro = c.DataCadastro,
                DataAtualizacao = c.DataAtualizacao
            });
        }
    }
} 