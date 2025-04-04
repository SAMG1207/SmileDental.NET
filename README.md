# 🦷 SmileDental .NET 

API Rest | .NET 8 | Desarrollo de aplicación web para gestión de una clínica dental "SmileDental".

## 🎭 Roles en la Aplicación
- Pacientes.
- Dentistas.
- Administrador.

## 🛠 Tecnologías
- .NET 8
- Entity Framework Core
- SQL Server
- JWT

## 🚀 Características Principales

* ✅ Autenticación Segura con JWT
    - Todos los endpoints de los actores están protegidos con autenticación mediante tokens JWT.
    - Middleware de autorización con app.UseAuthorization().

* ✅ Manejo de Base de Datos con Entity Framework Core
    - Conexión a SQL Server utilizando migraciones y LINQ para consultas eficientes.
    - Soporte para migraciones automáticas (dotnet ef database update).

* ✅ Documentación y Pruebas con Swagger o Postman

## 📦 Instalación y Configuración

(Próximamente en un contenedor para su mejor portabilidad)

- Clonar el repositorio: 
    - git clone https://github.com/SAMG1207/SmileDental.NET
    - cd SmileDental.NET

- IMPORTANTE: Configura tu propia cadena de conexión en el archivo appsettings.json.
- Configura las dependencias: 
    - dotnet restore
    - dotnet ef database update
    - dotnet run

## 📌 Uso
Esta aplicación es una API Rest, así que para su uso es necesario enviar peticiones a los endpoints según lo que se desee, lo que devolverá una respuesta en formato Json. La aplicación tiene en proceso de desarrollo su parte frontend hecha con Typescript, aunque es posible probar y ejecutarla desde Postman o Swagger. 

Recuerda que muchos endopints necesitan una petición "Bearer" para pasar un token de JWT en él, si no, no podrás obtener información y dará el error 401 Unauthorized. 

## 📜 Futuras Mejoras
- Implementación de la interfaz frontend en TypeScript
-  Integración con contenedores Docker para facilitar la portabilidad.
