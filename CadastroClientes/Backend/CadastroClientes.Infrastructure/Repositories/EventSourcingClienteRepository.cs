using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CadastroClientes.Domain.Entities;
using CadastroClientes.Domain.Interfaces;
using CadastroClientes.Domain.Events;
using CadastroClientes.Infrastructure.Data;

namespace CadastroClientes.Infrastructure.Repositories
{
    public class EventSourcingClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventStore _eventStore;

        public EventSourcingClienteRepository(ApplicationDbContext context, IEventStore eventStore)
        {
            _context = context;
            _eventStore = eventStore;
        }

        public async Task<Cliente?> ObterPorIdAsync(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                return cliente;
            }

            var eventos = await _eventStore.GetEventsAsync(id);
            if (!eventos.Any())
                return null;

            var clienteReconstruido = new Cliente();
            foreach (var evento in eventos)
            {
                clienteReconstruido.AplicarEvento(evento);
            }

            return clienteReconstruido;
        }

        public async Task<IEnumerable<Cliente>> ObterTodosAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente?> ObterPorEmailAsync(string email)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Cliente?> ObterPorDocumentoAsync(string documento)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Documento == documento);
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            return await _context.Clientes.AnyAsync(c => c.Email == email);
        }

        public async Task<bool> DocumentoExisteAsync(string documento)
        {
            return await _context.Clientes.AnyAsync(c => c.Documento == documento);
        }

        public async Task AdicionarAsync(Cliente cliente)
        {
            await _eventStore.SaveEventsAsync(cliente.Id, cliente.DomainEvents, cliente.Version);
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
            cliente.ClearDomainEvents();
        }

        public async Task AtualizarAsync(Cliente cliente)
        {
            await _eventStore.SaveEventsAsync(cliente.Id, cliente.DomainEvents, cliente.Version);
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
            cliente.ClearDomainEvents();
        }

        public async Task RemoverAsync(Cliente cliente)
        {
            await _eventStore.SaveEventsAsync(cliente.Id, cliente.DomainEvents, cliente.Version);
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            cliente.ClearDomainEvents();
        }
    }
} 