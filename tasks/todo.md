# Plan de Trabajo - Listar Form Class de WebPanels

## Objetivo
Implementar una opción en el menú "Commands" que liste la propiedad "Form Class" de todos los WebPanels de la KB.

## Tareas

- [x] Analizar el código existente para entender cómo iterar objetos y obtener propiedades.
- [x] Crear servicio `Core/Application/Services/WebPanelService.cs` con la lógica de extracción.
- [x] Actualizar `Common/Factories/ServiceFactory.cs` para inyectar el nuevo servicio.
- [x] Definir la clave del comando en `Commands/CommandKeys.cs`.
- [x] Agregar el recurso de texto en `Resources/ResourcesV1.resx`.
- [x] Registrar el comando en `Commands/Menu.package`.
- [x] Implementar el handler en `Commands/CommandManager.cs`.
- [x] Verificar la compilación (Skipped due to missing msbuild).

## Detalles de Implementación
- Se busca la propiedad "FormClass", "ThemeClass" o "Class" tanto en el objeto WebPanel como en su parte WebForm.
- La salida se dirige al Output de GeneXus usando `ILogger`.
