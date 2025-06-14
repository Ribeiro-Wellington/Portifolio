using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CadastroClientes.Domain.Entities;

namespace CadastroClientes.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente?> ObterPorIdAsync(Guid id);
        Task<Cliente?> ObterPorDocumentoAsync(string documento);
        Task<Cliente?> ObterPorEmailAsync(string email);
        Task<IEnumerable<Cliente>> ObterTodosAsync();
        Task AdicionarAsync(Cliente cliente);
        Task AtualizarAsync(Cliente cliente);
        Task RemoverAsync(Cliente cliente);
        Task<bool> DocumentoExisteAsync(string documento);
        Task<bool> EmailExisteAsync(string email);
    }
} 