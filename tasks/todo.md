# Plan de Implementación: Generador de Documentación Automática (Markdown)

El objetivo es crear una funcionalidad que permita generar documentación técnica de un objeto GeneXus seleccionado (inicialmente Procedimientos y Transacciones) en formato Markdown.

## ✅ Fase 1: Core & Dominio (Clean Architecture)
- [x] **Definir DTOs**: Crear `Core/Domain/DTOs/ObjectDocumentationDto.cs` (Nombre, Descripción, Parámetros, Variables, etc.). <!-- id: 1 -->
- [x] **Definir Interfaces**: Crear `Core/Domain/Interfaces/IDocumentationFormatter.cs` e `IDocumentationService.cs`. <!-- id: 2 -->

## ⚙️ Fase 2: Lógica de Aplicación e Infraestructura
- [x] **Implementar Formateador**: Crear `Core/Infrastructure/Formatters/MarkdownDocumentationFormatter.cs` que convierta el DTO a string Markdown. <!-- id: 3 -->
- [x] **Implementar Servicio de Extracción**: Crear `Core/Application/Services/DocumentationService.cs` que use la API de GeneXus para poblar el DTO. <!-- id: 4 -->
    - Extraer nombre y descripción.
    - Parsear reglas para encontrar `parm(...)`.
    - Listar variables (reusar lógica existente si es posible).

## 🎨 Fase 3: UI e Integración
- [x] **Crear Formulario de Vista Previa**: Crear `UI/Forms/DocumentationPreviewForm.cs` (TextBox multilínea, Botón Copiar, Botón Guardar). <!-- id: 5 -->
- [x] **Registrar Comando**: Actualizar `Package.cs` y `CommandManager.cs` para agregar el comando "Generate Markdown Docs". <!-- id: 6 -->

## 🏁 Fase 4: Revisión
- [x] **Pruebas Manuales**: Verificar la generación con un Procedimiento y una Transacción. <!-- id: 7 -->
- [x] **Refactorización Final**: Asegurar que todo cumpla con SOLID y limpiar código. <!-- id: 8 -->

## 🧹 Fase 5: Limpiador de Variables Locales (NUEVO)
- [ ] **Definir Interfaz**: Crear `Core/Domain/Interfaces/IVariableCleanerService.cs`. <!-- id: 9 -->
- [ ] **Implementar Lógica**: Crear `Core/Application/Services/VariableCleanerService.cs`. <!-- id: 10 -->
    - Escanear Source, Rules y Events.
    - Filtrar variables estándar y de sistema.
    - Eliminar variables sin referencias.
- [ ] **Integrar Comando**: Agregar "Limpiar Variables No Usadas" al menú contextual. <!-- id: 11 -->

## 🔍 Fase 6: Rastreador de Variable (Variable Tracer)
- [x] **Diseñar UI de Rastreo**: Crear formulario para mostrar ocurrencias. <!-- id: 12 -->
- [x] **Implementar Buscador**: Lógica para clasificar Lectura/Escritura. <!-- id: 13 -->
