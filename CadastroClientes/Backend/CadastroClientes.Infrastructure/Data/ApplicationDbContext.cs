using Microsoft.EntityFrameworkCore;
using CadastroClientes.Domain.Entities;
using CadastroClientes.Domain.Events;

namespace CadastroClientes.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<EventRecord> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<DomainEvent>();

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Documento).IsRequired().HasMaxLength(14);
                entity.Property(e => e.InscricaoEstadual).HasMaxLength(20);
                entity.Property(e => e.Telefone).IsRequired().HasMaxLength(15);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Cep).IsRequired().HasMaxLength(9);
                entity.Property(e => e.Endereco).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Numero).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Bairro).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Cidade).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(2);

                entity.HasIndex(e => e.Documento).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<EventRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EventType).HasMaxLength(100);
                entity.Property(e => e.EventData);
            });
        }
    }
} 