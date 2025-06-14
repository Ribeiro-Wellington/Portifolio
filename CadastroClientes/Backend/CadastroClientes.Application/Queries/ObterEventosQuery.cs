using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using CadastroClientes.Domain.Events;

namespace CadastroClientes.Application.Queries
{
    public class ObterEventosQuery : IRequest<IEnumerable<EventoDto>>
    {
        public Guid? AggregateId { get; set; }
        public string? EventType { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int? Limite { get; set; } = 100;
    }

    public class EventoDto
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string EventData { get; set; } = string.Empty;
        public int Version { get; set; }
        public DateTime Timestamp { get; set; }
        public string AggregateType { get; set; } = "Cliente";
        
        public string Descricao { get; set; } = string.Empty;
        public Dictionary<string, object> Mudancas { get; set; } = new();
        public string Resumo { get; set; } = string.Empty;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public void ProcessarEventData()
        {
            try
            {
                switch (EventType)
                {
                    case nameof(ClienteCriadoEvent):
                        var eventoCriado = JsonSerializer.Deserialize<ClienteCriadoEvent>(EventData, _jsonOptions);
                        if (eventoCriado != null)
                        {
                            Descricao = $"Cliente '{eventoCriado.Nome}' foi criado";
                            Resumo = $"Novo cliente: {eventoCriado.Nome} ({eventoCriado.Documento})";
                            Mudancas = new Dictionary<string, object>
                            {
                                ["Nome"] = eventoCriado.Nome,
                                ["Documento"] = eventoCriado.Documento,
                                ["Email"] = eventoCriado.Email,
                                ["Telefone"] = eventoCriado.Telefone,
                                ["Tipo"] = eventoCriado.IsPessoaJuridica ? "Pessoa Jurídica" : "Pessoa Física"
                            };
                        }
                        break;

                    case nameof(ClienteAtualizadoEvent):
                        var eventoAtualizado = JsonSerializer.Deserialize<ClienteAtualizadoEvent>(EventData, _jsonOptions);
                        if (eventoAtualizado != null)
                        {
                            Descricao = $"Cliente '{eventoAtualizado.Nome}' foi atualizado";
                            Resumo = $"Atualização: {eventoAtualizado.Nome}";
                            Mudancas = new Dictionary<string, object>
                            {
                                ["Nome"] = eventoAtualizado.Nome,
                                ["Email"] = eventoAtualizado.Email,
                                ["Telefone"] = eventoAtualizado.Telefone,
                                ["Endereço"] = $"{eventoAtualizado.Endereco}, {eventoAtualizado.Numero} - {eventoAtualizado.Bairro}, {eventoAtualizado.Cidade}/{eventoAtualizado.Estado}",
                                ["CEP"] = eventoAtualizado.Cep
                            };
                        }
                        break;

                    case nameof(ClienteRemovidoEvent):
                        var eventoRemovido = JsonSerializer.Deserialize<ClienteRemovidoEvent>(EventData, _jsonOptions);
                        if (eventoRemovido != null)
                        {
                            Descricao = $"Cliente '{eventoRemovido.Nome}' foi removido";
                            Resumo = $"Remoção: {eventoRemovido.Nome} ({eventoRemovido.Email})";
                            Mudancas = new Dictionary<string, object>
                            {
                                ["Nome"] = eventoRemovido.Nome,
                                ["Email"] = eventoRemovido.Email
                            };
                        }
                        break;

                    default:
                        Descricao = $"Evento do tipo '{EventType}'";
                        Resumo = $"Evento desconhecido: {EventType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                Descricao = $"Erro ao processar evento: {ex.Message}";
                Resumo = "Dados do evento não puderam ser interpretados";
            }
        }
    }
} 