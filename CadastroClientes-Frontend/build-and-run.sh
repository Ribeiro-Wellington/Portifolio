#!/bin/bash

echo "Construindo a imagem do frontend..."
docker build -t cadastro-clientes-frontend .

echo "Executando o container..."
docker run -d --name frontend-container -p 80:80 cadastro-clientes-frontend

echo "Frontend rodando em http://localhost" 