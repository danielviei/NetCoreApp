# Nombre del Proyecto

Descripción del proyecto.

## Requisitos

- .NET Core 8.0
- PostgreSQL (o cualquier otro sistema de gestión de bases de datos que estés utilizando)

## Instalación

1. Instala las dependencias del proyecto:
    ```
    dotnet restore
    ```
2. Configura tu cadena de conexión a la base de datos en el archivo `appsettings.json`. Para esto necesitas un archivo .env

## Ejecución de las migraciones

1. Para crear una nueva migración, ejecuta:
    ```
    dotnet ef migrations add NombreDeLaMigracion
    ```
2. Para aplicar las migraciones a la base de datos, ejecuta:
    ```
    dotnet ef database update
    ```

## Ejecución del proyecto

Para ejecutar el proyecto, utiliza el comando:
```
dotnet run
```