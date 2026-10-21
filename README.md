Monaco API 

API REST desarrollada con ASP.NET Core para administrar productos y operaciones de una tienda. Incluye categorías, clientes, sucursales, pedidos, pagos, usuarios y solicitudes de soporte.

Tecnologías

- .NET 10

- ASP.NET Core Web API

- Entity Framework Core

- PostgreSQL con Npgsql

- Scalar y OpenAPI para consultar la documentación de la API



Requisitos



- .NET 10 SDK

- PostgreSQL instalado y en ejecución

- Conexión a Internet la primera vez para descargar los paquetes del proyecto


Descargar el proyecto



```bash

git clone https://github.com/josu777123/Monaco-API.git

cd Monaco-API

```



El proyecto de la API está dentro de la carpeta `ProyectoDiseñoWeb2`.


Configurar la base de datos


Crea una base de datos llamada `ProyectoDiseñoWeb2Context`. Puedes hacerlo desde pgAdmin, conectado a PostgreSQL, ejecutando:



```sql

CREATE DATABASE "ProyectoDiseñoWeb2Context";

```



El proyecto lee la conexión con la clave `ProyectoDiseñoWeb2Context`. Para guardar tus datos de conexión localmente, abre una terminal en la raíz del repositorio y ejecuta estos comandos en PowerShell:



```powershell

dotnet user-secrets init --project ./ProyectoDiseñoWeb2/ProyectoDiseñoWeb2.csproj

dotnet user-secrets set "ConnectionStrings:ProyectoDiseñoWeb2Context" "Host=localhost;Port=5432;Database=ProyectoDiseñoWeb2Context;Username=postgres;Password=TU\_CONTRASEÑA" --project ./ProyectoDiseñoWeb2/ProyectoDiseñoWeb2.csproj

```



Reemplaza `TU\_CONTRASEÑA` por la contraseña de tu usuario de PostgreSQL. Si PostgreSQL usa otro puerto o usuario, actualiza también esos valores.



El primer comando se ejecuta una sola vez. Añade al proyecto un identificador para los secretos; ese identificador no contiene tu contraseña. Los secretos se guardan en tu equipo y no se suben al repositorio.



Instalar dependencias y crear las tablas



Desde la raíz del repositorio, restaura los paquetes:



```bash

dotnet restore ./ProyectoDiseñoWeb2/ProyectoDiseñoWeb2.csproj

```



Si todavía no tienes instalada la herramienta de Entity Framework, instálala:



```bash

dotnet tool install --global dotnet-ef --version 10.0.13

```



Aplica la migración incluida para crear las tablas en PostgreSQL. En PowerShell, ejecuta:



```powershell

$env:ASPNETCORE\_ENVIRONMENT = "Development"

dotnet ef database update --project ./ProyectoDiseñoWeb2/ProyectoDiseñoWeb2.csproj --startup-project ./ProyectoDiseñoWeb2/ProyectoDiseñoWeb2.csproj

```



La migración inicial se encuentra en `ProyectoDiseñoWeb2/Migrations`.


Ejecutar la API


Desde la raíz del repositorio:



```bash

dotnet run --project ./ProyectoDiseñoWeb2/ProyectoDiseñoWeb2.csproj --launch-profile http

```



La API se ejecutará en:



```text

http://localhost:5051

```



La documentación interactiva de Scalar está disponible en:



```text

http://localhost:5051/scalar/

```



La especificación OpenAPI está disponible en:



```text

http://localhost:5051/openapi/v1.json

```



\Recursos disponibles



Los controladores ofrecen operaciones para listar, buscar, crear, actualizar y borrar registros. La documentación de Scalar muestra las rutas y los parámetros de cada operación.



Categorías : `/api/Categorias` 
Clientes : `/api/Clientes` 
Detalles de pedidos : `/api/DetallePedidos` 
Pagos : `/api/Pagos` 
Pedidos : `/api/Pedidos` 
Productos : `/api/Productos` 
Solicitudes de soporte : `/api/SolicitudesSoporte` 
Sucursales : `/api/Sucursales` 
Usuarios : `/api/Usuarios` 



Ejemplos de rutas:



```text

GET    /api/Productos/listaReal

GET    /api/Productos/BuscarProducto/{Id\_Producto}

POST   /api/Productos/Crear

PUT    /api/Productos/ActualizarProducto/{...}

DELETE /api/Productos/Borrar/{Id\_Producto}

```



Estructura del proyecto



```text

Monaco-API/

├── README.md

└── ProyectoDiseñoWeb2/

&#x20;   ├── Controllers/   # Endpoints de la API

&#x20;   ├── Dominio/       # Modelos de datos

&#x20;   ├── Migrations/    # Migraciones de Entity Framework

&#x20;   ├── Properties/    # Configuración de ejecución

&#x20;   ├── Program.cs     # Configuración y arranque de la API

&#x20;   └── ProyectoDiseñoWeb2.csproj

```



Notas



- Este repositorio contiene el backend de la API; no necesita Node.js para ejecutarse.

- La conexión de `appsettings.json` contiene valores locales de ejemplo. Usa User Secrets para tus datos de PostgreSQL y evita subir contraseñas reales a GitHub.
