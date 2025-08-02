using Artech.Architecture.Common.Objects;
using Artech.Architecture.Common.Services;
using Artech.Genexus.Common;
using Artech.Genexus.Common.Parts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Packages.Menu
{
    public class VariableProcessor
    {
        /// <summary>
        /// Separa el texto ingresado en variables y atributos.
        /// </summary>
        public (List<string> Variables, List<string> Attributes) ExtractVariablesAndAttributes(List<string> input)
        {
            List<string> variables = new List<string>();
            List<string> attributes = new List<string>();

            foreach (var item in input)
            {
                if (item.StartsWith("&"))
                    variables.Add(item);
                else
                    variables.Add(item);
            }

            return (variables, attributes);
        }

        /// <summary>
        /// Genera las líneas de `Log.Debug` para las variables.
        /// </summary>
        public List<string> GenerateLogDebugLines(List<string> variables)
        {
            return variables.Where(variable => !string.IsNullOrEmpty(variable))
                            .Select(variable => $"Log.Debug(Format(\"{variable}=%1\", {variable}), '{RemoveAmpersand(variable)}')")
                            .ToList();
        }

        /// <summary>
        /// Genera las líneas `msg(Format(...))` para las variables.
        /// </summary>
        public List<string> GenerateMsgFormatLines(List<string> variables, bool isInRules = false)
        {
            List<string> msgLines = new List<string>();
            int batchSize = 7;

            // Asegurar que &Pgmname sea la primera variable
            if (!variables.Contains("&Pgmname"))
            {
                variables.Insert(0, "&Pgmname");
            }
            else
            {
                // Mover &Pgmname a la primera posición si ya está en la lista
                variables.Remove("&Pgmname");
                variables.Insert(0, "&Pgmname");
            }

            for (int i = 0; i < variables.Count; i += batchSize)
            {
                var batch = variables.Skip(i).Take(batchSize).ToList();
                var formats = new List<string>();
                var processedVariables = new List<string>();

                foreach (var var in batch)
                {
                    CommonServices.Output.AddLine("var " + var);
                    string variableName = var; // Mantiene el & si lo tiene
                    string processedVar = IsVariableSDT(RemoveAmpersand(variableName)) ? $"{variableName}.ToJson()" : variableName;

                    formats.Add($"{variableName} %{formats.Count + 1}");
                    processedVariables.Add(processedVar);
                }

                var line = $"msg(Format(\"{string.Join(", ", formats)}\", {string.Join(", ", processedVariables)}), status)";

                if (isInRules)
                    line += ";";

                msgLines.Add(line);
            }

            return msgLines;
        }



        private string RemoveAmpersand(string variable)
        {
            return variable.StartsWith("&") ? variable.Substring(1) : variable;
        }

        private bool IsVariableSDT(string variableName)
        {
            KBObjectPart currentPart = LSI.Packages.Extensiones.Utilidades.Entorno.CurrentEditingPart;
            if (currentPart == null)
            {
                CommonServices.Output.AddLine("No se encontró el contexto actual.");
                return false;
            }

            VariablesPart variablesPart = currentPart.KBObject.Parts.Get<VariablesPart>();
            if (variablesPart == null)
            {
                CommonServices.Output.AddLine("No se encontró la sección de variables.");
                return false;
            }

            Variable variable = variablesPart.Variables.FirstOrDefault(v => v.Name.Equals(variableName, StringComparison.OrdinalIgnoreCase));
            if (variable == null)
            {
                CommonServices.Output.AddLine($"La variable '{variableName}' no fue encontrada.");
                return false;
            }

            // Verificar si el tipo de la variable es un SDT o un Objeto Externo
            bool isSDT = variable.Type == eDBType.GX_SDT;
            bool isExternalObject = variable.Type == eDBType.GX_EXTERNAL_OBJECT;

            CommonServices.Output.AddLine($"La variable '{variableName}' es un SDT: {isSDT}, es un Objeto Externo: {isExternalObject}");

            return isSDT || isExternalObject;
        }


    }
}

