# unity-i2.Loc-extensions
🧩 I2 Localization CSV Extension

Extensión personalizada para el plugin I2 Localization de Unity.
Esta extensión permite descargar, importar y gestionar automáticamente archivos CSV de localización (por ejemplo, desde Google Sheets) en tiempo de ejecución, manteniendo sincronizados los idiomas del proyecto con una hoja de cálculo remota.

⚠️ Nota: Este repositorio no distribuye el plugin I2 Localization, solo proporciona una extensión complementaria.
I2 Localization es propiedad de Inter Illusion, y se puede adquirir en la Unity Asset Store
.

🚀 Características principales
🔹 LocalizationCSVLoader.cs

- Descarga automática de un CSV remoto desde una URL configurable (por ejemplo, Google Sheets).

- Importación directa del contenido al LanguageSourceAsset de I2 Localization.

- Modos de actualización configurables (Replace, Merge, etc.).

- Soporte para separadores personalizados.

- Opción de guardado local (Application.persistentDataPath).

- Modo de depuración para mostrar logs detallados.

🔹 LocalizationController.cs

- Controlador centralizado para el sistema de localización.

- Permite cambiar idioma, guardar preferencia en PlayerPrefs y obtener la lista de idiomas disponibles.

- Métodos estáticos para traducir términos y cambiar idioma dinámicamente.

- Evento OnLanguageChanged para actualizar la UI al cambiar el idioma.

- Cambio de idioma rápido en el editor (tecla F12).

🧰 Requisitos

1) Unity 2021.3+ (compatible también con versiones superiores).

2) Plugin I2 Localization instalado en el proyecto.

3) Permisos de acceso a la hoja de cálculo si usas Google Sheets como fuente de CSV.

⚙️ Configuración paso a paso

1. Instala el plugin I2 Localization desde la Unity Asset Store.

2. Copia los scripts LocalizationCSVLoader.cs y LocalizationController.cs en tu carpeta Assets/Scripts/Localization.

3. En tu escena principal, crea un GameObject vacío llamado LocalizationManager y añade los dos componentes:

    LocalizationCSVLoader

    LocalizationController

4. Asigna en el LocalizationCSVLoader el campo Target I2 Localization Asset (LanguageSourceAsset) de tu proyecto.

5. Introduce los valores de:

    Spreadsheet ID  (de tu Google Sheet)

    Spreadsheet GID (de la pestaña específica)

6. Ejecuta la escena: el script descargará e importará automáticamente las traducciones.

Puedes usar el LocalizationController para cambiar idioma en tiempo real o traducir términos en código:

LocalizationController.Instance.SetLanguage("Spanish");
string text = LocalizationController.Instance.Translate("Menu_Play");

🧩 Ejemplo de URL de Google Sheets

Formato de ejemplo utilizado en el script:

https://docs.google.com/spreadsheets/u/1/d/${id}/export?format=csv&id=${id}&gid=${gid}


Solo debes reemplazar:

${id} → ID de tu hoja de cálculo (lo que aparece entre /d/ y /edit).

${gid} → GUID de tu hoja de cálculo (lo que aparece tras #gid=)

🧠 Ejemplo práctico de uso

1. Crear una hoja en Google Sheets con las columnas:

Key | English | Spanish | French | Italian
myKey | Hello | Hola | Bonjour | Ciao


2. Publicarla en formato CSV (Archivo → Compartir → Cualquier persona con el enlace).

3. Copiar el ID y GID.

4. Configurar el LocalizationCSVLoader con esos valores.

Al iniciar, el sistema descargará el CSV e importará las traducciones a tu LanguageSourceAsset.

🪪 Créditos

Desarrollado por: Daniel Guerra Gallardo

Autor del plugin original: Inter Illusion

I2 Localization - Unity Asset Store

Licencia: MIT (solo para la extensión).
El uso del plugin I2 Localization requiere licencia válida del asset original.

🧱 Estructura del repositorio
      ├── LocalizationCSVLoader.cs
      └── LocalizationController.cs
📄 README.md
📄 LICENSE
