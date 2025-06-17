# Deploy com Docker

Este documento explica como executar o frontend Angular usando Docker.

## Pré-requisitos

- Docker instalado
- Docker Compose instalado
- API já rodando no Docker (porta 5000)

## Opção 1: Usando Docker Compose (Recomendado)

1. Certifique-se de que a API está rodando no Docker
2. Execute o comando:

```bash
docker-compose up --build
```

O frontend estará disponível em: http://localhost

## Opção 2: Build Manual

1. Construa a imagem:
```bash
docker build -t cadastro-clientes-frontend .
```

2. Execute o container:
```bash
docker run -d --name frontend-container -p 80:80 cadastro-clientes-frontend
```

## Opção 3: Usando o Script

Execute o script de build:
```bash
chmod +x build-and-run.sh
./build-and-run.sh
```

## Configuração

- O frontend roda na porta 80
- A API deve estar rodando na porta 5000
- O Nginx faz proxy das requisições `/api` para a API

## Troubleshooting

### Container não inicia
```bash
docker logs frontend-container
```

### Rebuild da imagem
```bash
docker-compose down
docker-compose up --build
```

### Limpar containers
```bash
docker-compose down
docker system prune -f
``` 