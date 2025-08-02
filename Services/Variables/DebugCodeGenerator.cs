using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Acme.Packages.Menu.Utilities;

namespace Acme.Packages.Menu.Services.Variables
{
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

                ValidateInputVariables(inputVariables);

                var extractedVariables = ExtractValidVariables(inputVariables);
                var logLines = GenerateLogLines(extractedVariables, outputType);

                Utils.ShowResultForm(logLines);
            }
        }

        private void ValidateInputVariables(List<string> inputVariables)
        {
            if (inputVariables == null)
            {
                throw new ArgumentNullException(nameof(inputVariables), "La lista de variables no puede ser nula.");
            }

            if (!inputVariables.Any())
            {
                throw new ArgumentException("Debe ingresar al menos una variable válida para generar código de debug.", nameof(inputVariables));
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
}