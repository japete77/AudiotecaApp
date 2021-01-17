Para desarrollar una aplicación para mac o iOS es en primer lugar necesario crearse una cuenta en https://developer.apple.com/

############
CERTIFICADOS
############
El siguiente paso es definir un perfil de desarrollo, para lo cual hay que crear un certificado de Desarrollo y otro de Distribución. 

Para generar estos certificados:
- Generar un Certificate Signing Request desde el mac
    * Abrir Keychain Access.app
    * Acceso a llaveros -> Asistentes para certificados -> Solicitar un certificado de una autoridad de certificación
    * Asignar dirección de email: nueva.luz.desarrollo@gmail.com
    * Asignar un nombre: Nueva Luz
    * Guardar en disco el certificado: CertificateSigningRequest.certSigningRequest
- Ir a https://developer.apple.com/account/resources/certificates/list 
- Pulsar en "+" 
- Seleccionar el tipo de certificado (Development, Distribution, etc.)
- Subir el Certificate Signing Request generado en el primer paso
- Completar la generación
- Se puede ahora descargar e instalar en el mac por si hiciera falta exportarlo a formato .p12

###########
IDENTIFIERS
###########
El identificador es lo identifica de manera única mi aplicación. Hay que generar uno que será usado para generar todas las versiones de la aplicación.

- Ir a https://developer.apple.com/account/resources/identifiers/list/bundleId
- Añadir nuevo identifier
- Seleccionar App IDs
- Asignar el BundleID. Este identificador debe ser único y se recomienda usar un dominio inverso (i.e. com.nuevaluz)
- Asinar la descripción
- Seleccionar las Capabilities (los servicios de los que herá uso la aplicación. En el caso integración con las notificaciones APN hay que seleccionar dicho servicio y configurar los certificados correspondientes)

#######
DEVICES
#######
En esta sección se dan de alta los dispositivos permitidos para la implementación y despliegue de la aplicación en desarrollo.

 - Añade un nuevo dispositivos
 - Rellena el nombre
 - Asigna el UUID. Puedes averiguar el UIDD de tu dispositivo siguiendo estos pasos:
    * Conecta el iPhone, iPad o iPod a tu Mac.
    * Entra en el Finder y accede a la ubicación de tu iPhone en la parte lateral izquierda.
    * Aquí observaremos toda la información de nuestro equipo pero el UDID está algo oculto. Deberemos de ir con el ratón a la parte superior izquierda y haremos clic encima de la información de batería y almacenamiento que se nos muestra de nuestro equipo.
    * Con un solo clic tendremos acceso al nº de serie y al UDID.

########
PROFILES
########
Con todos los elementos anteriores registrados, ahora podemos crear un perfil que agrupe todos estos elementos.
- Ir a https://developer.apple.com/account/resources/profiles/list
- Crea un nuevo perfil
- Selecciona el tipo de perfil
- Selecciona el AppID
- Selecciona el certificado apropiado
- Selecciona los dispositivos permitidos
- Asignar el nombre
- Completar el proceso


Una vez con los perfiles definidos, hay que actualizarlos en la lista de perfiles de Visual Studio
- Abrir Visual Studio
- Ir al menú superior: Visual Studio -> Preferencias
- En la sección Publicación -> Cuentas de desarrollador de apple
- Dar de alta la cuenta con la contraseña de acceso
- Ir a ver detalles
- Descargar todos los perfiles

A partir de ese momento ya deben aparecer los perfiles asociados que estarán disponibles en las propiedades del proyecto (Menu contextual del proyecto -> Opciones -> Compilación -> Firma de lote para iOS)

**************
NOTIFICACIONES
**************
Para poder hacer uso de las notificaciones hay que activar el servicio en el AppID en las capabilities: Push Notifications
- Marcar el servicio como activo en el Identifier
- Crear el certificado para desarrollo y producción
- Una vez creados hay que ir a Certificates y descargarlos
- Instalarlos en el mac y exportar el certificado .p12
- Con ese certificado hay que configurar el Platform applications en SNS de AWS

* NOTA MUY IMPORTANTE *: Para que el APN de producción funcione hay que configurar en el fichero Entitlement.plist la clave "Platform applications" a "production" (para desarrollo debe tener el valor "development")


*******************
UPLOAD TO APP STORE
*******************
- Desde visual studio usar el menú contextual en el proyecto de la App
- Archivo
- Una vez generado el archivo abrir xcode.app
- Window -> Organizer (ahi aparecerá el bundle compilado)
- Seleccionar el paquete y pulsar en "Distribute App"
- Selecciona el destino
- Seleccina si es para subir o exportar
- Seleccionar el perfil apropiado
- Subir