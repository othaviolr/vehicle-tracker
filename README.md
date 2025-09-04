# Vehicle Tracker API

Sistema de rastreamento veicular desenvolvido em .NET 8 com PostgreSQL, implementando arquitetura limpa e padrões enterprise para gestão de frotas e monitoramento GPS.

## Tecnologias Utilizadas

- .NET 8 Web API
- Entity Framework Core
- PostgreSQL
- FluentValidation
- Serilog
- Swagger/OpenAPI
- Docker

## Arquitetura

O projeto segue Clean Architecture com separação clara de responsabilidades:
VehicleTracker.API/          # Controllers, Swagger, configurações
VehicleTracker.Application/  # Services, DTOs, validações
VehicleTracker.Domain/       # Entidades, interfaces, regras de negócio
VehicleTracker.Infrastructure/ # Repositórios, EF Context, banco

## Funcionalidades

### Gestão de Veículos
- CRUD completo de veículos
- Validação de placas brasileiras (formato antigo e Mercosul)
- Busca por placa, status ou paginação
- Soft delete para auditoria

### Rastreamento GPS
- Registro de localizações com coordenadas e velocidade
- Histórico com filtros por data
- Consulta da última posição conhecida
- Validação de coordenadas geográficas

### Sistema de Alertas
- Listagem de veículos roubados
- Veículos em manutenção
- Status customizáveis por enum
