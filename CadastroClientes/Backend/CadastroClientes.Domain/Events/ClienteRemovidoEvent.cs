using System;
using System.Text.Json.Serialization;

namespace CadastroClientes.Domain.Events
{
    public class ClienteRemovidoEvent : DomainEvent
    {
        [JsonInclude]
        public string Nome { get; private set; }
        [JsonInclude]
        public string Email { get; private set; }

        public ClienteRemovidoEvent(Guid aggregateId, string nome, string email) : base(aggregateId)
        {
            Nome = nome;
            Email = email;
        }
    }
} 