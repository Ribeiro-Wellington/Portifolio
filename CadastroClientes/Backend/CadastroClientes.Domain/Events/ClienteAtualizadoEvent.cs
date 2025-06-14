using System;
using System.Text.Json.Serialization;

namespace CadastroClientes.Domain.Events
{
    public class ClienteAtualizadoEvent : DomainEvent
    {
        [JsonInclude]
        public string Nome { get; private set; }
        [JsonInclude]
        public string Telefone { get; private set; }
        [JsonInclude]
        public string Email { get; private set; }
        [JsonInclude]
        public string Cep { get; private set; }
        [JsonInclude]
        public string Endereco { get; private set; }
        [JsonInclude]
        public string Numero { get; private set; }
        [JsonInclude]
        public string Bairro { get; private set; }
        [JsonInclude]
        public string Cidade { get; private set; }
        [JsonInclude]
        public string Estado { get; private set; }
        [JsonInclude]
        public string? InscricaoEstadual { get; private set; }
        [JsonInclude]
        public bool Isento { get; private set; }

        public ClienteAtualizadoEvent(
            Guid aggregateId,
            string nome,
            string telefone,
            string email,
            string cep,
            string endereco,
            string numero,
            string bairro,
            string cidade,
            string estado,
            string? inscricaoEstadual = null,
            bool isento = false) : base(aggregateId)
        {
            Nome = nome;
            Telefone = telefone;
            Email = email;
            Cep = cep;
            Endereco = endereco;
            Numero = numero;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            InscricaoEstadual = inscricaoEstadual;
            Isento = isento;
        }
    }
} 