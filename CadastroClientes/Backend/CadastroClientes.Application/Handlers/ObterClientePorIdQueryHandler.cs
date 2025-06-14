using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CadastroClientes.Domain.Interfaces;
using CadastroClientes.Application.ViewModels;

namespace CadastroClientes.Application.Handlers
{
    public class ObterClientePorIdQueryHandler : IRequestHandler<Queries.ObterClientePorIdQuery, ClienteViewModel>
    {
        private readonly IClienteRepository _clienteRepository;

        public ObterClientePorIdQueryHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<ClienteViewModel> Handle(Queries.ObterClientePorIdQuery request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(request.Id);
            if (cliente == null)
                return null;

            return new ClienteViewModel
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Documento = cliente.Documento,
                IsPessoaJuridica = cliente.IsPessoaJuridica,
                InscricaoEstadual = cliente.InscricaoEstadual,
                Isento = cliente.Isento,
                DataNascimento = cliente.DataNascimento,
                Telefone = cliente.Telefone,
                Email = cliente.Email,
                Cep = cliente.Cep,
                Endereco = cliente.Endereco,
                Numero = cliente.Numero,
                Bairro = cliente.Bairro,
                Cidade = cliente.Cidade,
                Estado = cliente.Estado,
                DataCadastro = cliente.DataCadastro,
                DataAtualizacao = cliente.DataAtualizacao
            };
        }
    }
} 