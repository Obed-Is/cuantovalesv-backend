# Cuanto vale sv - API

**Cuanto Vale SV** es una aplicación web diseñada para facilitar la búsqueda y comparación de precios de productos ofrecidos por empresas reconocidas en El Salvador.

La plataforma actúa únicamente como un **agregador de información**, permitiendo a los usuarios encontrar el mejor precio disponible y redirigiéndolos al sitio oficial del proveedor correspondiente para completar su compra.

**Cuanto Vale SV no vende productos ni actúa como intermediario comercial**.
Tampoco realiza prácticas como clonación, falsificación, suplantación o cualquier otro tipo de actividad fraudulenta. Su propósito es exclusivamente informativo y de comparación.

## Documentación de la API

A continuacion se presenta la documentacion neceria para comprender la estructura y el uso de la API.

La API cuenta con 2 endpoints principales para la busqueda de productos:

| METODO | Endpoint | DESCRIPCION |
| :--- | :---: | :---: | 
| GET | api/products?term= | Busca productos en todas las empresas disponibles usando el término proporcionado en la query. | 
| POST | api/products?term= | Busca productos en empresas específicas. Recibe un arreglo de empresas en el cuerpo de la petición y el término de búsqueda en la query. |

#### Ejemplo (GET)

```bash
GET /api/products?term=laptop
```

#### Ejemplo (POST)

```bash
POST /api/products?term=laptop
Content-Type: application/json
```

```JSON
{
  "filters": ["walmart", "siman"]
}
```

### Estructura
La API sigue el patron **Clean Arquitecture** de manera simplificada, compuesta por 3 capas principales.

Las dependencias siguen el siguiente flujo:
```bash
Core ← Infrastructure ← WebApi.
```

```bash
.
├── Core/
│   ├── DTOs/ - Objetos que se utilizan para transferir datos entre capas
│   ├── Interfaces/ - Interfaces de los servicios
│   └── Exceptions/ - Excepciones personalizadas
├── Infrastructure/
│   ├── Scrapers/ - Scrapers de los productos
│   └── Services/ - Servicios de los productos
└── WebApi/
    ├── Controllers/ - Controladores de la API
    ├── Middlewares/ - Middlewares de la API
    └── Program.cs - Punto de entrada de la API
```

### Instalacion

Para ejecutar la API necesita tener instalado .NET 8 SDK y seguir estos pasos:

- Clonar el repositorio:

```git
git clone https://github.com/Obed-Is/cuantovalesv-backend.git
```

- Acceder a la carpeta del proyecto:
```bash
cd cuantovalesv-backend
```

- Restaurar dependencias:
```bash
dotnet restore
```

- Instalar navegadores de Playwright:
```bash
pwsh bin/Debug/net8.0/playwright.ps1 install
``` 
> Nota: Si el comando anterior presenta errores, consulta la documentación oficial de Playwright:
[playwright.dev](https://playwright.dev/dotnet/docs/browsers)

- Ejecutar la API:
```bash
dotnet run --project WebApi
```

### Uso en Visual Studio
Si utilizas Visual Studio 2022 o superior, establece WebApi como proyecto de inicio antes de ejecutar la solución.


### Notas

- Los resultados son obtenidos en tiempo real mediante scrapers.
- El tiempo de respuesta puede variar según el sitio consultado y la velocidad de su intenet.
- La disponibilidad de productos depende de cada proveedor.