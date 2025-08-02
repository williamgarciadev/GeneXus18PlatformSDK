# GeneXus 18 Platform SDK - Menu Plugin

## 📋 Descripción

Plugin de extensión para GeneXus 18 que proporciona herramientas adicionales de desarrollo y depuración. Se integra como un plugin dentro del IDE de GeneXus para facilitar el trabajo con variables, logging y análisis de objetos.

## 🏗️ Arquitectura

Este proyecto ha sido completamente refactorizado siguiendo los principios de **Clean Code** de Robert C. Martin:

### ✨ Características Principales

- **Extracción Inteligente de Variables**: Conversión automática de texto a variables GeneXus con tipos correctos
- **Generación de Código Debug**: Creación automática de líneas de logging y debug
- **Exportación de Historial**: Exportación completa del historial de objetos de la KB
- **Análisis de Estructura**: Exportación de estructuras de tablas y análisis de código

### 🏛️ Arquitectura Modular

```
MenuV2/
├── Commands/                    # Comandos principales
├── Infrastructure/              # Utilidades base y reflexión
├── Services/                    # Servicios especializados
│   ├── Export/                 # Servicios de exportación
│   ├── Variables/              # Manejo de variables
│   └── Analysis/               # Análisis de código
├── Models/                     # Modelos de datos
├── Formatters/                 # Formateadores de salida
├── UI/                         # Interfaces de usuario
└── Utilities/                  # Utilidades heredadas
```

## 🚀 Instalación

### Prerrequisitos

- GeneXus 18 Platform SDK
- .NET Framework 4.7.2
- Visual Studio 2017 o superior

### Compilación

```bash
# Restaurar paquetes NuGet
nuget restore Menu.sln

# Compilar proyecto
msbuild Menu.sln /p:Configuration=Release

# El post-build automáticamente instala en GeneXus
```

## 📊 Principios Clean Code Aplicados

### ✅ Transformación Realizada

- **ANTES**: CommandManager monolítico con +1000 líneas
- **DESPUÉS**: 12 clases especializadas con 230 líneas en el manager principal

### 🎯 Principios Implementados

- **Single Responsibility Principle (SRP)**: Cada clase tiene una única responsabilidad
- **Open/Closed Principle (OCP)**: Abierto para extensión, cerrado para modificación
- **Dependency Inversion Principle (DIP)**: Dependencias por abstracción
- **DRY**: Eliminación de código duplicado
- **Clean Functions**: Funciones pequeñas y enfocadas
- **Meaningful Names**: Nombres descriptivos y auto-documentados

## 🔧 Comandos Disponibles

| Comando | Descripción | Atajo |
|---------|-------------|-------|
| **Generate Log Debug Form** | Genera formulario de debug | Ctrl+Shift+H |
| **Show Object History** | Exporta historial de objetos | Ctrl+Shift+G |
| **Extract Procedure Variables** | Extrae variables de procedimiento | Menú contextual |
| **Extract Smart Variable** | Extrae variable inteligente | Menú contextual |
| **Export Table Structure** | Exporta estructura de tablas | Ctrl+Shift+T |
| **Export Procedure Source** | Exporta código fuente | Ctrl+Shift+P |

## 🛡️ Compatibilidad

- **Framework**: .NET Framework 4.7.2
- **Lenguaje**: C# 7.3 (compatible con VS 2017)
- **GeneXus**: Platform SDK 18
- **Tipos Soportados**: Solo tipos escalares compatibles con GeneXus

## 📈 Métricas de Mejora

| Métrica | Antes | Después | Mejora |
|---------|--------|---------|---------|
| **Líneas CommandManager** | 1000+ | 230 | -77% |
| **Clases Especializadas** | 1 | 12 | +1100% |
| **Mantenibilidad** | Baja | Alta | ⬆️ |
| **Testabilidad** | Difícil | Fácil | ⬆️ |

## 🤝 Contribución

Este proyecto sigue estrictos estándares de Clean Code. Para contribuir:

1. Mantener principios SOLID
2. Funciones pequeñas y enfocadas (< 20 líneas)
3. Nombres descriptivos y auto-documentados
4. Un solo nivel de abstracción por función
5. Compatibilidad C# 7.3 y .NET Framework 4.7.2

## 📄 Licencia

Este proyecto es parte del GeneXus 18 Platform SDK.

---

**Refactorización Clean Code completada** ✨  
*Transformación de código legacy a arquitectura moderna y mantenible*