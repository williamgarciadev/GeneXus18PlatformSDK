# Plan de Trabajo - Listar Form Class de WebPanels (Fixes Finales)

## Objetivo
Solucionar errores de compilación y mejorar la extracción de la propiedad "Form Class".

## Correcciones
- [x] **Error CS1061 (WebFormPart.Layout):** Se eliminó la dependencia de `.Layout` y se reemplazó por un análisis directo del código fuente (`.Source`) usando expresiones regulares (Regex). Esto es más compatible y menos propenso a errores de versión del SDK.
- [x] **Errores CS8370 (Strings):** Se eliminaron los literales de cadena modernos (`"""`) y se reemplazaron por strings estándar compatibles con C# 7.3 / .NET 4.7.
- [x] **Lógica de Extracción:** Se implementó una búsqueda de patrones XML/HTML (`Name="FormClass" Value="..."`) para encontrar la propiedad incluso si no está expuesta directamente en el objeto de API.

## Estado
Listo para compilar y probar.
