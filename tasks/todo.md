# Plan de Trabajo - Listar Form Class de WebPanels (Exportar a Excel)

## Objetivo
Mejorar la funcionalidad de listado de "Form Class" para exportar los resultados a un archivo Excel (.csv compatible) y solucionar el error de compilación.

## Tareas

- [x] Corregir error CS0246 incluyendo `Core/Application/Services/WebPanelService.cs` en `Menu.csproj`.
- [x] Refactorizar `WebPanelService.cs`:
    - [x] Crear estructura `WebPanelInfo`.
    - [x] Implementar lógica de generación de CSV con BOM UTF-8 y separador `;`.
    - [x] Implementar apertura automática del archivo generado.
- [x] Actualizar `CommandManager.cs` para llamar al nuevo método `ListFormClassPropertyAndExport`.
- [x] Verificar compilación (manualmente vía revisión de archivos).

## Detalles de Implementación
- Se usa CSV con encoding UTF-8 BOM y punto y coma (`;`) como separador para máxima compatibilidad con Excel en configuraciones regionales españolas/latinas.
- El archivo temporal se crea en `%TEMP%` y se lanza `Process.Start` para que el SO lo abra con la aplicación predeterminada (Excel).
