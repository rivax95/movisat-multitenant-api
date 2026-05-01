# Movisat Multi-Tenant Products API

API REST en .NET 8 para gestionar productos con versionado de rutas y aislamiento por tenant.

## Requisitos

- .NET SDK 8 o 9

## Levantar la API

```powershell
dotnet restore
dotnet run --project src/Movisat.MultiTenantApi.Api/Movisat.MultiTenantApi.Api.csproj
```

La API crea automaticamente una base SQLite local `products.db` y la seedea si esta vacia.

Endpoints principales:

```http
GET /api/v1/acme/products?category=navigation
GET /api/v1/acme/products
GET /api/1/acme/products?category=navigation
GET /health
```

Swagger se expone cuando el entorno es `Development`.

## Ejecutar pruebas

```powershell
dotnet test
```

Las pruebas usan `WebApplicationFactory<Program>` y `HttpClient` real del `TestServer`, atravesando middleware, routing, endpoint filter, Minimal API, Application, Infrastructure, EF Core, SQLite y serializacion JSON.

## Arquitectura

La solucion esta separada en capas:

- `Movisat.MultiTenantApi.Domain`: entidad `Product` e invariantes basicas del catalogo.
- `Movisat.MultiTenantApi.Application`: casos de uso, DTOs internos e interfaces.
- `Movisat.MultiTenantApi.Infrastructure`: EF Core, SQLite, seed y repositorios.
- `Movisat.MultiTenantApi.Api`: composicion, rutas HTTP, middleware, filtros y contratos JSON.
- `Movisat.MultiTenantApi.Tests`: pruebas de integracion end-to-end con DB aislada por test.

Documento de decisiones: [docs/DECISIONES_ARQUITECTURA.md](docs/DECISIONES_ARQUITECTURA.md).

## Reglas implementadas

- `version` solo acepta `1` o `2`; cualquier otro valor enrutable devuelve `400 Bad Request`.
- Tenants bloqueados `sandbox` y `test` devuelven `403 Forbidden`.
- Si se informa `category` y no existe para ese tenant/version, devuelve `404 Not Found`.
- Si no se informa `category`, devuelve todos los productos del tenant/version.
- Las respuestas validas incluyen headers `X-Tenant` y `X-Api-Version`.

## Casos cubiertos por tests

- Ruta valida con categoria existente: `200 OK` y JSON estructurado.
- Version `3`: `400 Bad Request`.
- Tenant `test` y `sandbox`: `403 Forbidden`.
- Categoria inexistente: `404 Not Found`.
- Peticion sin categoria: devuelve todos los productos del tenant/version.
- No hay fuga de datos entre tenants.
- No hay fuga de datos entre versiones.
- Soporte adicional para ruta sin prefijo `v`: `/api/1/{tenant}/products`.
