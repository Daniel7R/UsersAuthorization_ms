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
- **Infrastructure/EventBus/**: Contiene la config del event bus/rabbitmq.
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

***Respuesta***:
```
{
    "result": null,
    "isSuccess": true,
    "message": ""
}
```

- [POST] `/api/v1/auth/login`: este metodo es el encargado de autenticar al usuario de acuerdo a un usuario y contraña que se envian en la peticion, con el fin de autenticar al usuario para poder estar autorizado a hacer uso de ciertos recursos, este metodo retorna varios items del detalle del user, ademas de un JWT.

***Body**:
```
{
  "username": "pepito2@gmail.com",
  "password": "Carlos123*"
}
```

***Respuesta***:
```
{
    "result": {
        "user": {
            "id": 5,
            "name": "Pepito Perez",
            "email": "pepito2@gmail.com",
            "status": "ACTIVE"
        },
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InBlcGl0bzJAZ21haWwuY29tIiwic3ViIjoiNSIsIm5hbWUiOiJQZXBpdG8gUGVyZXoiLCJuYmYiOjE3NDIwMjE4MzQsImV4cCI6MTc0MjYwODYzNCwiaWF0IjoxNzQyMDAzODM0LCJpc3MiOiJlLXNwb3J0cy1hdXRoLWFwaSIsImF1ZCI6ImUtc3BvcnRzLWNsaWVudCJ9.YoSsmvgpc4ijw60b235BOmLNBnoK4jVqIFOksSMD_uc"
    },
    "isSuccess": true,
    "message": ""
}
```



## RabbitMQ(Event Bus)
En el proyecto se hace uso de RabbitMQ como Message Broker para procesamiento de eventos sincronos con el patron de integracion Request/Reply. 

### Colas Procesamiento Sincrono(Request/Reply Queues)
- `user.by_id"`: esta cola se encarga de obtener info basica del usuario para validaciones realizadas en algunos ms, con respecto a los usuarios.
- `users.bulk.info`: esta cola se encarga de obtener la info de los usuarios, pero a nivel de lotes, para mandar correos masivos en las notificaciones.
- `users.emails`: obtiene los ids de los usuarios a los cuales se les realizara el envio del correo masivo dentro del sistema