# Mónaco — tienda de bebidas

Aplicación web académica para consultar un catálogo de bebidas, registrar pedidos y administrar productos. El proyecto conecta un frontend Angular con una API ASP.NET Core y una base de datos PostgreSQL.

## Funcionalidades

### Frontend

- Catálogo público con búsqueda, disponibilidad, precio, marca, presentación y stock.
- Registro de cuenta de cliente e inicio y cierre de sesión.
- Acceso como visitante en modo de solo lectura.
- Pedidos para clientes: el cliente solicita una unidad desde una tarjeta, consulta sus pedidos y recibe el resultado de la API.
- Administración de productos para el rol Administrador: crear, editar y desactivar productos.
- Indicadores de carga, mensajes de error y actualización del catálogo.

### Roles

| Rol | Acceso |
|---|---|
| Visitante | Consultar el catálogo. |
| Cliente | Consultar el catálogo, registrar pedidos y consultar sus propios pedidos. |
| Administrador | Consultar, crear, editar y desactivar productos. |

El panel de administración de esta versión gestiona **productos**. Las demás operaciones CRUD antiguas del backend están deshabilitadas por una política de acceso.

## Qué hace el frontend

El frontend está en `FrontendMonacoApi/` y usa Angular 22 con componentes standalone, señales (`signal` y `computed`), `HttpClient` y Tailwind CSS.

- `src/app/app.ts` mantiene la pantalla visible, comprueba la sesión y permite la navegación según el rol.
- `src/app/app.html` presenta el catálogo, el acceso, los pedidos del cliente y la administración.
- `src/app/services/servicio-auth.ts` llama a la API para iniciar sesión, registrar clientes, recuperar la sesión y cerrarla.
- `src/app/services/servicio-productos.ts` obtiene la lista de productos activos.
- `src/app/services/servicio-pedidos.ts` obtiene los pedidos del cliente y envía solicitudes para crear pedidos.
- `src/app/components/` contiene las tarjetas y listas de productos y pedidos, los resúmenes y el formulario CRUD de productos.
- `src/app/models/` define los tipos de producto, pedido y sesión que usa Angular.
- `proxy.conf.json` reenvía las solicitudes `/api/...` al backend local en `http://127.0.0.1:5051`.

La comunicación queda así:

```text
Navegador → Angular (localhost:4200) → proxy /api → ASP.NET Core (localhost:5051)
                                                         ↓
                                                  PostgreSQL
```

El navegador usa rutas relativas como `/api/Productos/listaReal`. Durante el desarrollo, el proxy de Angular las reenvía al backend; por eso no hace falta configurar CORS para esta ejecución local. Si cambias `proxy.conf.json`, detén y vuelve a iniciar `npm start`.

Al confirmar un pedido, el backend valida la sesión del cliente y el stock. Si hay unidades disponibles, registra el pedido y su detalle, y descuenta el stock en la misma transacción.

## Tecnologías

- Frontend: Angular 22, TypeScript, RxJS y Tailwind CSS 4.
- Backend: ASP.NET Core / .NET 10, Entity Framework Core y Npgsql.
- Base de datos: PostgreSQL.
- Autenticación: cookie HTTP-only y contraseña almacenada usando el hasher de ASP.NET Core Identity.

Para Angular 22, instala Node.js 22.22.3 o superior en la línea 22, o una versión compatible indicada por la [tabla oficial de compatibilidad de Angular](https://angular.dev/reference/versions). También necesitas npm, el SDK de .NET 10 y PostgreSQL.

## Ejecutar el proyecto en local

### 1. Preparar PostgreSQL

Crea la base de datos `MonacoApi` en PostgreSQL. Configura la cadena de conexión en User Secrets para que la contraseña no quede en el repositorio.

Abre PowerShell en la carpeta del backend:

```powershell
cd "C:\ruta\a\Proyecto final\Backend_VS\ProyectoDiseñoWeb2"
```

Inicializa User Secrets una sola vez y guarda tu conexión. Cambia el usuario y la contraseña según tu instalación:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:ProyectoDiseñoWeb2Context" "Host=localhost;Port=5432;Database=MonacoApi;Username=postgres;Password=TU_CONTRASEÑA"
```

### 2. Iniciar el backend

Desde `Backend_VS/ProyectoDiseñoWeb2/` ejecuta:

```powershell
dotnet run --launch-profile http
```

El backend aplica las migraciones al iniciar y crea una categoría inicial y una cuenta de demostración si no existen. La API queda en `http://localhost:5051`; la documentación interactiva está en `http://localhost:5051/scalar/`.

Puedes comprobar la consulta pública del catálogo en `http://localhost:5051/api/Productos/listaReal`.

### 3. Iniciar el frontend

Abre una segunda terminal y entra en `FrontendMonacoApi/`:

```powershell
cd "C:\ruta\a\Proyecto final\FrontendMonacoApi"
npm ci
npm start
```

Abre `http://localhost:4200`. Mantén abiertas las dos terminales mientras uses la aplicación.

Para generar la versión compilada del frontend:

```powershell
npm run build
```

## Estructura del repositorio

```text
Proyecto final/
├── Backend_VS/
│   └── ProyectoDiseñoWeb2/
│       ├── Controllers/       # Endpoints y autorización
│       ├── Dominio/           # Entidades de la base de datos
│       ├── Migrations/        # Migraciones de Entity Framework
│       ├── Program.cs         # Configuración, autenticación y arranque
│       └── ProyectoDiseñoWeb2.csproj
├── FrontendMonacoApi/
│   ├── public/
│   ├── src/app/               # Componentes, modelos y servicios Angular
│   ├── proxy.conf.json        # Proxy local hacia el backend
│   ├── package.json
│   └── package-lock.json
├── .gitignore
└── README.md
```

## Preparar la subida a GitHub

El repositorio debe crearse desde la carpeta `Proyecto final/`, que contiene tanto el frontend como el backend. El `.gitignore` de esta raíz excluye dependencias, compilaciones y archivos locales de Visual Studio.

1. Crea un repositorio vacío en GitHub.
2. Abre una terminal en `Proyecto final/` y ejecuta:

```powershell
git init
git status
git add .
git status
git commit -m "Proyecto final Mónaco"
git branch -M main
git remote add origin https://github.com/TU-USUARIO/TU-REPOSITORIO.git
git push -u origin main
```

Reemplaza la URL de ejemplo por la dirección de tu repositorio. Antes de confirmar, revisa el segundo `git status`: no deben aparecer contraseñas, archivos de configuración local, `node_modules`, `dist`, `bin` ni `obj`.

## Configuración de seguridad antes de publicar

- `Program.cs` crea una cuenta de administrador de demostración con una contraseña fija en el código. Cámbiala por una configuración privada antes de usar o publicar el sistema fuera del entorno local.
- Mantén la cadena de conexión y las contraseñas reales fuera de `appsettings.json`, del código fuente y de GitHub. Usa User Secrets en desarrollo y variables de entorno o un gestor de secretos en producción.
- Las cookies de sesión deben usarse con HTTPS en producción. El proxy y las direcciones `localhost` de este README son solo para desarrollo local.

## Estado del proyecto

Esta versión implementa el catálogo, autenticación y registro, pedidos de clientes con control de stock y administración de productos. No incluye pagos en línea ni un panel CRUD general para todas las entidades de la base de datos.
