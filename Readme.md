# Aplicación Base para Testeo de Frameworks

Esta es una aplicación base básica que consiste en que los usuarios puedan registrarse y crear publicaciones y comentarlas, con la finalidad de poder comparar este framework con otros en diferentes criterios.

## 📋 Requisitos

- .NET Core 8.0
- PostgreSQL (o cualquier otro sistema de gestión de bases de datos que estés utilizando)

## 🚀 Instalación

1. Instala las dependencias del proyecto:
    ```
    dotnet restore
    ```
## ⚙️ Configuración

1. Para configurar la cadena de conexión a la base de datos en el archivo `appsettings.json`. Es necesario crear un archivo .env con los siguientes valores:
    a. Para la base de datos es necesario:
        - `DB_HOST`: El host donde se encuentra tu base de datos.
        - `DB_NAME`: El nombre de la base de datos a la que te quieres conectar.
        - `DB_USER`: La contraseña para el usuario especificado para la base de datos.
        - `DB_PASS`: El host donde se encuentra tu base de datos.
    b. Para poder enviar los correos para la recuperacion de contraseña es necesario:
        - `EMAIL_USERNAME`: El correo gmail con el cual deseas enviar los correos.
        - `EMAIL_PASSWORD`: La contraseña de aplicación utilizada para autenticarse con la API de Gmail.

⚠️ Asegúrate de que el usuario y la contraseña proporcionados para la base de datos tienen los permisos necesarios para acceder a la base de datos especificada.
## 🗄️Ejecución de las migraciones de la Base de Datos

1. Para crear una nueva migración, ejecuta:
    ```
    dotnet ef migrations add NombreDeLaMigracion
    ```
2. Para aplicar las migraciones a la base de datos, ejecuta:
    ```
    dotnet ef database update
    ```

## 🎮Ejecución del proyecto
1. Para compilar el proyecto, utiliza el comando:
```
dotnet publish -c Release 
```
2. Para ejecutar el proyecto, utiliza el comando:
```
dotnet ruta_al_archivo_dll_generado_con_el_comando_anterior
```

##  🔨 Desarrollo
Para ejecutar el proyecto en un entorno de desarrollo, utiliza el comando:
```
dotnet run
```
