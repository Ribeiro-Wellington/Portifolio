using System;
using MediatR;

namespace CadastroClientes.Application.Commands
{
    public class CriarClienteCommand : IRequest<Guid>
    {
        public required string Nome { get; set; }
        public required string Documento { get; set; }
        public bool IsPessoaJuridica { get; set; }
        public string? InscricaoEstadual { get; set; }
        public bool Isento { get; set; }
        public DateTime DataNascimento { get; set; }
        public required string Telefone { get; set; }
        public required string Email { get; set; }
        public required string Cep { get; set; }
        public required string Endereco { get; set; }
        public required string Numero { get; set; }
        public required string Bairro { get; set; }
        public required string Cidade { get; set; }
        public required string Estado { get; set; }
    }
} 