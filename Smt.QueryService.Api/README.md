# SMT Query Service

Servicio de consulta de métricas de tráfico del Sistema de Monitoreo de Tráfico (SMT).  
Implementado bajo principios **cloud-native** y **12-Factor App** como parte del Milestone 3 del proyecto Capstone.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- (Opcional) Cliente REST: [Swagger UI](http://localhost:8080/swagger), Postman o curl

## Configuración

1. Copia el archivo de ejemplo y ajusta los valores según tu entorno:

```bash
cp .env.example .env
```

Variables disponibles:

| Variable | Descripción | Valor por defecto |
|---|---|---|
| `APP__SERVICENAME` | Nombre del servicio | `smt-query-service` |
| `APP__ENVIRONMENTNAME` | Entorno de ejecución | `Development` |
| `APP__DEFAULTMETRICSINTERVALMINUTES` | Intervalo entre métricas simuladas (minutos) | `60` |
| `ASPNETCORE_URLS` | URL en la que escucha el servicio | `http://0.0.0.0:7036` |
| `ASPNETCORE_ENVIRONMENT` | Entorno ASP.NET Core | `Development` |


## Ejecución

```bash
cd Smt.QueryService.Api
dotnet run
```

El servicio estará disponible en: `https://localhost:7036/`


## Endpoints

### Health Check
```
GET /health
```
Respuesta:
```json
{
  "status": "ok",
  "timestampUtc": "2026-04-19T23:51:37.3061403Z"
}
```

### Métricas de tráfico
```
GET /metrics?locationId=loc-001&from=2026-04-01T00:00:00Z&to=2026-04-02T00:00:00Z
```
Parámetros:

| Parámetro | Tipo | Descripción |
|---|---|---|
| `locationId` | string | ID de la ubicación |
| `from` | datetime (UTC) | Inicio del rango |
| `to` | datetime (UTC) | Fin del rango |

Respuesta:
```json
[
    {
        "locationId": "loc-001",
        "timestampUtc": "2026-04-01T12:00:00Z",
        "vehicleCount": 1045,
        "avgSpeed": 58,
        "maxSpeed": 90,
        "minSpeed": 45
    }
]
```

### Swagger UI
```
GET /swagger/index.html
```


## Notas de diseño

- Los datos son **simulados** mediante un generador seeded por `locationId` y rango de fechas, garantizando reproducibilidad en las respuestas.
- El servicio es **stateless**: no mantiene estado entre solicitudes.
- Toda la configuración se inyecta por **variables de entorno**, sin valores hardcodeados en el código.
- Los logs se emiten a **stdout** en formato de consola estructurado.

