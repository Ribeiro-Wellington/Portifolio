#!/bin/bash

set -e

echo "=== Inicialização do Banco de Dados ==="

echo "1. Aguardando SQL Server estar pronto..."
for i in {1..30}; do
  if /opt/mssql-tools18/bin/sqlcmd -S sqlserver -U sa -P YourStrong@Passw0rd -C -Q "SELECT 1" &> /dev/null; then
    echo "   SQL Server está pronto!"
    break
  fi
  echo "   Tentativa $i/30 - SQL Server ainda não está pronto..."
  sleep 5
done

echo "2. Verificando se o banco CadastroClientes existe..."
DB_COUNT=$(/opt/mssql-tools18/bin/sqlcmd -S sqlserver -U sa -P YourStrong@Passw0rd -C -Q "SELECT COUNT(*) FROM sys.databases WHERE name = 'CadastroClientes'" -h -1 -W | sed -n '/^[0-9]/p' | tr -d ' \t\r\n')

echo "   Resultado: $DB_COUNT bancos encontrados"

if [ "$DB_COUNT" = "0" ]; then
  echo "3. Criando banco de dados CadastroClientes..."
  /opt/mssql-tools18/bin/sqlcmd -S sqlserver -U sa -P YourStrong@Passw0rd -C -Q "CREATE DATABASE CadastroClientes"
  echo "   Banco criado com sucesso!"
else
  echo "3. Banco de dados CadastroClientes já existe."
fi

echo "4. Aplicando migrações do Entity Framework..."
cd /src/CadastroClientes.API
dotnet ef database update --project ../CadastroClientes.Infrastructure

echo "5. Verificação final..."
/opt/mssql-tools18/bin/sqlcmd -S sqlserver -U sa -P YourStrong@Passw0rd -C -Q "SELECT name FROM sys.databases WHERE name = 'CadastroClientes'"

echo "=== Inicialização concluída com sucesso! ===" 