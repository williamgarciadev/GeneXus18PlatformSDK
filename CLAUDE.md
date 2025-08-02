# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Proyecto Overview

Este es un package de extensión para GeneXus 18 que proporciona herramientas adicionales de desarrollo y depuración. Se integra como un plugin dentro del IDE de GeneXus para facilitar el trabajo con variables, logging y análisis de objetos.

## Arquitectura Principal

### Plugin Structure
- **Namespace**: `Acme.Packages.Menu` (definido en múltiples archivos)
- **Assembly Target**: .NET Framework 4.7.2 (library)
- **Package GUID**: `163faefb-1d07-4a23-acca-5f287020bcac`
- **Main Package Class**: `MenuPackage : AbstractPackageUI`

### Core Components
1. **CommandManager**: Gestor central de comandos del plugin
2. **VariableHelper**: Lógica para manejo inteligente de variables GeneXus  
3. **Utils**: Utilidades para interacción con editor y UI
4. **ExtractorTablasGX**: Exportación de estructuras de tablas

### Key Features
- **Variable Extraction**: Conversión automática de texto a variables GeneXus con tipos correctos
- **Debug Logging**: Generación automática de código de debug/logging
- **Object History**: Exportación del historial de objetos de la KB
- **Table Structure Export**: Exportación de estructuras de tablas

## Build Commands

```bash
# Build solution
msbuild Menu.sln /p:Configuration=Debug
msbuild Menu.sln /p:Configuration=Release

# Build specific project  
msbuild Menu.csproj /p:Configuration=Debug

# Restore NuGet packages
nuget restore Menu.sln
```

## Development Setup

### Required GeneXus SDK
- GeneXus 18 Platform SDK
- Referencias a DLLs en `..\..\Bin\` (Artech.* assemblies)
- KBDoctor DLLs en `DllKbDoctor\`

### Dependencies
- **Framework**: .NET Framework 4.7.2
- **External Libraries**: LSI.Packages.Extensiones.Utilidades
- **NuGet Packages**: Newtonsoft.Json, System.Text.Json, System.IO.Pipelines

### Post-Build Process
El proyecto tiene un evento post-build que actualiza automáticamente el catálogo de GeneXus:
```
"$(GX_SDK_DIR)\Tools\Updater" ..\..\Resources\Catalog.xml ..\..\ "$(GX_PROGRAM_DIR_18_U10)"\ $(Configuration)
"$(GX_PROGRAM_DIR_18_U10)"\Genexus /install
```

## Core Architecture Patterns

### Command Pattern Implementation
- Cada funcionalidad se implementa como comandos (`CommandKeys.cs`)
- `CommandManager` registra handlers para exec/query de cada comando
- Comandos disponibles:
  - `WGExtractVariable`: Extracción inteligente de variables
  - `WGExtractProcedure`: Generación de código de logging
  - `ShowObjectHistory`: Exportar historial de objetos
  - `CmdExportTableStructure`: Exportar estructura de tablas

### Variable Type Resolution
El sistema resuelve tipos de variables usando dos estrategias:
1. **Prefix-based**: Primer carácter determina tipo (ej: `s` = string, `n` = numeric)
2. **Reference-based**: Usa nombre antes de `_` como referencia a atributo/dominio

### GeneXus Integration Points
- **VariablesPart**: Acceso a variables de objetos GeneXus
- **KBObjectPart**: Contexto del objeto siendo editado  
- **UIServices**: Integración con servicios del IDE
- **EditorService**: Manipulación del editor de código

## Testing Commands

No hay un framework de testing específico configurado. Para testing manual:

1. Build del proyecto
2. Ejecutar post-build para instalar en GeneXus
3. Abrir GeneXus y verificar que el menú aparece
4. Probar cada comando desde el menú contextual

## Patterns específicos para GeneXus

### Compatibilidad con Genexus
- Solo tipos escalares: `string`, `int`, `bool`, `decimal`, `DateTime`
- Sin arrays, matrices, enums o generics
- Métodos NO lanzan excepciones - usan try-catch interno
- Errores devueltos en objeto resultado
- Constructor público sin parámetros requerido

### Variable Naming Conventions
- Variables GeneXus empiezan con `&`
- Prefijos de tipo: primer carácter indica tipo de dato
- Referencias: formato `referencia_nombreVariable`

### UI Integration Patterns
- `Utils.ShowWarning/ShowError/ShowInfo`: Mensajes consistentes
- `Utils.Log`: Output a ventana de salida de GeneXus
- `Utils.GetSelectedTextSafe`: Obtener texto seleccionado del editor
- `Utils.ReplaceSelectedTextInEditor`: Reemplazar texto en editor

## Key Classes to Understand

### CommandManager.cs
- Registro central de todos los comandos del plugin
- Implementa handlers para ejecución y habilitación de comandos
- Contiene lógica de negocio principal de cada feature

### VariableHelper.cs
- Resolución inteligente de tipos de variables
- Integración con KB de GeneXus para buscar atributos/dominios
- Creación automática de variables en la parte correspondiente

### Utils.cs
- Utilidades para interacción con el IDE
- Manejo de clipboard y editor
- Logging y mensajes de usuario
- Generación de código de debug

### ProcedureSourceExtractor.cs
- Exportación completa de código fuente de procedimientos
- Análisis de variables, propiedades y métricas de código
- Generación de JSON estructurado con información detallada
- Manejo robusto de errores por procedimiento individual

## Package Configuration

### Menu.package Structure
El archivo `Commands/Menu.package` define la integración UI con GeneXus:

**Comandos Registrados:**
- `CmdGenerateLogDebugForm` - Generar formulario de debug (Ctrl+Shift+H)
- `ShowObjectHistory` - Mostrar historial de objetos (Ctrl+Shift+G)  
- `WGExtractProcedure` - Extraer variables de procedimiento (menú contextual)
- `WGExtractVariable` - Extraer variable inteligente (menú contextual)
- `CmdExportTableStructure` - Exportar estructura de tablas (Ctrl+Shift+T)
- `CmdExportProcedureSource` - Exportar código fuente de procedimientos (Ctrl+Shift+P)

**Grupos de Menú:**
- `MainCommandsGroup`: Comandos en menú principal "Commands"
- `ContextCommandsGroup`: Comandos en menú contextual del editor

### Resources & Localization
- Strings: `Acme.Packages.Menu.Resources`
- Images: Iconos en `Resources/Images/`
- Package ID: `163faefb-1d07-4a23-acca-5f287020bcac`

## Specific Features Deep Dive

### Variable Extraction System
**Dos modos de resolución:**
1. **Prefix Mode**: `sVariable` → tipo String basado en primer carácter
2. **Reference Mode**: `Attribute_NewVar` → tipo basado en atributo/dominio existente

**Proceso de extracción:**
```csharp
// 1. Obtener texto seleccionado
string selectedText = Utils.GetSelectedTextSafe(commandData);

// 2. Determinar tipo y longitud
(eDBType variableType, int length, bool isBasedOnAttributeOrDomain) = 
    VariableHelper.GetTypeAndLengthFromReference(baseReference);

// 3. Crear variable en KB
VariableHelper.AddVariableBasedOn(variableName, currentPart, baseReference, commandData);

// 4. Reemplazar texto en editor
Utils.ReplaceSelectedTextInEditor(commandData, selectedText, newVariableName);
```

### Debug Code Generation
**Dos formatos soportados:**
- **Log.Debug**: `Log.Debug("Variable: " + &Variable.ToString())`
- **msg**: `msg("Variable: " + &Variable.ToString())`

**Forms UI:**
- `VariablesInputForm`: Input de múltiples variables
- `LogDebugResultForm`: Mostrar código generado
- Soporte para output type selection

### Table Structure Export
**Exporta estructura completa de transacciones:**
```json
{
  "Transaccion": "NombreTransaccion",
  "Descripcion": "Descripción",
  "Campos": [
    {
      "Nombre": "AttributeName",
      "Tipo": "VarChar", 
      "Longitud": 50,
      "Decimales": 0,
      "EsClave": true,
      "EsForanea": false,
      "PermiteNulos": false,
      "Dominio": "DomainName",
      "Descripcion": "Campo descripción",
      "EsImagen": false
    }
  ]
}
```

### Object History Export
Exporta historial completo de objetos KB a CSV:
- Columnas: Objeto, Tipo, Fecha, Usuario, Operación, Versión
- Formato: `HistorialObjetos.csv` en Desktop
- Incluye todos los tipos de objetos con historial disponible

### Procedure Source Code Export
**Exporta código fuente completo de todos los procedimientos:**
```json
{
  "FechaExportacion": "2024-01-15 14:30:00",
  "TotalProcedimientos": 150,
  "KnowledgeBase": "MiKB",
  "Modelo": "MiModelo",
  "Procedimientos": [
    {
      "Nombre": "ProcedureName",
      "Descripcion": "Descripción del procedimiento",
      "GUID": "guid-string",
      "FechaCreacion": "2024-01-10 10:00:00",
      "Modulo": "ModuleName",
      "SourceCode": "// Código fuente completo del procedimiento",
      "Rules": "// Reglas si existen",
      "Variables": [
        {
          "Nombre": "VariableName",
          "Tipo": "VarChar",
          "Longitud": 50,
          "Decimales": 0,
          "Descripcion": "Variable description",
          "BasadoEn": "AttributeName",
          "EsStandard": false,
          "Firmado": false
        }
      ],
      "Propiedades": {
        "EsMain": true,
        "EsGenerado": false,
        "PuedeEjecutarse": true,
        "ProtocoloLlamada": "HTTP"
      },
      "Metricas": {
        "LineasCodigo": 125,
        "NivelComplejidad": 8,
        "MaxBloquesCodigo": 45,
        "MaxNivelAnidamiento": 3
      }
    }
  ]
}
```

**Características:**
- Exporta todos los procedimientos de la KB actual
- Incluye código fuente completo, reglas y variables
- Métricas de complejidad de código automatizadas
- Formato JSON estructurado y legible
- Archivo guardado en Desktop con timestamp
- Manejo robusto de errores por procedimiento

## UI Components

### Forms Architecture
- **VariablesInputForm**: Input manual de variables con ComboBox para tipo output
- **LogDebugResultForm**: Display de resultados con scroll
- **Form1**: Form base (no especificado uso)

### Editor Integration
- **Selection-based commands**: Trabajan con texto seleccionado
- **Context menu integration**: Comandos disponibles en menú contextual
- **Real-time text replacement**: Reemplazo inmediato en editor

## Error Handling Patterns

### Consistent Error Display
```csharp
// Warning messages
Utils.ShowWarning("Mensaje", "Título");

// Error messages  
Utils.ShowError("Error message");

// Info messages
Utils.ShowInfo("Info message", "Título");

// Output window logging
Utils.Log("Debug message");
```

### Exception Handling Strategy
- Métodos principales usan try-catch completo
- Errores se logean a Output window
- UI feedback inmediato al usuario
- No se lanzan excepciones hacia GeneXus

## Working with GeneXus APIs

### Accessing Current Context
```csharp
var model = UIServices.KB.CurrentModel;
var kb = model.KB;
KBObjectPart currentPart = Entorno.CurrentEditingPart;
```

### Variable Management
```csharp
VariablesPart variablesPart = currentPart.KBObject.Parts.Get<VariablesPart>();
Variable newVar = new Variable(variableName, variablesPart) { Type = type, Length = length };
variablesPart.Add(newVar);
```

### Transaction Structure Access
```csharp
var transacciones = model.GetObjects<Transaction>().ToList();
foreach (var trx in transacciones)
{
    var estructura = trx.Structure?.Root;
    foreach (var attr in estructura.Attributes)
    {
        var atributo = attr.Attribute;
        // Access: attr.Name, atributo.Type, atributo.Length, etc.
    }
}
```

### Object History Queries
```csharp
foreach (Guid objType in KnowledgeBase.GetKBObjectTypes())
{
    IEnumerable<Entity> entities = kb.GetEntitiesByModelTypeOrderByName(model, objType);
    foreach (Entity entity in entities)
    {
        var obj = KBObject.Get(model, entity.Key);
        var historyList = kb.GetModelEntityHistoryByModelEntityKey(model, obj.Key);
        // Process history records
    }
}
```

### Saving Changes
```csharp
KBObjectSavePreferences savePreferences = new KBObjectSavePreferences(KBObjectSavePreferences.ForcedSave)
{
    SkipValidation = false
};
currentPart.KBObject.Save(savePreferences);
```

## Common Development Workflows

### Adding New Commands
1. Definir CommandKey en `CommandKeys.cs`
2. Registrar handler en `CommandManager` constructor
3. Implementar métodos `Exec*` y `Query*`
4. Actualizar `Menu.package` con definición UI
5. Agregar recursos de texto si necesario

### Variable Type Resolution Flow
1. Check if variable contains underscore (`_`)
2. If yes: Use prefix as attribute/domain reference
3. If no: Use first character as type prefix
4. Resolve type through `VariableHelper.GetTypeAndLengthFromReference()`
5. Create variable with resolved type and length

## Principios de Código Limpio

Basado en las prácticas de Robert C. Martin, este proyecto sigue los estándares de código limpio para mantener un desarrollo ágil y sostenible.

### Reglas Fundamentales

#### 1. Claridad sobre Brevedad
- **Nombres descriptivos**: El código debe leerse como prosa
- **Funciones pequeñas**: Una función = una responsabilidad
- **Comentarios mínimos**: El código se explica a sí mismo

#### 2. Principio DRY (Don't Repeat Yourself)
- No duplicar lógica
- Extraer código común a funciones/módulos
- Reutilizar antes que reescribir

#### 3. Principio KISS (Keep It Simple, Stupid)
- Solución más simple que funcione
- Evitar sobre-ingeniería
- Refactorizar cuando sea necesario

#### 4. Principio YAGNI (You Aren't Gonna Need It)
- No implementar funcionalidad especulativa
- Desarrollar solo lo requerido
- Iterar basado en necesidades reales

### Estándares de Calidad

#### Estructura de Código
- Funciones máximo 20-30 líneas
- Clases con responsabilidad única
- Módulos cohesivos y bajo acoplamiento
- Jerarquía de directorios lógica

#### Manejo de Errores
- Fallar rápido y de forma explícita
- Errores específicos y descriptivos
- Logging apropiado para debugging
- Recuperación elegante cuando sea posible

#### Testing
- Tests como documentación ejecutable
- Cobertura en funcionalidad crítica
- Tests unitarios rápidos y aislados
- TDD cuando sea apropiado

### Prácticas de Desarrollo

#### Code Review
- Todo código pasa por revisión
- Feedback constructivo y específico
- Enfoque en legibilidad y mantenibilidad
- Validación de estándares establecidos

#### Refactoring Continuo
- Mejorar diseño sin cambiar comportamiento
- Pequeñas mejoras constantes
- Eliminar código muerto
- Simplificar complejidad innecesaria

#### Documentación
- README actualizado y útil
- Comentarios solo cuando el "por qué" no es obvio
- APIs documentadas
- Decisiones arquitectónicas registradas

### Herramientas de Soporte

#### Automatización de Calidad
- Linters configurados
- Formateo automático
- Hooks de pre-commit
- Pipeline de CI/CD con validaciones

#### Métricas
- Complejidad ciclomática controlada
- Duplicación de código monitoreada
- Cobertura de tests medida
- Deuda técnica visible

### Filosofía de Equipo

> "Cualquier tonto puede escribir código que una computadora entienda. Los buenos programadores escriben código que los humanos pueden entender." - Martin Fowler

- **Código para humanos**: Optimizar para legibilidad
- **Mejora continua**: Cada commit deja el código mejor
- **Responsabilidad colectiva**: Todo el equipo es dueño del código
- **Pragmatismo**: Balance entre perfección y entrega