using System;
using FluentValidation;
using CadastroClientes.Domain.Entities;

namespace CadastroClientes.Domain.Validations
{
    public class ClienteValidator : AbstractValidator<Cliente>
    {
        public ClienteValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

            RuleFor(x => x.Documento)
                .NotEmpty().WithMessage("Documento é obrigatório")
                .MaximumLength(14).WithMessage("Documento deve ter no máximo 14 caracteres")
                .Must(ValidarDocumento).WithMessage("Documento inválido");

            RuleFor(x => x.Telefone)
                .NotEmpty().WithMessage("Telefone é obrigatório")
                .MaximumLength(15).WithMessage("Telefone deve ter no máximo 15 caracteres")
                .Matches(@"^\(\d{2}\) \d{4,5}-\d{4}$").WithMessage("Telefone deve estar no formato (99) 99999-9999");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-mail é obrigatório")
                .EmailAddress().WithMessage("E-mail deve estar em formato válido")
                .MaximumLength(100).WithMessage("E-mail deve ter no máximo 100 caracteres");

            RuleFor(x => x.Cep)
                .NotEmpty().WithMessage("CEP é obrigatório")
                .Matches(@"^\d{5}-\d{3}$").WithMessage("CEP deve estar no formato 99999-999");

            RuleFor(x => x.Endereco)
                .NotEmpty().WithMessage("Endereço é obrigatório")
                .MaximumLength(200).WithMessage("Endereço deve ter no máximo 200 caracteres");

            RuleFor(x => x.Numero)
                .NotEmpty().WithMessage("Número é obrigatório")
                .MaximumLength(10).WithMessage("Número deve ter no máximo 10 caracteres");

            RuleFor(x => x.Bairro)
                .NotEmpty().WithMessage("Bairro é obrigatório")
                .MaximumLength(100).WithMessage("Bairro deve ter no máximo 100 caracteres");

            RuleFor(x => x.Cidade)
                .NotEmpty().WithMessage("Cidade é obrigatória")
                .MaximumLength(100).WithMessage("Cidade deve ter no máximo 100 caracteres");

            RuleFor(x => x.Estado)
                .NotEmpty().WithMessage("Estado é obrigatório")
                .Length(2).WithMessage("Estado deve ter exatamente 2 caracteres");

            When(x => x.IsPessoaJuridica, () =>
            {
                RuleFor(x => x.InscricaoEstadual)
                    .MaximumLength(20).WithMessage("Inscrição Estadual deve ter no máximo 20 caracteres");
            });
        }

        private bool ValidarDocumento(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return false;

            var documentoLimpo = documento.Replace(".", "").Replace("-", "").Replace("/", "");

            if (documentoLimpo.Length == 11)
            {
                return ValidarCPF(documentoLimpo);
            }
            else if (documentoLimpo.Length == 14)
            {
                return ValidarCNPJ(documentoLimpo);
            }

            return false;
        }

        private bool ValidarCPF(string cpf)
        {
            if (cpf.Length != 11 || cpf.All(c => c == cpf[0]))
                return false;

            var soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * (10 - i);

            var resto = soma % 11;
            var digito1 = resto < 2 ? 0 : 11 - resto;

            if (cpf[9] - '0' != digito1)
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * (11 - i);

            resto = soma % 11;
            var digito2 = resto < 2 ? 0 : 11 - resto;

            return cpf[10] - '0' == digito2;
        }

        private bool ValidarCNPJ(string cnpj)
        {
            if (cnpj.Length != 14 || cnpj.All(c => c == cnpj[0]))
                return false;

            var soma = 0;
            var peso = 2;
            for (int i = 11; i >= 0; i--)
            {
                soma += (cnpj[i] - '0') * peso;
                peso = peso == 9 ? 2 : peso + 1;
            }

            var resto = soma % 11;
            var digito1 = resto < 2 ? 0 : 11 - resto;

            if (cnpj[12] - '0' != digito1)
                return false;

            soma = 0;
            peso = 2;
            for (int i = 12; i >= 0; i--)
            {
                soma += (cnpj[i] - '0') * peso;
                peso = peso == 9 ? 2 : peso + 1;
            }

            resto = soma % 11;
            var digito2 = resto < 2 ? 0 : 11 - resto;

            return cnpj[13] - '0' == digito2;
        }
    }
} 