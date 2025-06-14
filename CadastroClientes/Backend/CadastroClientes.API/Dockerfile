FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CadastroClientes.API/CadastroClientes.API.csproj", "CadastroClientes.API/"]
COPY ["CadastroClientes.Application/CadastroClientes.Application.csproj", "CadastroClientes.Application/"]
COPY ["CadastroClientes.Domain/CadastroClientes.Domain.csproj", "CadastroClientes.Domain/"]
COPY ["CadastroClientes.Infrastructure/CadastroClientes.Infrastructure.csproj", "CadastroClientes.Infrastructure/"]
RUN dotnet restore "CadastroClientes.API/CadastroClientes.API.csproj"
COPY . .
WORKDIR "/src/CadastroClientes.API"
RUN dotnet build "CadastroClientes.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CadastroClientes.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CadastroClientes.API.dll"] 