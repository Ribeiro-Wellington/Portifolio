using Microsoft.EntityFrameworkCore;
using CadastroClientes.Domain.Entities;
using CadastroClientes.Domain.Interfaces;
using CadastroClientes.Infrastructure.Data;

namespace CadastroClientes.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;

        public ClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente?> ObterPorIdAsync(Guid id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        public async Task<Cliente?> ObterPorDocumentoAsync(string documento)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Documento == documento);
        }

        public async Task<Cliente?> ObterPorEmailAsync(string email)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<IEnumerable<Cliente>> ObterTodosAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task AdicionarAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Cliente cliente)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DocumentoExisteAsync(string documento)
        {
            return await _context.Clientes.AnyAsync(c => c.Documento == documento);
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            return await _context.Clientes.AnyAsync(c => c.Email == email);
        }
    }
} 