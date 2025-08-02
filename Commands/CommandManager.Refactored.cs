using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;
using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Framework.Commands;
using Artech.Architecture.Common.Objects;
using Artech.Architecture.UI.Framework.Helper;
using Artech.Genexus.Common;
using Artech.Udm.Framework;
using LSI.Packages.Extensiones.Utilidades;
using Acme.Packages.Menu.Utilities;

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
        }

        #endregion

        #region Export Commands

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
        /// Ejecuta una acción con manejo consistente de errores
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
                Utils.Log($"✅ Operación completada: {operationName}");
                return true;
            }
            catch (Exception ex)
            {
                string errorMessage = $"❌ Error al {operationName}: {ex.Message}";
                Utils.ShowError(errorMessage);
                Utils.Log(errorMessage);
                return false;
            }
        }

        #endregion
    }

    #region Extracted Classes

    /// <summary>
    /// Maneja la exportación del historial de objetos de la KB
    /// </summary>
    internal class ObjectHistoryExporter
    {
        public void ExportToCSV()
        {
            var model = UIServices.KB.CurrentModel;
            var kb = model.KB;

            ValidateKBAccess(model, kb);

            var csvData = GenerateHistoryData(model, kb);
            var filePath = SaveToFile(csvData);

            Utils.ShowInfo(
                $"✅ Historial exportado exitosamente a:\n{filePath}", 
                "Exportación completa");
        }

        private void ValidateKBAccess(KBModel model, KnowledgeBase kb)
        {
            if (model == null || kb == null)
            {
                throw new InvalidOperationException("No se pudo acceder al modelo o la KB actual.");
            }
        }

        private List<string> GenerateHistoryData(KBModel model, KnowledgeBase kb)
        {
            var lines = new List<string>
            {
                "Objeto,Tipo,Fecha,Usuario,Operación,Versión"
            };

            foreach (Guid objType in KnowledgeBase.GetKBObjectTypes())
            {
                ProcessObjectType(model, kb, objType, lines);
            }

            return lines;
        }

        private void ProcessObjectType(KBModel model, KnowledgeBase kb, Guid objType, List<string> lines)
        {
            var entities = kb.GetEntitiesByModelTypeOrderByName(model, objType);
            
            foreach (Entity entity in entities)
            {
                var obj = KBObject.Get(model, entity.Key);
                if (obj == null) continue;

                ProcessObjectHistory(kb, model, obj, lines);
            }
        }

        private void ProcessObjectHistory(KnowledgeBase kb, KBModel model, KBObject obj, List<string> lines)
        {
            var historyList = kb.GetModelEntityHistoryByModelEntityKey(model, obj.Key);
            if (historyList == null || !historyList.Any()) return;

            foreach (var history in historyList.OrderByDescending(h => h.Timestamp))
            {
                var csvLine = FormatHistoryToCsv(obj, history, kb);
                lines.Add(csvLine);
            }
        }

        private string FormatHistoryToCsv(KBObject obj, dynamic history, KnowledgeBase kb)
        {
            var nombre = obj.Name.Replace(",", "");
            var tipo = obj.TypeName;
            var fecha = history.Timestamp.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
            var usuario = kb.LoadKBUser(history.HistoryUserId)?.Name ?? "Desconocido";
            var operacion = history.Operation.ToString();
            var version = history.EntityVersionId.ToString();

            return $"{nombre},{tipo},{fecha},{usuario},{operacion},{version}";
        }

        private string SaveToFile(List<string> csvData)
        {
            var filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
                "HistorialObjetos.csv");

            File.WriteAllLines(filePath, csvData, Encoding.UTF8);
            return filePath;
        }
    }

    /// <summary>
    /// Maneja la generación de código de debug con variables
    /// </summary>
    internal class DebugCodeGenerator
    {
        public void ShowInputFormAndGenerate()
        {
            using (var inputForm = new VariablesInputForm())
            {
                if (inputForm.ShowDialog() != DialogResult.OK) return;

                var inputVariables = inputForm.InputVariables;
                var outputType = inputForm.SelOutputType;

                ValidateInput(inputVariables);
                
                var extractedVariables = ExtractValidVariables(inputVariables);
                var logLines = GenerateLogLines(extractedVariables, outputType);

                Utils.ShowResultForm(logLines);
            }
        }

        private void ValidateInput(List<string> inputVariables)
        {
            if (!inputVariables.Any())
            {
                throw new ArgumentException("No se han ingresado variables válidas.");
            }
        }

        private List<string> ExtractValidVariables(List<string> inputVariables)
        {
            var extractedVariables = inputVariables
                .SelectMany(Utils.ExtractVariables)
                .ToList();

            if (!extractedVariables.Any())
            {
                throw new ArgumentException("No se encontraron variables que comiencen con '&'.");
            }

            return extractedVariables;
        }

        private List<string> GenerateLogLines(List<string> variables, string outputType)
        {
            return outputType == "msg" 
                ? Utils.GenerateMsgFormatLines(variables, false)
                : Utils.GenerateLogDebugLines(variables);
        }
    }

    /// <summary>
    /// Maneja la extracción inteligente de variables
    /// </summary>
    internal class VariableExtractor
    {
        public void ExtractFromSelection(CommandData commandData)
        {
            var selectedText = GetValidatedSelection(commandData);
            var variableName = Utils.RemoveAmpersand(selectedText);
            var currentPart = GetValidatedCurrentPart();

            Utils.Log($"🔍 Variable extraída: {variableName}");

            var variableInfo = ResolveVariableType(variableName);
            var newVariableName = $"&{variableInfo.CleanName}";

            UpdateEditor(commandData, selectedText, newVariableName);
            CreateVariableInKB(variableInfo, currentPart, commandData);
        }

        private string GetValidatedSelection(CommandData commandData)
        {
            var selectedText = Utils.GetSelectedTextSafe(commandData);
            if (string.IsNullOrEmpty(selectedText))
            {
                throw new ArgumentException("No se pudo obtener texto seleccionado.");
            }
            return selectedText;
        }

        private KBObjectPart GetValidatedCurrentPart()
        {
            var currentPart = Entorno.CurrentEditingPart;
            if (currentPart == null)
            {
                throw new InvalidOperationException("No se encontró el contexto actual.");
            }
            return currentPart;
        }

        private VariableInfo ResolveVariableType(string variableName)
        {
            if (variableName.Contains("_"))
            {
                return ResolveReferenceBasedVariable(variableName);
            }
            else
            {
                return ResolvePrefixBasedVariable(variableName);
            }
        }

        private VariableInfo ResolveReferenceBasedVariable(string variableName)
        {
            var parts = variableName.Split(new char[] { '_' }, 2, StringSplitOptions.None);
            var baseReference = parts[0].Trim();
            var cleanName = parts[1].Trim();

            Utils.Log($"🔹 Basando '{cleanName}' en '{baseReference}'");

            var (type, length, isBasedOnAttrOrDomain) = VariableHelper.GetTypeAndLengthFromReference(baseReference);

            return new VariableInfo
            {
                CleanName = cleanName,
                Type = type,
                Length = length,
                BaseReference = baseReference,
                IsBasedOnAttributeOrDomain = isBasedOnAttrOrDomain
            };
        }

        private VariableInfo ResolvePrefixBasedVariable(string variableName)
        {
            if (variableName.Length <= 1)
            {
                throw new ArgumentException("El nombre de la variable debe tener más de un carácter.");
            }

            var prefix = variableName[0];
            var cleanName = variableName.Substring(1).Trim();

            Utils.Log($"🔹 Prefijo detectado: {prefix}, Nombre limpio: {cleanName}");

            var (type, length) = VariableHelper.GetTypeFromPrefix(prefix);

            return new VariableInfo
            {
                CleanName = cleanName,
                Type = type,
                Length = length,
                BaseReference = null,
                IsBasedOnAttributeOrDomain = false
            };
        }

        private void UpdateEditor(CommandData commandData, string oldText, string newText)
        {
            Utils.ReplaceSelectedTextInEditor(commandData, oldText, newText);
        }

        private void CreateVariableInKB(VariableInfo varInfo, KBObjectPart currentPart, CommandData commandData)
        {
            if (varInfo.Type == eDBType.NONE)
            {
                throw new InvalidOperationException($"No se encontró una referencia válida para '{varInfo.BaseReference ?? varInfo.CleanName}'.");
            }

            var fullVariableName = $"&{varInfo.CleanName}";

            if (!VariableHelper.IsVariableDefined(fullVariableName, currentPart))
            {
                if (varInfo.IsBasedOnAttributeOrDomain)
                {
                    VariableHelper.AddVariableBasedOn(varInfo.CleanName, currentPart, varInfo.BaseReference, commandData);
                    Utils.Log($"✅ Variable '{varInfo.CleanName}' creada basada en '{varInfo.BaseReference}'.");
                }
                else
                {
                    VariableHelper.AddVariable(varInfo.CleanName, currentPart, varInfo.Type, varInfo.Length);
                    Utils.Log($"✅ Variable '{varInfo.CleanName}' creada con tipo {varInfo.Type} y longitud {varInfo.Length}.");
                }
            }
        }
    }

    /// <summary>
    /// Maneja la extracción de variables de procedimientos
    /// </summary>
    internal class ProcedureVariableExtractor
    {
        public void ExtractAndGenerateCode(CommandData commandData)
        {
            Utils.Log("Ejecutando comando WGExtractProcedureCommand");

            var selectedText = GetValidatedSelection(commandData);
            var variables = ExtractVariables(selectedText);
            var logLines = GenerateLogLines(variables);

            Utils.PasteResultInEditor(logLines);
            Utils.Log("Proceso completado con éxito.");
        }

        private string GetValidatedSelection(CommandData commandData)
        {
            var selectedText = Utils.GetSelectedTextSafe(commandData);
            if (string.IsNullOrWhiteSpace(selectedText))
            {
                throw new ArgumentException("No se pudo obtener texto válido.");
            }
            return selectedText;
        }

        private List<string> ExtractVariables(string selectedText)
        {
            var variables = Utils.ExtractVariables(selectedText);
            if (!variables.Any())
            {
                throw new ArgumentException("No se encontraron variables que comiencen con '&'.");
            }
            return variables;
        }

        private List<string> GenerateLogLines(List<string> variables)
        {
            return Utils.GenerateMsgFormatLines(variables, Utility.IsInRules());
        }
    }

    /// <summary>
    /// Información de una variable extraída
    /// </summary>
    internal class VariableInfo
    {
        public string CleanName { get; set; }
        public eDBType Type { get; set; }
        public int Length { get; set; }
        public string BaseReference { get; set; }
        public bool IsBasedOnAttributeOrDomain { get; set; }
    }

    #endregion
}