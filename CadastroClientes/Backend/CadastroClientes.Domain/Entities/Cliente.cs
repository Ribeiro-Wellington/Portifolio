using System;
using System.Collections.Generic;
using CadastroClientes.Domain.Events;

namespace CadastroClientes.Domain.Entities
{
    public class Cliente
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Documento { get; private set; }
        public bool IsPessoaJuridica { get; private set; }
        public string? InscricaoEstadual { get; private set; }
        public bool Isento { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public string Telefone { get; private set; }
        public string Email { get; private set; }
        public string Cep { get; private set; }
        public string Endereco { get; private set; }
        public string Numero { get; private set; }
        public string Bairro { get; private set; }
        public string Cidade { get; private set; }
        public string Estado { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }

        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        public int Version { get; private set; }

        public Cliente() 
        {
            Nome = string.Empty;
            Documento = string.Empty;
            Telefone = string.Empty;
            Email = string.Empty;
            Cep = string.Empty;
            Endereco = string.Empty;
            Numero = string.Empty;
            Bairro = string.Empty;
            Cidade = string.Empty;
            Estado = string.Empty;
        }

        public Cliente(
            string nome,
            string documento,
            bool isPessoaJuridica,
            DateTime dataNascimento,
            string telefone,
            string email,
            string cep,
            string endereco,
            string numero,
            string bairro,
            string cidade,
            string estado,
            string? inscricaoEstadual = null,
            bool isento = false)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Documento = documento;
            IsPessoaJuridica = isPessoaJuridica;
            InscricaoEstadual = inscricaoEstadual;
            Isento = isento;
            DataNascimento = dataNascimento;
            Telefone = telefone;
            Email = email;
            Cep = cep;
            Endereco = endereco;
            Numero = numero;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            DataCadastro = DateTime.UtcNow;
            Version = 0;

            AdicionarEvento(new ClienteCriadoEvent(
                Id, nome, documento, isPessoaJuridica, dataNascimento,
                telefone, email, cep, endereco, numero, bairro, cidade, estado,
                inscricaoEstadual, isento));
        }

        public void Atualizar(
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
            bool isento = false)
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
            DataAtualizacao = DateTime.UtcNow;
            Version++;

            AdicionarEvento(new ClienteAtualizadoEvent(
                Id, nome, telefone, email, cep, endereco, numero, bairro, cidade, estado,
                inscricaoEstadual, isento));
        }

        public void Remover()
        {
            AdicionarEvento(new ClienteRemovidoEvent(Id, Nome, Email));
        }

        private void AdicionarEvento(DomainEvent evento)
        {
            _domainEvents.Add(evento);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public void AplicarEvento(DomainEvent evento)
        {
            switch (evento)
            {
                case ClienteCriadoEvent e:
                    Id = e.AggregateId;
                    Nome = e.Nome;
                    Documento = e.Documento;
                    IsPessoaJuridica = e.IsPessoaJuridica;
                    InscricaoEstadual = e.InscricaoEstadual;
                    Isento = e.Isento;
                    DataNascimento = e.DataNascimento;
                    Telefone = e.Telefone;
                    Email = e.Email;
                    Cep = e.Cep;
                    Endereco = e.Endereco;
                    Numero = e.Numero;
                    Bairro = e.Bairro;
                    Cidade = e.Cidade;
                    Estado = e.Estado;
                    DataCadastro = e.Timestamp;
                    Version = 1;
                    break;

                case ClienteAtualizadoEvent e:
                    Nome = e.Nome;
                    Telefone = e.Telefone;
                    Email = e.Email;
                    Cep = e.Cep;
                    Endereco = e.Endereco;
                    Numero = e.Numero;
                    Bairro = e.Bairro;
                    Cidade = e.Cidade;
                    Estado = e.Estado;
                    InscricaoEstadual = e.InscricaoEstadual;
                    Isento = e.Isento;
                    DataAtualizacao = e.Timestamp;
                    Version++;
                    break;

                case ClienteRemovidoEvent e:
                    DataAtualizacao = e.Timestamp;
                    Version++;
                    break;
            }
        }
    }
} 