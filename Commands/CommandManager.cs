using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Framework.Commands;
using Acme.Packages.Menu.Services.Export;
using Acme.Packages.Menu.Services.Variables;
using Acme.Packages.Menu.Core.Application.Services;
using Acme.Packages.Menu.Utilities;
using Artech.Architecture.UI.Framework.Helper;
using Acme.Packages.Menu.Common.Factories;
using Acme.Packages.Menu.Core.Domain.DTOs;
using Acme.Packages.Menu.UI.Forms;
using Artech.Architecture.Common.Objects;
using Artech.Architecture.UI.Framework.Objects;
using Artech.Common.Framework.Selection;
using Artech.Genexus.Common.Parts;
using System.Windows.Forms;
using Artech.Architecture.UI.Framework.Editors;

namespace Acme.Packages.Menu
{
    class CommandManager : CommandDelegator
    {
        public CommandManager()
        {
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            AddCommand(CommandKeys.CmdGenerateLogDebugForm, new ExecHandler(ExecGenerateLogDebugFormCommand), new QueryHandler(QueryGenerateLogDebugFormCommand));
            AddCommand(CommandKeys.WGExtractVariable, new ExecHandler(ExecWGExtractVariableCommand), new QueryHandler(QueryAlwaysEnabled));
            AddCommand(CommandKeys.WGExtractProcedure, new ExecHandler(ExecWGExtractProcedureCommand), new QueryHandler(QueryAlwaysEnabled));
            AddCommand(CommandKeys.ShowObjectHistory, new ExecHandler(ExecShowObjectHistory), new QueryHandler(QueryAlwaysEnabled));
            AddCommand(CommandKeys.CmdExportTableStructure, new ExecHandler(ExecExportTableStructure), new QueryHandler(QueryAlwaysEnabled));
            AddCommand(CommandKeys.CmdExportProcedureSource, new ExecHandler(ExecExportProcedureSource), new QueryHandler(QueryAlwaysEnabled));
            AddCommand(CommandKeys.CmdCountCodeLines, new ExecHandler(ExecCountCodeLines), new QueryHandler(QueryAlwaysEnabled));
            AddCommand(CommandKeys.CmdExportObjectsWithSourceLines, new ExecHandler(ExecExportObjectsWithSourceLines), new QueryHandler(QueryAlwaysEnabled));
            AddCommand(CommandKeys.CmdGenerateMarkdownDocs, new ExecHandler(ExecGenerateMarkdownDocs), new QueryHandler(QueryGenerateLogDebugFormCommand));
            AddCommand(CommandKeys.CmdCleanUnusedVariables, new ExecHandler(ExecCleanUnusedVariables), new QueryHandler(QueryGenerateLogDebugFormCommand));
            AddCommand(CommandKeys.CmdSmartFixVariables, new ExecHandler(ExecSmartFixVariables), new QueryHandler(QueryGenerateLogDebugFormCommand));
            AddCommand(CommandKeys.CmdTraceVariable, new ExecHandler(ExecTraceVariable), new QueryHandler(QueryAlwaysEnabled));
            AddCommand(CommandKeys.CmdGoToSubroutine, new ExecHandler(ExecGoToSubroutine), new QueryHandler(QueryAlwaysEnabled));
            AddCommand(CommandKeys.CmdBackFromGoTo, new ExecHandler(ExecBackFromGoTo), new QueryHandler(QueryAlwaysEnabled));
            AddCommand(CommandKeys.CmdFindUnreferencedObjects, new ExecHandler(ExecFindUnreferencedObjects), new QueryHandler(QueryGenerateLogDebugFormCommand));
            AddCommand(CommandKeys.CmdInventoryExternalObjects, new ExecHandler(ExecInventoryExternalObjects), new QueryHandler(QueryGenerateLogDebugFormCommand));
            AddCommand(CommandKeys.CmdListWebPanelFormClass, new ExecHandler(ExecListWebPanelFormClass), new QueryHandler(QueryGenerateLogDebugFormCommand));
        }

        private bool ExecListWebPanelFormClass(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var service = ServiceFactory.GetWebPanelService();
                service.ListFormClassProperty();
            }, "listar form class webpanels");
        }

        private bool ExecInventoryExternalObjects(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var service = new InventoryService();
                string csvContent = service.GetExternalObjectsCsv();

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = $"ExternalObjects_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, csvContent);
                    Utils.ShowInfo("Inventario generado exitosamente.", "Inventario");
                }
            }, "inventario external objects");
        }

        private void SaveCurrentNavigationState(CommandData commandData)
        {
            try
            {
                // Método HÍBRIDO para obtener el objeto y la parte actual
                KBObject obj = GetObjectFromContext(commandData);
                KBObjectPart part = null;
                try { part = LSI.Packages.Extensiones.Utilidades.Entorno.CurrentEditingPart; } catch { }
                
                if (obj == null && part != null) obj = part.KBObject;
                if (obj == null) return;

                var textExtractor = ServiceFactory.GetEditorTextExtractor();
                var pos = textExtractor.GetCursorPosition(commandData);
                int line = pos?.Line ?? 1;

                string partName = part?.Name ?? "";
                if (string.IsNullOrEmpty(partName) && part != null) partName = part.TypeDescriptor?.Name ?? "";

                Utils.Log($"📍 Historial: Guardando {obj.Name} ({partName}) Ln: {line}");
                Utils.PushNavigation(obj, partName, line);
            }
            catch (Exception ex) { Utils.Log("⚠️ Error guardando historial: " + ex.Message); }
        }

        private bool ExecBackFromGoTo(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                // 1. DETECCIÓN CONTEXTUAL: ¿Estamos sobre una definición de subrutina?
                var textExtractor = ServiceFactory.GetEditorTextExtractor();
                string currentLineText = textExtractor.GetCurrentLine(commandData);
                
                if (!string.IsNullOrEmpty(currentLineText) && currentLineText.Trim().ToLower().StartsWith("sub "))
                {
                    string subName = ServiceFactory.GetSubroutineNavigatorService().CleanSubroutineName(currentLineText);
                    if (!string.IsNullOrEmpty(subName))
                    {
                        Utils.Log($"🔍 Buscando llamador 'Do' para la subrutina actual: '{subName}'...");
                        KBObject currentObject = GetObjectFromContext(commandData);
                        if (currentObject != null)
                        {
                            foreach (KBObjectPart part in currentObject.Parts)
                            {
                                string source = GetPartSource(part);
                                int callerLine = ServiceFactory.GetSubroutineNavigatorService().FindCallerLine(source, subName);
                                if (callerLine > 0)
                                {
                                    Utils.Log($"🔙 Volviendo al llamador (Do) en {part.Name} Ln: {callerLine}");
                                    JumpToLine(currentObject, new VariableOccurrenceDto { LineNumber = callerLine, PartName = part.Name });
                                    return;
                                }
                            }
                        }
                        Utils.ShowWarning($"No se encontró ninguna llamada (Do) para la subrutina '{subName}' en este objeto.", "Volver");
                        return; // IMPORTANTE: Si estamos en un Sub, NO usamos la pila si falla la búsqueda.
                    }
                }

                // 2. Si NO estamos en un Sub, intentar historial de navegación (Pila)
                var prev = Utils.PopNavigation();
                if (prev != null)
                {
                    Utils.Log($"🔙 Volviendo a historial: {prev.Item1.Name} ({prev.Item2}) Ln: {prev.Item3}");
                    Utils.NavigateToLine(prev.Item1, prev.Item2, prev.Item3);
                    return;
                }

                // 3. Fallback final: Comandos nativos
                Utils.Log("ℹ️ Sin contexto ni historial guardado, intentando comandos nativos...");
                if (Utils.TryInvokeBack()) return;

                Type baseKeysType = typeof(Artech.Architecture.UI.Framework.Commands.CommandKeys);
                foreach (Type container in baseKeysType.GetNestedTypes())
                {
                    if (container.Name.Equals("Wiki", StringComparison.OrdinalIgnoreCase)) continue;

                    string[] backCandidates = { "NavigateBackward", "NavigateBack", "Backward" };
                    foreach (string name in backCandidates)
                    {
                        PropertyInfo prop = container.GetProperty(name, BindingFlags.Public | BindingFlags.Static);
                        if (prop != null)
                        {
                            object val = prop.GetValue(null);
                            if (val is CommandKey key)
                            {
                                UIServices.CommandDispatcher.Dispatch(key);
                                Utils.Log($"✅ Volver atrás completado vía {container.Name}.{name}");
                                return;
                            }
                        }
                    }
                }
            }, "volver atrás");
        }

        private bool ExecFindUnreferencedObjects(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                if (UIServices.KB.CurrentModel == null) return;
                
                var service = ServiceFactory.GetUnreferencedObjectsService();
                Utils.Log("🔎 Buscando objetos no referenciados...");
                var unreferenced = service.GetUnreferencedObjects(UIServices.KB.CurrentModel);
                
                if (unreferenced.Count > 0)
                {
                    Utils.Log($"❌ Se encontraron {unreferenced.Count} objetos no referenciados.");
                    // Aquí podríamos mostrar un formulario con los resultados
                    // Por ahora solo logueamos
                    foreach (var obj in unreferenced)
                    {
                        Utils.Log($"   - {obj.TypeDescriptor.Name}: {obj.Name}");
                    }
                    Utils.ShowInfo($"Se encontraron {unreferenced.Count} objetos no referenciados. Ver Output.", "Limpieza KB");
                }
                else
                {
                    Utils.ShowInfo("✅ No se encontraron objetos no referenciados (relevantes).", "Limpieza KB");
                }
            }, "buscar objetos no referenciados");
        }

        private KBObject GetObjectFromContext(CommandData commandData)
        {
            KBObject found = null;
            if (commandData != null && commandData.Context != null)
            {
                object context = commandData.Context;
                found = ExtractKBObject(context);
                if (found == null && context is ISelectionContainer container && container.SelectedObjects != null)
                {
                    foreach (object selObj in container.SelectedObjects)
                    {
                        found = ExtractKBObject(selObj);
                        if (found != null) break;
                    }
                }
                if (found == null && context is System.Collections.IEnumerable selection)
                {
                    foreach (object selObj in selection)
                    {
                        found = ExtractKBObject(selObj);
                        if (found != null) break;
                    }
                }
            }
            if (found == null) found = ExtractKBObject(null);
            return found;
        }

        private KBObject ExtractKBObject(object obj)
        {
            if (obj != null)
            {
                if (obj is KBObject kbObj) return kbObj;
                if (obj is KBObjectPart part) return part.KBObject;
                if (obj is IGxDocument doc) return doc.Object;
            }
            try
            {
                KBObjectPart currentPart = LSI.Packages.Extensiones.Utilidades.Entorno.CurrentEditingPart;
                if (currentPart != null) return currentPart.KBObject;
            }
            catch (Exception) { }
            return null;
        }

        private bool ExecCleanUnusedVariables(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                KBObject currentObject = GetObjectFromContext(commandData);
                if (currentObject == null) { Utils.ShowError("No se pudo identificar el objeto."); return; }
                var cleaner = ServiceFactory.GetVariableCleanerService();
                int removed = cleaner.CleanUnusedVariables(currentObject);
                Utils.ShowInfo(removed > 0 ? "Se eliminaron " + removed + " variables." : "No se encontraron variables no utilizadas.", "Limpieza");
            }, "limpiar variables no usadas");
        }

        private bool ExecGenerateMarkdownDocs(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                KBObject currentObject = GetObjectFromContext(commandData);
                if (currentObject == null) { Utils.ShowError("No se pudo identificar el objeto."); return; }
                var docService = ServiceFactory.GetDocumentationService();
                var formatter = ServiceFactory.GetDocumentationFormatter();
                var docData = docService.ExtractDocumentation(currentObject);
                var markdown = formatter.Format(docData);
                new DocumentationPreviewForm(markdown, currentObject.Name).ShowDialog();
            }, "generar documentación markdown");
        }

        private bool ExecTraceVariable(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                string selectedVar = Utils.GetSelectedTextSafe(commandData);
                if (string.IsNullOrEmpty(selectedVar)) { Utils.ShowError("Seleccione una variable."); return; }
                if (!selectedVar.StartsWith("&")) selectedVar = "&" + selectedVar;
                KBObject currentObject = GetObjectFromContext(commandData);
                if (currentObject == null) { Utils.ShowError("Objeto no identificado."); return; }
                var traces = ServiceFactory.GetVariableTracerService().GetOccurrences(currentObject, selectedVar);
                if (traces.Count == 0) { Utils.ShowInfo("No hay ocurrencias.", "Rastreador"); return; }
                using (var tracerForm = new VariableTracerForm(selectedVar, traces))
                {
                    tracerForm.OnJumpToCode += (trace) => JumpToLine(currentObject, trace);
                    tracerForm.ShowDialog();
                }
            }, "rastrear variable");
        }

        private void JumpToLine(KBObject obj, VariableOccurrenceDto trace)
        {
            Utils.Log($"🔍 [JumpToLine] Iniciando navegación a línea {trace?.LineNumber} en parte '{trace?.PartName}'");

            if (obj == null || trace == null)
            {
                Utils.Log("❌ [JumpToLine] obj o trace es null");
                return;
            }

            // Usar la nueva utilidad robusta de navegación (cubre ActiveView y Reflection)
            if (Utils.NavigateToLine(obj, trace.PartName, trace.LineNumber))
            {
                Utils.Log("✅ Navegación completada exitosamente vía Utils.");
                return;
            }

            Utils.Log("❌ Todas las estrategias de navegación fallaron.");
            Utils.ShowWarning("Se encontró la definición pero no se pudo mover el cursor automáticamente.\nLínea: " + trace.LineNumber, "Navegación");
        }

        private bool ExecGoToSubroutine(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                Utils.Log("🚀 Comando 'Go To Subroutine' iniciado");

                if (Utils.TryInvokeGoToImplementation())
                {
                    Utils.Log("✅ Navegacion nativa completada.");
                    return;
                }

                // Estrategia de detección mejorada (estilo IDE moderno con SOLID):
                var textExtractor = ServiceFactory.GetEditorTextExtractor();

                // 1. Selección explícita del usuario (máxima prioridad)
                string selection = Utils.GetSelectedTextSafe(commandData);
                if (!string.IsNullOrEmpty(selection))
                {
                    Utils.Log($"✅ Método 1: Selección explícita - '{selection}'");
                }

                // 2. Palabra bajo el cursor (nueva funcionalidad tipo F12 - método LSI)
                if (string.IsNullOrEmpty(selection))
                {
                    selection = textExtractor.GetWordUnderCursor(commandData);
                    if (!string.IsNullOrEmpty(selection))
                    {
                        Utils.Log($"✅ Método 2: Palabra bajo cursor (LSI) - '{selection}'");
                    }
                }

                // 3. Fallback: toda la línea (para compatibilidad con código anterior)
                if (string.IsNullOrEmpty(selection))
                {
                    selection = textExtractor.GetCurrentLine(commandData);
                    if (!string.IsNullOrEmpty(selection))
                    {
                        Utils.Log($"✅ Método 3: Línea completa - '{selection}'");
                    }
                }

                // 4. Último recurso: intentar vía clipboard
                if (string.IsNullOrEmpty(selection))
                {
                    selection = Utils.GetTextViaClipboard();
                    if (!string.IsNullOrEmpty(selection))
                    {
                        Utils.Log($"✅ Método 4: Clipboard - '{selection}'");
                    }
                }

                if (string.IsNullOrEmpty(selection))
                {
                    Utils.Log("❌ Ningún método pudo detectar texto");
                    Utils.ShowError("No se detectó ninguna selección ni subrutina bajo el cursor.\n\nPor favor revisa el Output para detalles de debugging.");
                    return;
                }

                string subName = ServiceFactory.GetSubroutineNavigatorService().CleanSubroutineName(selection);
                Utils.Log($"🔍 Nombre de subrutina limpio: '{subName}'");

                // Intentar obtener la parte actual (LSI)
                KBObjectPart currentPart = null;
                try { currentPart = LSI.Packages.Extensiones.Utilidades.Entorno.CurrentEditingPart; } catch { }

                // Fallback: Si LSI falla, intentar obtener el objeto del documento activo
                KBObject currentObject = currentPart?.KBObject;
                if (currentObject == null && UIServices.Environment.ActiveDocument != null)
                {
                    currentObject = UIServices.Environment.ActiveDocument.Object;
                }

                if (currentObject == null) { Utils.ShowError("No se pudo identificar el objeto activo."); return; }

                Utils.Log($"✅ Objeto identificado: {currentObject.Name}");
                Utils.Log($"🔍 Buscando definición de 'Sub {subName}'...");

                // Si tenemos la parte específica, buscamos ahí primero
                int line = -1;
                string targetPartName = "";

                if (currentPart != null)
                {
                    Utils.Log($"🔍 Buscando en parte actual: {currentPart.Name}");
                    string source = GetPartSource(currentPart);
                    if (!string.IsNullOrEmpty(source))
                    {
                        Utils.Log($"✅ Source obtenido: {source.Length} caracteres");
                        line = ServiceFactory.GetSubroutineNavigatorService().FindDefinitionLine(source, subName);
                        if (line > 0)
                        {
                            targetPartName = currentPart.Name;
                            Utils.Log($"✅ Definición encontrada en línea {line} de {targetPartName}");
                        }
                        else
                        {
                            Utils.Log($"⚠️ No se encontró en parte actual");
                        }
                    }
                }

                // Si no encontramos en la parte actual (o no la tenemos), buscamos en todas las partes del objeto
                if (line <= 0)
                {
                    Utils.Log($"🔍 Buscando en todas las partes del objeto ({currentObject.Parts.Count} partes)");
                    foreach (KBObjectPart part in currentObject.Parts)
                    {
                        // Evitar buscar de nuevo en la misma parte si ya buscamos
                        if (currentPart != null && part.Guid == currentPart.Guid) continue;

                        Utils.Log($"  🔍 Buscando en: {part.Name}");
                        string source = GetPartSource(part);
                        if (!string.IsNullOrEmpty(source))
                        {
                            line = ServiceFactory.GetSubroutineNavigatorService().FindDefinitionLine(source, subName);
                            if (line > 0)
                            {
                                targetPartName = part.Name;
                                Utils.Log($"✅ Definición encontrada en línea {line} de {targetPartName}");
                                break;
                            }
                        }
                    }
                }

                if (line > 0)
                {
                    Utils.Log($"🚀 Ejecutando salto a línea {line} en {targetPartName}");
                    
                    // GUARDAR POSICIÓN ACTUAL ANTES DE SALTAR
                    SaveCurrentNavigationState(commandData);

                    JumpToLine(currentObject, new VariableOccurrenceDto { LineNumber = line, PartName = targetPartName });
                    Utils.Log($"✅ Navegación completada");
                }
                else
                {
                    Utils.Log($"❌ No se encontró la definición de 'Sub {subName}' en ninguna parte");
                    Utils.ShowWarning("No se encontró la definición de 'Sub " + subName + "' en el objeto " + currentObject.Name + ".", "Navegación");
                }
            }, "ir a definición de subrutina");
        }

        private string GetPartSource(KBObjectPart part)
        {
            try 
            {
                 if (part is ISource s) return s.Source;
                 // Reflection fallback
                 return part.GetType().GetProperty("Source")?.GetValue(part)?.ToString();
            }
            catch { return null; }
        }

        // Métodos GetWordUnderCursor() y GetCurrentLineContext() migrados a EditorTextExtractor (Services/EditorTextExtractor.cs)
        // Acceso vía ServiceFactory.GetEditorTextExtractor() - Principio Single Responsibility

        private bool ExecExportTableStructure(CommandData commandData) { return ExecuteWithErrorHandling(() => ExtractorTablasGX.ExportarEstructuraTablas(), "exportar estructura"); }
        private bool ExecExportProcedureSource(CommandData commandData) { return ExecuteWithErrorHandling(() => ProcedureSourceExtractor.ExportarSourceCodeProcedimientos(), "exportar source"); }
        private bool ExecCountCodeLines(CommandData commandData) { return ExecuteWithErrorHandling(() => new CodeLineCounter().ShowCodeLineCountForm(), "contar líneas"); }
        private bool ExecExportObjectsWithSourceLines(CommandData commandData) { return ExecuteWithErrorHandling(() => ObjectSourceLineExporter.ExportarObjetosConLineasOperativas(), "exportar objetos"); }
        private bool ExecShowObjectHistory(CommandData commandData) { return ExecuteWithErrorHandling(() => new ObjectHistoryExporter().ExportToCSV(), "exportar historial"); }
        public bool ExecGenerateLogDebugFormCommand(CommandData commandData) { return ExecuteWithErrorHandling(() => new DebugCodeGenerator().ShowInputFormAndGenerate(), "generar debug"); }
        private bool ExecWGExtractVariableCommand(CommandData commandData) { return ExecuteWithErrorHandling(() => new VariableExtractor().ExtractFromSelection(commandData), "extraer variable"); }
        private bool ExecWGExtractProcedureCommand(CommandData commandData) { return ExecuteWithErrorHandling(() => new ProcedureVariableExtractor().ExtractAndGenerateCode(commandData), "extraer variables proc"); }
        private bool ExecSmartFixVariables(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                KBObject currentObject = GetObjectFromContext(commandData);
                if (currentObject == null) return;
                var undefinedVars = ServiceFactory.GetSmartVariableService().GetUndefinedVariables(currentObject);
                if (!undefinedVars.Any()) { Utils.ShowInfo("No hay variables huérfanas.", "Smart Fix"); return; }
                using (var fixForm = new SmartFixVariablesForm(undefinedVars))
                {
                    if (fixForm.ShowDialog() == DialogResult.OK)
                    {
                        ServiceFactory.GetSmartVariableService().DefineVariables(currentObject, fixForm.SelectedVariables);
                        Utils.ShowInfo("Se crearon " + fixForm.SelectedVariables.Count + " variables.", "Éxito");
                    }
                }
            }, "smart fix");
        }

        private bool QueryAlwaysEnabled(CommandData commandData, ref CommandStatus status) { status.State = CommandState.Enabled; return true; }
        private bool QueryGenerateLogDebugFormCommand(CommandData commandData, ref CommandStatus status) { status.State = UIServices.KB?.CurrentKB != null ? CommandState.Enabled : CommandState.Disabled; return true; }

        private bool ExecuteWithErrorHandling(Action action, string operationName)
        {
            try { Utils.Log("🔄 Iniciando: " + operationName); action(); return true; }
            catch (Exception ex) { Utils.Log("❌ Error en " + operationName + ": " + ex.Message); Utils.ShowError("Error en " + operationName + ": " + ex.Message); return false; }
        }
    }
}
