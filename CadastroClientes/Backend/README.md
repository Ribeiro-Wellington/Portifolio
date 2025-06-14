-- API Cadastro de Clientes

- Tecnologias Utilizadas

- .NET 8
- Entity Framework Core
- SQL Server
- Docker & Docker Compose
- DDD (Domain-Driven Design)
- CQRS (Command Query Responsibility Segregation)
- Event Sourcing
- FluentValidation
- xUnit (Testes Unitários)

Passo á Passo:

executar o comando: docker-compose up --build

O sistema irá:
1. Iniciar o SQL Server
2. Aguardar o SQL Server estar pronto
3. Criar o banco de dados automaticamente (se não existir)
4. Aplicar as migrações do Entity Framework
5. Iniciar a API

Para acessar a API
- Swagger: http://localhost:5000/swagger



- Exemplo de Criação de Cliente

{
    "nome": "Admin Wellington",
    "documento": "12345678978",
    "isPessoaJuridica": false,
    "inscricaoEstadual": null,
    "isento": true,
    "dataNascimento": "1993-08-09T00:00:00",
    "telefone": "(11) 91111-2222",
    "email": "email@email.com.br",
    "cep": "11111-445",
    "endereco": "rua um",
    "numero": "01",
    "bairro": "bairro um",
    "cidade": "cidade um",
    "estado": "SP"
}

- Exemplo de Alteração de Cliente
 
 {
  "id": "Guid Gerado",
  "nome": "Wellington Admin Admin",
  "telefone": "(11) 99999-9988",
  "email": "email2@email.com",
  "cep": "11122-887",
  "endereco": "rua das pedras",
  "numero": "99",
  "bairro": "bairro das pedras",
  "cidade": "Pedras",
  "estado": "RS",
  "inscricaoEstadual": null,
  "isento": true
}