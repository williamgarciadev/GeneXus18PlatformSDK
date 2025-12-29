# Plan de Trabajo - Listar Form Class de WebPanels (Fixes Sintaxis)

## Objetivo
Corregir errores de sintaxis en `WebPanelService.cs` (escapado de strings y acceso a propiedades).

## Correcciones
- [x] **Strings Regex:** Se usan strings verbatim (`@"..."`) para simplificar y corregir las expresiones regulares, evitando errores `CS1009`.
- [x] **Acceso a Source:** Se usa el cast a `ISource` para acceder a `.Source` de forma segura, evitando `CS1061`.
- [x] **IPropertiesObject:** Se eliminó la dependencia de `IPropertiesObject` y se reemplazó por comprobaciones directas de `KBObject` y `KBObjectPart`, evitando `CS0246`.

## Estado
Listo para compilar y probar.