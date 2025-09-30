# PrimerParcial - API REST

**Desarrollado por: GRUPO 10 - UPSA**

Este proyecto es una API REST construida con ASP.NET Core que proporciona funcionalidades para la gestión de eventos, productos y tickets de soporte técnico.

## 🚀 Descripción del Proyecto

La aplicación implementa tres módulos principales:
- **Gestión de Eventos**: Administración completa de eventos con filtros avanzados
- **Gestión de Productos**: CRUD completo para el catálogo de productos
- **Sistema de Tickets de Soporte**: Manejo de tickets de soporte técnico con estados y asignaciones

## 🛠️ Tecnologías Utilizadas

- **Framework**: ASP.NET Core 9.0
- **Base de Datos**: SQL Server con Entity Framework Core
- **Documentación**: Swagger/OpenAPI
- **Arquitectura**: Patrón Repository con Controllers

## 📊 Modelos de Datos

### Event (Evento)
Representa eventos que pueden ser presenciales o en línea.

**Propiedades:**
- `Id` (int): Identificador único
- `Title` (string): Título del evento (máx. 200 caracteres) *[Requerido]*
- `Location` (string): Ubicación del evento (máx. 200 caracteres) *[Requerido]*
- `StartAt` (DateTime): Fecha y hora de inicio *[Requerido]*
- `EndAt` (DateTime?)): Fecha y hora de finalización *[Opcional]*
- `IsOnline` (bool): Indica si el evento es en línea *[Requerido]*
- `Notes` (string?): Notas adicionales (máx. 1000 caracteres) *[Opcional]*

### Product (Producto)
Representa productos del catálogo con control de inventario.

**Propiedades:**
- `Id` (int): Identificador único
- `Name` (string): Nombre del producto (máx. 100 caracteres) *[Requerido]*
- `Description` (string?): Descripción del producto *[Opcional]*
- `Price` (decimal): Precio del producto (≥ 0) *[Requerido]*
- `Stock` (int): Cantidad en inventario (≥ 0) *[Requerido]*
- `IsActive` (bool): Estado activo del producto (por defecto: true)
- `CreatedAt` (DateTime): Fecha de creación (automática)
- `UpdatedAt` (DateTime?)): Fecha de última actualización *[Opcional]*

### SupportTicket (Ticket de Soporte)
Representa tickets de soporte técnico con sistema de estados y asignaciones.

**Propiedades:**
- `Id` (int): Identificador único
- `Subject` (string): Asunto del ticket (máx. 200 caracteres) *[Requerido]*
- `RequesterEmail` (string): Email del solicitante (formato email válido, máx. 255 caracteres) *[Requerido]*
- `Description` (string?): Descripción detallada del problema *[Opcional]*
- `Severity` (string): Nivel de severidad (máx. 50 caracteres) *[Requerido]*
- `Status` (string): Estado actual del ticket (máx. 50 caracteres) *[Requerido]*
- `OpenedAt` (DateTime): Fecha de apertura (automática)
- `ClosedAt` (DateTime?)): Fecha de cierre *[Automático al cerrar]*
- `AssignedTo` (string?): Usuario asignado (máx. 100 caracteres) *[Opcional]*

## 🔌 API Endpoints

### 📅 EventsController (`/api/Events`)

#### GET `/api/Events`
Obtiene la lista de eventos con filtros opcionales.

**Parámetros de consulta:**
- `from` (DateTime?): Filtrar eventos desde esta fecha
- `to` (DateTime?): Filtrar eventos hasta esta fecha  
- `online` (bool?): Filtrar por tipo (true=online, false=presencial)
- `q` (string?): Búsqueda de texto en título, ubicación o notas

**Respuesta:** Lista de eventos ordenados por fecha de inicio

#### GET `/api/Events/{id}`
Obtiene un evento específico por su ID.

**Parámetros:**
- `id` (int): ID del evento

**Respuestas:**
- `200`: Evento encontrado
- `404`: Evento no encontrado

#### POST `/api/Events`
Crea un nuevo evento.

**Validaciones:**
- La fecha de fin no puede ser menor que la fecha de inicio
- Normalización automática de campos de texto

**Respuesta:** `201` con el evento creado

#### PUT `/api/Events/{id}`
Actualiza completamente un evento existente.

**Validaciones:**
- ID en la URL debe coincidir con el del cuerpo
- Mismas validaciones que POST

**Respuesta:** `204` si actualización exitosa

#### DELETE `/api/Events/{id}`
Elimina un evento.

**Respuesta:** `204` si eliminación exitosa

### 🛍️ ProductController (`/api/Product`)

#### GET `/api/Product`
Obtiene todos los productos.

**Respuesta:** Lista completa de productos

#### GET `/api/Product/{id}`
Obtiene un producto específico.

**Respuestas:**
- `200`: Producto encontrado
- `404`: Producto no encontrado

#### POST `/api/Product`
Crea un nuevo producto.

**Comportamiento automático:**
- `CreatedAt` se establece automáticamente
- `UpdatedAt` se inicializa como null
- `IsActive` se establece como true por defecto

#### PUT `/api/Product/{id}`
Actualiza un producto existente.

**Comportamiento automático:**
- `UpdatedAt` se actualiza automáticamente al momento actual

#### DELETE `/api/Product/{id}`
Elimina un producto.

### 🎫 SupportTicketsController (`/api/SupportTickets`)

#### GET `/api/SupportTickets`
Obtiene tickets con filtros opcionales.

**Parámetros de consulta:**
- `status` (string?): Filtrar por estado
- `severity` (string?): Filtrar por severidad

**Respuesta:** Lista de tickets ordenados por fecha de apertura (más recientes primero)

#### GET `/api/SupportTickets/{id}`
Obtiene un ticket específico.

#### POST `/api/SupportTickets`
Crea un nuevo ticket.

**Comportamiento automático:**
- `OpenedAt` se establece automáticamente
- Si el estado inicial es "Resolved" o "Closed", se establece `ClosedAt` automáticamente
- Normalización de campos de texto

#### PUT `/api/SupportTickets/{id}`
Actualiza completamente un ticket.

**Lógica de cierre automático:**
- Si el estado cambia a "Resolved" o "Closed", se establece `ClosedAt` automáticamente
- Si el estado cambia a otro valor, se limpia `ClosedAt`

#### PATCH `/api/SupportTickets/{id}/status`
Actualiza únicamente el estado del ticket.

**Cuerpo:** String con el nuevo estado (formato JSON)

#### PATCH `/api/SupportTickets/{id}/assign`
Asigna o desasigna un ticket.

**Cuerpo:** String con el usuario asignado (formato JSON, null para desasignar)

#### DELETE `/api/SupportTickets/{id}`
Elimina un ticket.

## 🗄️ Base de Datos

El proyecto utiliza **Entity Framework Core** con **SQL Server**. La configuración incluye:

- **Índices optimizados** para consultas frecuentes
- **Validaciones a nivel de base de datos**
- **Migraciones** para control de versiones del esquema

### Índices Implementados:
- `SupportTickets`: Status, Severity, OpenedAt
- `Events`: StartAt, IsOnline

## ⚙️ Configuración y Ejecución

### Prerrequisitos
- .NET 9.0 SDK
- SQL Server (LocalDB o instancia completa)

### Pasos para ejecutar:

1. **Clonar el repositorio**
2. **Configurar la cadena de conexión** en `appsettings.json`
3. **Ejecutar migraciones**:
   ```bash
   dotnet ef database update
   ```
4. **Ejecutar la aplicación**:
   ```bash
   dotnet run
   ```

### URLs de la aplicación:
- **API**: `https://localhost:7xxx` (puerto asignado automáticamente)
- **Swagger UI**: `https://localhost:7xxx/swagger`
- **Health Check**: `GET /ping` → Responde "pong"

## 📖 Documentación de la API

La documentación completa de la API está disponible a través de **Swagger UI** cuando la aplicación está en ejecución.

**Accede a la documentación interactiva en:**
```
🔗 https://app-250929212203.azurewebsites.net/swagger/index.html
```

*Nota: Reemplaza este espacio con la URL real de tu API una vez desplegada.*

## 🧪 Pruebas

El proyecto incluye un archivo `PrimerParcial.http` con ejemplos de requests para probar todos los endpoints.

## 👥 Equipo de Desarrollo

**GRUPO 10 - UPSA**

---

**© 2025 GRUPO 10 - UPSA. Todos los derechos reservados.**
