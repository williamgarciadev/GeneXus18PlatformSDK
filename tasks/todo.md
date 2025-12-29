# Plan de Trabajo - Diagnóstico de WebPanel Form Class

## Objetivo
Agregar un diagnóstico detallado para entender por qué la propiedad "Form Class" aparece vacía.

## Tareas
- [x] **Implementar Modo Debug:**
    - Al ejecutar el comando, para el **primer WebPanel encontrado**, se imprimirá en el Output de GeneXus:
        - Todas las propiedades del Objeto relacionadas con "Class".
        - Todas las propiedades de la parte WebForm relacionadas con "Class".
        - Una muestra del código fuente (XML/HTML) del layout.
        - Resultados de pruebas de Regex en tiempo real.
    - Esto permitirá ver el nombre exacto de la propiedad interna.
- [x] **Mantener Compatibilidad:** El código sigue siendo compatible con C# 7.3.

## Estado
Listo para compilar y ejecutar diagnóstico.