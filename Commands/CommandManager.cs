using System;
using System.Linq;
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

namespace Acme.Packages.Menu
{
    /// <summary>
    /// Gestor central de comandos del plugin Menu para GeneXus
    /// </summary>
    class CommandManager : CommandDelegator
    {
        #region Constructor

        public CommandManager()
        {
            RegisterCommands();
        }

        #endregion

        #region Command Registration

        /// <summary>
        /// Registra todos los comandos disponibles en el plugin
        /// </summary>
        private void RegisterCommands()
        {
            AddCommand(CommandKeys.CmdGenerateLogDebugForm,
                new ExecHandler(ExecGenerateLogDebugFormCommand),
                new QueryHandler(QueryGenerateLogDebugFormCommand));

            AddCommand(CommandKeys.WGExtractVariable,
                new ExecHandler(ExecWGExtractVariableCommand),
                new QueryHandler(QueryAlwaysEnabled));

            AddCommand(CommandKeys.WGExtractProcedure,
                new ExecHandler(ExecWGExtractProcedureCommand),
                new QueryHandler(QueryAlwaysEnabled));

            AddCommand(CommandKeys.ShowObjectHistory,
                new ExecHandler(ExecShowObjectHistory),
                new QueryHandler(QueryAlwaysEnabled));

            AddCommand(CommandKeys.CmdExportTableStructure,
                new ExecHandler(ExecExportTableStructure),
                new QueryHandler(QueryAlwaysEnabled));

            AddCommand(CommandKeys.CmdExportProcedureSource,
                new ExecHandler(ExecExportProcedureSource),
                new QueryHandler(QueryAlwaysEnabled));

            AddCommand(CommandKeys.CmdCountCodeLines,
                new ExecHandler(ExecCountCodeLines),
                new QueryHandler(QueryAlwaysEnabled));

            AddCommand(CommandKeys.CmdExportObjectsWithSourceLines,
                new ExecHandler(ExecExportObjectsWithSourceLines),
                new QueryHandler(QueryAlwaysEnabled));

            AddCommand(CommandKeys.CmdGenerateMarkdownDocs,
                new ExecHandler(ExecGenerateMarkdownDocs),
                new QueryHandler(QueryGenerateLogDebugFormCommand));

            AddCommand(CommandKeys.CmdCleanUnusedVariables,
                new ExecHandler(ExecCleanUnusedVariables),
                new QueryHandler(QueryGenerateLogDebugFormCommand));

            AddCommand(CommandKeys.CmdSmartFixVariables,
                new ExecHandler(ExecSmartFixVariables),
                new QueryHandler(QueryGenerateLogDebugFormCommand));

            AddCommand(CommandKeys.CmdTraceVariable,
                new ExecHandler(ExecTraceVariable),
                new QueryHandler(QueryAlwaysEnabled));
        }

        #endregion

        #region Helper for Selection

        private KBObject GetObjectFromContext(CommandData commandData)
        {
            KBObject found = null;

            if (commandData != null && commandData.Context != null)
            {
                object context = commandData.Context;
                Utils.Log("🔍 Analizando contexto: " + context.GetType().FullName);

                found = ExtractKBObject(context);

                if (found == null && context is ISelectionContainer container)
                {
                    if (container.SelectedObjects != null)
                    {
                        foreach (object selObj in container.SelectedObjects)
                        {
                            found = ExtractKBObject(selObj);
                            if (found != null) break;
                        }
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

            if (found == null)
            {
                Utils.Log("🔍 Contexto no identificado, intentando fallback de editor activo...");
                found = ExtractKBObject(null);
            }

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
            catch {{}}

            return null;
        }

        #endregion

        #region Export Commands

        /// <summary>
        /// Limpia las variables no utilizadas del objeto actual
        /// </summary>
        private bool ExecCleanUnusedVariables(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                KBObject currentObject = GetObjectFromContext(commandData);

                if (currentObject == null)
                {
                    Utils.ShowError("No se pudo identificar el objeto para limpiar.\nPor favor, asegúrese de seleccionar un objeto en el KB Explorer o tener uno abierto.");
                    return;
                }

                var cleaner = ServiceFactory.GetVariableCleanerService();
                int removed = cleaner.CleanUnusedVariables(currentObject);

                if (removed > 0)
                {
                    Utils.ShowInfo($"✅ Se eliminaron {removed} variables no utilizadas en '{currentObject.Name}'.", "Limpieza Completada");
                }
                else
                {
                    Utils.ShowInfo("No se encontraron variables no utilizadas para eliminar.", "Limpieza Completada");
                }
            }, "limpiar variables no usadas");
        }

        /// <summary>
        /// Genera documentación en Markdown para el objeto seleccionado
        /// </summary>
        private bool ExecGenerateMarkdownDocs(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                KBObject currentObject = GetObjectFromContext(commandData);

                if (currentObject == null)
                {
                    Utils.ShowError("No se pudo identificar el objeto para documentar.\nPor favor, seleccione un objeto en el KB Explorer o abra uno en el editor.");
                    return;
                }

                var docService = ServiceFactory.GetDocumentationService();
                var formatter = ServiceFactory.GetDocumentationFormatter();

                var docData = docService.ExtractDocumentation(currentObject);
                var markdown = formatter.Format(docData);

                var previewForm = new DocumentationPreviewForm(markdown, currentObject.Name);
                previewForm.ShowDialog();
            }, "generar documentación markdown");
        }

        /// <summary>
        /// Exporta la estructura de tablas de todas las transacciones
        /// </summary>
        private bool ExecExportTableStructure(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                ExtractorTablasGX.ExportarEstructuraTablas();
            }, "exportar estructura de tablas");
        }

        /// <summary>
        /// Exporta el código fuente de todos los procedimientos
        /// </summary>
        private bool ExecExportProcedureSource(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                ProcedureSourceExtractor.ExportarSourceCodeProcedimientos();
            }, "exportar código fuente de procedimientos");
        }

        /// <summary>
        /// Muestra formulario interactivo para contar líneas de código
        /// </summary>
        private bool ExecCountCodeLines(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var lineCounter = new CodeLineCounter();
                lineCounter.ShowCodeLineCountForm();
            }, "mostrar contador de líneas de código");
        }

        /// <summary>
        /// Exporta objetos con líneas operativas a CSV y archivos individuales de código fuente
        /// </summary>
        private bool ExecExportObjectsWithSourceLines(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                ObjectSourceLineExporter.ExportarObjetosConLineasOperativas();
            }, "exportar objetos con líneas operativas y código fuente");
        }

        /// <summary>
        /// Exporta el historial completo de objetos de la KB
        /// </summary>
        private bool ExecShowObjectHistory(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var historyExporter = new ObjectHistoryExporter();
                historyExporter.ExportToCSV();
            }, "exportar historial de objetos");
        }

        #endregion

        #region Variable Extraction Commands

        /// <summary>
        /// Genera formulario para crear líneas de debug con variables
        /// </summary>
        public bool ExecGenerateLogDebugFormCommand(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var debugGenerator = new DebugCodeGenerator();
                debugGenerator.ShowInputFormAndGenerate();
            }, "generar líneas de debug");
        }

        /// <summary>
        /// Extrae e inteligentemente crea una variable basada en texto seleccionado
        /// </summary>
        private bool ExecWGExtractVariableCommand(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var variableExtractor = new VariableExtractor();
                variableExtractor.ExtractFromSelection(commandData);
            }, "extraer variable inteligente");
        }

        /// <summary>
        /// Extrae variables de texto seleccionado y genera código de logging
        /// </summary>
        private bool ExecWGExtractProcedureCommand(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var procedureExtractor = new ProcedureVariableExtractor();
                procedureExtractor.ExtractAndGenerateCode(commandData);
            }, "extraer variables de procedimiento");
        }

        #endregion

        #region Query Handlers

        private bool QueryAlwaysEnabled(CommandData commandData, ref CommandStatus status)
        {
            status.State = CommandState.Enabled;
            return true;
        }

        private bool QueryGenerateLogDebugFormCommand(CommandData commandData, ref CommandStatus status)
        {
            status.State = UIServices.KB?.CurrentKB != null
                ? CommandState.Enabled
                : CommandState.Disabled;
            return true;
        }

        #endregion

        #region Smart Fix Commands

        /// <summary>
        /// Rastrear una variable seleccionada en todo el objeto actual.
        /// </summary>
        private bool ExecTraceVariable(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                string selectedVar = Utils.GetSelectedTextSafe(commandData);
                if (string.IsNullOrEmpty(selectedVar))
                {
                    Utils.ShowError("Por favor, seleccione el nombre de una variable en el código para rastrear.");
                    return;
                }

                if (!selectedVar.StartsWith("&")) selectedVar = "&" + selectedVar;

                KBObject currentObject = GetObjectFromContext(commandData);
                if (currentObject == null)
                {
                    Utils.ShowError("No se pudo identificar el objeto para rastrear.");
                    return;
                }

                var tracer = ServiceFactory.GetVariableTracerService();
                var traces = tracer.GetOccurrences(currentObject, selectedVar);

                if (traces.Count == 0)
                {
                    Utils.ShowInfo($"No se encontraron ocurrencias de '{selectedVar}' en el código activo.", "Rastreador");
                    return;
                }

                using (var tracerForm = new VariableTracerForm(selectedVar, traces))
                {
                    tracerForm.OnJumpToCode += (trace) => {
                        JumpToLine(currentObject, trace);
                    };
                    tracerForm.ShowDialog();
                }
            }, "rastrear variable");
        }

        private void JumpToLine(KBObject obj, VariableOccurrenceDto trace)
        {
            try 
            {
                foreach (KBObjectPart part in obj.Parts)
                {
                    if (part.Name.Equals(trace.PartName, StringComparison.OrdinalIgnoreCase) || 
                        part.TypeDescriptor.Name.Equals(trace.PartName, StringComparison.OrdinalIgnoreCase))
                    {
                        UIServices.DocumentManager.OpenDocument(obj, new OpenDocumentOptions());
                        
                        try 
                        {
                            object editorObj = UIServices.EditorManager.GetEditor(part.Guid);
                            if (editorObj != null)
                            {
                                System.Reflection.MethodInfo selectMethod = editorObj.GetType().GetMethod("Select", new Type[] { typeof(int), typeof(int) });
                                if (selectMethod != null)
                                {
                                    selectMethod.Invoke(editorObj, new object[] { trace.LineNumber, 1 });
                                }
                            }
                        }
                        catch {{}}
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log("No se pudo saltar a la línea: " + ex.Message);
            }
        }

        /// <summary>
        /// Escanea el objeto actual y permite definir variables no declaradas masivamente.
        /// </summary>
        private bool ExecSmartFixVariables(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                KBObject currentObject = GetObjectFromContext(commandData);

                if (currentObject == null)
                {
                    Utils.ShowError("No se pudo identificar el objeto para analizar.");
                    return;
                }

                var smartService = ServiceFactory.GetSmartVariableService();
                var undefinedVars = smartService.GetUndefinedVariables(currentObject);

                if (!undefinedVars.Any())
                {
                    Utils.ShowInfo("No se detectaron variables sin definir en este objeto.", "Smart Fix");
                    return;
                }

                using (var fixForm = new SmartFixVariablesForm(undefinedVars))
                {
                    if (fixForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        smartService.DefineVariables(currentObject, fixForm.SelectedVariables);
                        Utils.ShowInfo($"✅ Se crearon {fixForm.SelectedVariables.Count} variables exitosamente.", "Smart Fix Completado");
                    }
                }
            }, "auto-definir variables masivamente");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Ejecuta una acción con manejo robusto y específico de errores
        /// </summary>
        /// <param name="action">Acción a ejecutar</param>
        /// <param name="operationName">Nombre de la operación para logging</param>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        private bool ExecuteWithErrorHandling(Action action, string operationName)
        {
            try
            {
                Utils.Log($"🔄 Iniciando operación: {operationName}");
                action();
                Utils.Log($"✅ Operación completada exitosamente: {operationName}");
                return true;
            }
            catch (InvalidOperationException ex)
            {
                HandleSpecificError(ex, operationName, "Operación inválida");
                return false;
            }
            catch (ArgumentException ex)
            {
                HandleSpecificError(ex, operationName, "Argumento inválido");
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                HandleSpecificError(ex, operationName, "Acceso denegado");
                return false;
            }
            catch (System.IO.IOException ex)
            {
                HandleSpecificError(ex, operationName, "Error de archivo/sistema");
                return false;
            }
            catch (Exception ex)
            {
                HandleGenericError(ex, operationName);
                return false;
            }
        }

        private void HandleSpecificError(Exception ex, string operationName, string errorType)
        {
            string errorMessage = $"❌ {errorType} al {operationName}: {ex.Message}";
            Utils.ShowError($"{errorType}\n\nOperación: {operationName}\nDetalle: {ex.Message}");
            Utils.Log($"{errorMessage}\nStackTrace: {ex.StackTrace}");
        }

        private void HandleGenericError(Exception ex, string operationName)
        {
            string errorMessage = $"❌ Error inesperado al {operationName}: {ex.Message}";
            Utils.ShowError($"Error Inesperado\n\nOperación: {operationName}\nTipo: {ex.GetType().Name}\nDetalle: {ex.Message}");
            Utils.Log($"{errorMessage}\nTipo: {ex.GetType().FullName}\nStackTrace: {ex.StackTrace}");
        }

        #endregion
    }
}