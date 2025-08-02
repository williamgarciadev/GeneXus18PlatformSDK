using System;
using System.Collections.Generic;
using System.Linq;
using Artech.Common.Framework.Commands;
using Acme.Packages.Menu.Utilities;
using LSI.Packages.Extensiones.Utilidades;

namespace Acme.Packages.Menu.Services.Variables
{
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
}