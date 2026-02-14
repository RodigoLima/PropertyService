# PropertyService

Microsserviço de propriedades e talhões da plataforma AgroSolutions. Responsável pelo cadastro de propriedades e talhões (áreas de plantio com cultura), com status por talhão (Normal, Alerta de Seca, Risco de Praga) atualizado via mensageria a partir do motor de alertas.

## Tecnologias

- .NET 9
- PostgreSQL
- MassTransit (RabbitMQ)
- Docker e Kubernetes (Kind)
- Prometheus e Grafana
- JWT (validação de token emitido pelo IdentityService)

## Estrutura

```
src/
├── PropertyService.Api
├── PropertyService.Application
├── PropertyService.Domain
└── PropertyService.Infrastructure
tests/
└── PropertyService.Tests
k8s/
├── kind/
└── base/
```

## Pré-requisitos

- .NET 9 SDK
- PostgreSQL
- RabbitMQ (para consumer de atualização de status)

## Configuração

`appsettings.json` / variáveis de ambiente:

- `ConnectionStrings:DefaultConnection` – PostgreSQL
- `RabbitMq:Host`, `Username`, `Password`, `VirtualHost`
- `Jwt:Issuer`, `Jwt:Key` (validação do token do IdentityService)

## Executar

```bash
dotnet restore PropertyService.sln
dotnet run --project src/PropertyService.Api
```

Swagger: `http://localhost:5173/swagger` (ou porta configurada em `ASPNETCORE_URLS`).

## Endpoints principais

- `GET/POST/PUT/DELETE /api/Propriedades` – CRUD de propriedades (autenticado)
- `GET/POST/PUT/DELETE /api/Talhoes/*` – CRUD de talhões por propriedade, com cultura (autenticado)

Resposta dos talhões inclui o campo `Status` (Normal, AlertaDeSeca, RiscoDePraga), atualizado quando o serviço de medições/alertas publica eventos na fila.

## Mensageria

- Consome `TalhaoStatusUpdateMessage` (RabbitMQ): ao receber, atualiza o status do talhão no banco.

## Observabilidade

- Métricas Prometheus em `/metrics`
- Dashboards Grafana provisionados em `k8s/base/observability/grafana`

## Testes e CI/CD

```bash
dotnet test tests/PropertyService.Tests.csproj --configuration Release
```

Pipeline GitHub Actions: CI (build + testes) e CD (build e push da imagem Docker para Docker Hub).
