using System;
using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Framework.Commands;
using Acme.Packages.Menu.Services.Export;
using Acme.Packages.Menu.Services.Variables;
using Acme.Packages.Menu.Utilities;
using Artech.Architecture.UI.Framework.Helper;
using Acme.Packages.Menu.Common.Factories;
using Acme.Packages.Menu.UI.Forms;
using Artech.Architecture.Common.Objects;

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
        }

        #endregion

        #region Export Commands

        /// <summary>
        /// Genera documentación en Markdown para el objeto seleccionado
        /// </summary>
        private bool ExecGenerateMarkdownDocs(CommandData commandData)
        {
            return ExecuteWithErrorHandling(() =>
            {
                KBObject currentObject = UIServices.KB.CurrentObject;
                if (currentObject == null)
                {
                    Utils.ShowError("No hay ningún objeto seleccionado o abierto.");
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