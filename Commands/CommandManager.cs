using System;
using System.Linq;
using System.Collections.Generic;
using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Framework.Commands;
using Acme.Packages.Menu.Services.Export;
using Acme.Packages.Menu.Services.Variables;
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
            AddCommand(CommandKeys.CmdFindUnreferencedObjects, new ExecHandler(ExecFindUnreferencedObjects), new QueryHandler(QueryGenerateLogDebugFormCommand));
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
            if (obj == null || trace == null) return;

            // 1. Abrir y asegurar que el documento está en pantalla
            try
            {
                if (UIServices.DocumentManager != null)
                {
                    UIServices.DocumentManager.OpenDocument(obj, new OpenDocumentOptions());
                }
            }
            catch (Exception ex) { Utils.Log("⚠ Error abriendo documento: " + ex.Message); }
            
            // Pequeña pausa para que el IDE procese la apertura de la pestaña
            System.Threading.Thread.Sleep(50);
            Application.DoEvents();

            // 2. Intentar salto vía Documento Activo (Método más estable en GX18)
            try 
            {
                if (UIServices.Environment != null && UIServices.Environment.ActiveDocument != null && UIServices.Environment.ActiveDocument.Object == obj)
                {
                    var activeDoc = UIServices.Environment.ActiveDocument;
                    // Buscamos el método Select en el documento
                    var selectMethod = activeDoc.GetType().GetMethod("Select", new Type[] { typeof(int), typeof(int), typeof(int) });
                    if (selectMethod != null)
                    {
                        // Linea, Caracter, Largo
                        selectMethod.Invoke(activeDoc, new object[] { trace.LineNumber, 1, 0 });
                        Utils.Log("🚀 Salto ejecutado vía ActiveDocument.Select");
                        return;
                    }
                }
            } catch (Exception ex) { Utils.Log("⚠ Error en salto ActiveDocument: " + ex.Message); }

            // 3. Si falla, intentar vía Editor de la Parte
            try
            {
                foreach (KBObjectPart part in obj.Parts)
                {
                    if (part == null) continue;
                    
                    string partName = part.Name ?? "";
                    string targetName = trace.PartName ?? "";

                    bool nameMatch = partName.Equals(targetName, StringComparison.OrdinalIgnoreCase);
                    bool typeMatch = false;
                    
                    if (!nameMatch && part.TypeDescriptor != null && part.TypeDescriptor.Name != null)
                    {
                        typeMatch = part.TypeDescriptor.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase);
                    }

                    if (nameMatch || typeMatch)
                    {
                        if (UIServices.EditorManager != null)
                        {
                            object editorObj = UIServices.EditorManager.GetEditor(part.Guid);
                            if (editorObj != null)
                            {
                                Type eType = editorObj.GetType();
                                
                                // Probamos todas las combinaciones de salto conocidas
                                var mGoTo = eType.GetMethod("GoTo", new Type[] { typeof(Artech.Common.Location.IPosition) }) ??
                                            eType.GetMethod("SetPosition", new Type[] { typeof(int), typeof(int), typeof(int) }) ??
                                            eType.GetMethod("Select", new Type[] { typeof(int), typeof(int) });

                                if (mGoTo != null)
                                {
                                    if (mGoTo.GetParameters().Length == 1) // GoTo(IPosition)
                                    {
                                        Type tPosType = typeof(Artech.Common.Location.IPosition).Assembly.GetType("Artech.Common.Location.TextPosition");
                                        object tPos = Activator.CreateInstance(tPosType, new object[] { trace.LineNumber, 1 });
                                        mGoTo.Invoke(editorObj, new object[] { tPos });
                                    }
                                    else if (mGoTo.GetParameters().Length == 3) // SetPosition(l, c, len)
                                        mGoTo.Invoke(editorObj, new object[] { trace.LineNumber, 1, 0 });
                                    else // Select(l, c)
                                        mGoTo.Invoke(editorObj, new object[] { trace.LineNumber, 1 });

                                    // FORZAR SCROLL
                                    eType.GetMethod("ScrollToCaret")?.Invoke(editorObj, null);
                                    eType.GetMethod("Focus")?.Invoke(editorObj, null);
                                    Application.DoEvents();
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex) { Utils.Log("❌ Error en salto por Editor: " + ex.ToString()); }
        }

        private bool ExecGoToSubroutine(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                // Prioridad absoluta a la selección real del editor (Estilo Descartes)
                string selection = Utils.GetSelectedTextSafe(commandData);
                
                // Si no hay selección, intentar obtener la línea bajo el cursor
                if (string.IsNullOrEmpty(selection)) selection = GetCurrentLineContext();

                // Si aún no hay selección, intentar obtenerla vía Clipboard (Copy)
                if (string.IsNullOrEmpty(selection)) selection = Utils.GetTextViaClipboard();
                
                if (string.IsNullOrEmpty(selection)) 
                {
                    Utils.ShowError("No se detectó ninguna selección ni subrutina bajo el cursor."); 
                    return; 
                }

                string subName = ServiceFactory.GetSubroutineNavigatorService().CleanSubroutineName(selection);

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

                // Si tenemos la parte específica, buscamos ahí primero
                int line = -1;
                string targetPartName = "";

                if (currentPart != null)
                {
                    string source = GetPartSource(currentPart);
                    if (!string.IsNullOrEmpty(source))
                    {
                        line = ServiceFactory.GetSubroutineNavigatorService().FindDefinitionLine(source, subName);
                        targetPartName = currentPart.Name;
                    }
                }

                // Si no encontramos en la parte actual (o no la tenemos), buscamos en todas las partes del objeto
                if (line <= 0)
                {
                    foreach (KBObjectPart part in currentObject.Parts)
                    {
                        // Evitar buscar de nuevo en la misma parte si ya buscamos
                        if (currentPart != null && part.Guid == currentPart.Guid) continue;

                        string source = GetPartSource(part);
                        if (!string.IsNullOrEmpty(source))
                        {
                            line = ServiceFactory.GetSubroutineNavigatorService().FindDefinitionLine(source, subName);
                            if (line > 0)
                            {
                                targetPartName = part.Name;
                                break;
                            }
                        }
                    }
                }

                if (line > 0) 
                {
                    JumpToLine(currentObject, new VariableOccurrenceDto { LineNumber = line, PartName = targetPartName });
                }
                else 
                {
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

        private string GetCurrentLineContext()
        {
            try
            {
                KBObjectPart currentPart = LSI.Packages.Extensiones.Utilidades.Entorno.CurrentEditingPart;
                if (currentPart == null) return null;

                object editorObj = UIServices.EditorManager.GetEditor(currentPart.Guid);
                if (editorObj == null) return null;

                // 1. Obtener el número de línea actual (Nivel SDK)
                int lineNumber = -1;
                var propPos = editorObj.GetType().GetProperty("CurrentPosition");
                if (propPos != null)
                {
                    object posObj = propPos.GetValue(editorObj);
                    if (posObj != null)
                    {
                        var propLine = posObj.GetType().GetProperty("Line");
                        if (propLine != null) lineNumber = Convert.ToInt32(propLine.GetValue(posObj));
                    }
                }

                if (lineNumber <= 0) return null;

                // 2. Obtener el código de la parte actual
                string source = "";
                if (currentPart is ISource s) source = s.Source;
                else source = currentPart.GetType().GetProperty("Source")?.GetValue(currentPart)?.ToString();

                if (string.IsNullOrEmpty(source)) return null;

                // 3. Extraer la línea exacta
                string[] lines = source.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                if (lineNumber <= lines.Length)
                {
                    return lines[lineNumber - 1].Trim();
                }
            }
            catch (Exception ex)
            {
                Utils.Log("❌ Error detectando línea: " + ex.Message);
            }
            return null;
        }

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