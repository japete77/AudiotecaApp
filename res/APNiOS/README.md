## **1. Crear una Clave APNs en Apple Developer**

Antes de integrar APNs con Firebase o AWS SNS, necesitas una clave `.p8` desde Apple Developer.

### **Pasos para generar la clave:**

1. Ve a **Apple Developer** e inicia sesión.
2. Accede a **Certificates, Identifiers & Profiles**.
3. En la sección **Keys**, haz clic en el botón **"+" (Crear nueva clave)**.
4. Introduce un **nombre descriptivo** para la clave, como `"Clave APNs Firebase"`.
5. Activa la opción **Apple Push Notification service (APNs)**.
6. Haz clic en **Continuar** y luego en **Registrar**.
7. Descarga el archivo `.p8` generado y guárdalo en un lugar seguro. ⚠️ **Apple solo te permite descargarlo una vez**.

### **Información clave necesaria**

Después de descargar el `.p8`, necesitarás estos valores:

- **Key ID** (se muestra en Apple Developer junto a la clave descargada).
- **Team ID** (lo encuentras en la cuenta de Apple Developer).
- **Bundle ID** de tu aplicación (debe coincidir con el de tu app en MAUI).

------

## **2. Configurar Firebase con la clave APNs**

### **Subir la clave a Firebase**

1. Ve a la **Consola de Firebase**.
2. Selecciona tu proyecto.
3. Ve a **Project Settings > Cloud Messaging**.
4. En la sección **APNs Authentication Key**, haz clic en **Upload**.
5. Sube el archivo `.p8`.
6. Ingresa el **Key ID** y el **Team ID**.
7. Asegúrate de que el **Bundle ID** sea correcto.
8. Guarda los cambios.

Ahora Firebase puede enviar notificaciones push a dispositivos iOS usando la clave APNs.

------

## **3. Configurar AWS SNS con la clave APNs**

Si quieres usar **AWS SNS** en lugar de Firebase para enviar notificaciones, debes configurar una plataforma en SNS.

### **Pasos para configurar AWS SNS con la clave APNs**

1. Ve a la **Consola de Amazon SNS**.

2. Ve a **Applications** y haz clic en **Create Platform Application**.

3. Introduce un nombre para la aplicación (ej. `"MyApp_APNs"`).

4. En 

   Push notification platform

   , selecciona:

   - **APNs Sandbox** para pruebas desde Xcode.
   - **APNs (Producción)** para TestFlight/App Store.

5. En **Authentication Method**, elige **Token-based**.

6. Sube la clave 

   .p8

    y completa:

   - **Key ID** (lo copiaste en el paso 1).
   - **Team ID** (lo encuentras en Apple Developer).
   - **Bundle ID** de la aplicación.

7. Guarda los cambios y copia el **ARN de la aplicación**.

Ahora SNS podrá enviar notificaciones a dispositivos iOS.



**Key ID**: G27D562G3P
**Team ID**: JNM343SE4U



Para ejecutar la app desde un dispositivo físico y poder recibir las notificaciones

### 2. **Registrar tu iPad como dispositivo**

1. **Obtén el UDID de tu iPad:**
   - Conecta tu iPad a tu Mac.
   - Abre Finder (o iTunes en versiones anteriores) y selecciona tu dispositivo.
   - Haz clic en el número de serie para que se muestre el UDID (puedes copiarlo).
2. **Agregar el dispositivo en el portal de desarrolladores:**
   - Accede a tu [Apple Developer Account](https://developer.apple.com/account).
   - Ve a **Certificates, Identifiers & Profiles** > **Devices**.
   - Añade un nuevo dispositivo e introduce el UDID de tu iPad junto con un nombre descriptivo.

------

### 3. **Crear un App ID**

1. En el portal de desarrolladores, ve a **Certificates, Identifiers & Profiles** > **Identifiers**.
2. Crea un nuevo **App ID** o asegúrate de que el existente tenga el **Bundle Identifier** que usas en tu proyecto (por ejemplo, `com.nuevaluz.appfonoteca`).
3. Verifica que las capacidades (como Push Notifications, etc.) estén activadas según lo que necesite tu app.

------

### 4. **Crear un certificado de desarrollo**

1. En el portal de desarrolladores, dirígete a **Certificates**.

2. Crea un nuevo certificado para 

   iOS Development

    siguiendo las instrucciones:

   - Genera una **Certificate Signing Request (CSR)** desde tu Mac usando la utilidad Keychain Access.
   - Sube el CSR y descarga el certificado generado.

3. Instala el certificado haciendo doble clic sobre él para añadirlo a tu llavero.

------

### 5. **Crear un perfil de aprovisionamiento de desarrollo**

1. En **Certificates, Identifiers & Profiles**, ve a **Profiles** y crea un nuevo perfil de **Development**.
2. Selecciona el **App ID** que creaste o usas para tu aplicación.
3. Elige el certificado de desarrollo que creaste en el paso anterior.
4. Selecciona el dispositivo (tu iPad) de la lista de dispositivos registrados.
5. Descarga el perfil de aprovisionamiento generado.