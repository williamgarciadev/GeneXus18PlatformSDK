
## 🌟 Nuevas Funcionalidades Implementadas (Diciembre 2025)

### 1. **Corrección Robusta de "Ir a Subrutina"**
Se solucionó un problema crítico donde el comando "Ir a Subrutina" fallaba al no detectar correctamente la selección o el contexto del editor.

*   **Detección de Selección Mejorada**: Implementación de `GetSelectedTextReflected` que utiliza Reflection para acceder a la propiedad `SelectedText` del editor subyacente, incluso a través de capas de editores virtuales (Events, Rules).
*   **Fallback a Portapapeles**: Si la API del editor falla, se intenta capturar el texto bajo el cursor simulando un comando de copia al portapapeles.
*   **Estabilidad UI**: Se añadieron comprobaciones de nulidad defensivas para `UIServices.DocumentManager`, `UIServices.Environment` y `UIServices.EditorManager`, evitando cierres inesperados (NullReferenceException).

### 2. **Buscador de Objetos No Referenciados**
Inspirado en herramientas como KBDoctor, se añadió una funcionalidad para detectar "código muerto" a nivel de objeto.

*   **Servicio de Análisis (`UnreferencedObjectsService`)**:
    *   Identifica todos los objetos `Main` de la KB.
    *   Realiza un recorrido del grafo de llamadas (Call Graph Traversal) para identificar todos los objetos alcanzables.
    *   Reporta aquellos objetos (Procedimientos, WebPanels, Transacciones) que no son invocados por ningún proceso principal.
*   **Integración**: Nuevo comando `CmdFindUnreferencedObjects` accesible desde el menú.

### 3. **Limpiador de Variables Locales**
Implementación completa de la fase de limpieza de variables.

*   **Análisis Inteligente**: Escanea Source, Rules, Events y Conditions para determinar el uso real de variables.
*   **Lista Blanca**: Protege variables de sistema (`Pgmname`, `Time`, etc.) para evitar falsos positivos.
*   **Acción Directa**: Elimina automáticamente las variables no utilizadas del objeto activo.
