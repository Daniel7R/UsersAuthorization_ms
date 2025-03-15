# UsersAuthorization
Este proyecto gestiona el registro y autorización de usuarios en el sistema de e-sports, por medio de 2 endpoints, que son los

## Requisitos Previos
- .NET 8 SDK
- PostgreSQL
- RabbitMQ Server

## Estructura del Proyecto

- **API/Controllers/**: Contiene los controladores de la API.
- **Domain/Entities/**: Contiene los modelos o entidades de datos.
- **Application/Services/**: Contiene los servicios de la aplicación.
- **Infrastructure/Data/**: Contiene el contexto de la base de datos.
- **Program.cs**: Punto de entrada del proyecto.

# Instrucciones de Ejecución
Para ejecutar el proyecto UsersAuthorization, sigue estos pasos:

- Asegúrate de tener una base de datos PostgreSQL en funcionamiento.
- Configura las variables de entorno necesarias o modifica los archivos appsettings.json, según sea necesario.
- Navega al directorio del proyecto UsersAuthorization.
- Ejecuta el siguiente comando para aplicar las migraciones de la base de datos: `dotnet ef database update`
- Ejecuta el siguiente comando para iniciar el proyecto: `dotnet run`

Esto iniciará el proyecto y estará listo para gestionar la autorización de usuarios.

Generar Documentación con Swagger
Swagger automáticamente genera la documentación de la API. Para ver la documentación generada, inicia la aplicación y navega a http://localhost:<puerto>/swagger.

## Enpoints Controllers

- [POST] `/api/v1/auth/register`: este metodo es el encargado de registrar un usuario dentro del sistema para poder despues hacer uso de un metodo de inicio de sesion.

### Ejemplo: `/api/v1/auth/register`

***Body***:
```
{
  "name": "Pepito Perez",
  "email": "pepito@gmail.com",
  "password": "Carlos123*"
}
```