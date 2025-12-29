# Plan de Trabajo - Listar Form Class de WebPanels (Fixes)

## Objetivo
Corregir la falta de descripción en el menú y el reporte en blanco (CSV vacío).

## Tareas

- [x] Corregir Recurso de Texto:
    - [x] Mover `CmdListWebPanelFormClass_Name` de `ResourcesV1.resx` a `Resources.resx` (ya que `Menu.package` apunta a `Resources`).
    - [x] Revertir cambio en `ResourcesV1.resx`.
- [x] Robustecer `WebPanelService.cs`:
    - [x] Usar comparación por nombre de tipo (`obj.TypeDescriptor.Name == "WebPanel"`) para evitar problemas de casting.
    - [x] Mejorar la búsqueda de `WebFormPart` usando `Get("WebForm")` genérico.
    - [x] Asegurar codificación UTF-8 con BOM explícito en la exportación.
    - [x] Agregar logging detallado del progreso.
- [x] Verificar compilación.

## Detalles de Implementación
- Se centralizaron los recursos en `Resources.resx`.
- Se usa `UTF8Encoding(true)` para forzar el BOM, asegurando que Excel abra el CSV con los caracteres correctos.