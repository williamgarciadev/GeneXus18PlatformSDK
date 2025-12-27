using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Framework.Commands;
using Artech.Architecture.Common.Services;
using LSI.Packages.Extensiones.Utilidades.GxClassExtensions;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common.Parts;
using Artech.Genexus.Common;

namespace Acme.Packages.Menu.Utilities
{


    public static class Utils
    {
        public static void Log(string message)
        {
            CommonServices.Output.AddLine(message);
            Console.WriteLine(message);
        }

        public static void ShowWarning(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static string GetSelectedTextSafe(CommandData commandData)
        {
            // 1. Intentar con la extensión de LSI (si está disponible y funciona)
            string selectedText = CommandDataExtensions.LsiGetSelectedText(commandData);
            if (!string.IsNullOrWhiteSpace(selectedText))
                return selectedText;

            // 2. Intentar accediendo directamente al Editor activo de GeneXus
            try 
            {
                if (UIServices.EditorManager != null)
                {
                    // Intentar obtener el objeto desde la parte que se está editando actualmente
                    KBObjectPart currentPart = LSI.Packages.Extensiones.Utilidades.Entorno.CurrentEditingPart;
                    KBObject currentObject = currentPart?.KBObject;

                    if (currentObject != null)
                    {
                        // Obtener la vista activa del documento
                        var view = UIServices.EditorManager.GetEditor(currentObject.Guid) as ITextEditor;
                        if (view != null)
                        {
                            // ITextEditor suele tener métodos para selección, pero depende de la implementación específica
                            // Si no es ITextEditor estándar, podríamos intentar reflexión o interfaces comunes
                            // Por ahora, confiamos más en el portapapeles como fallback universal
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error intentando acceder al editor directamente: {ex.Message}");
            }

            // 3. Fallback: Usar el portapapeles (Simular Ctrl+C)
            try
            {
                UIServices.CommandDispatcher.Dispatch(Artech.Architecture.UI.Framework.Commands.CommandKeys.Core.Copy);
                selectedText = Clipboard.GetText().Trim();
            }
            catch (Exception ex)
            {
                Log($"Error al intentar copiar del portapapeles: {ex.Message}");
            }
            
            return selectedText;
        }

        public static string RemoveAmpersand(string text)
        {
            return text.StartsWith("&") ? text.Substring(1) : text;
        }

        public static void PasteResultInEditor(List<string> logLines)
        {
            string resultText = string.Join(Environment.NewLine, logLines) + Environment.NewLine;
            Clipboard.SetText(resultText);
            UIServices.CommandDispatcher.Dispatch(new CommandKey(new Guid("98121D96-A7D8-468b-9310-B1F468F812AE"), "Paste"));
        }

        public static List<string> ExtractVariables(string inputText)
        {
            HashSet<string> variables = new HashSet<string>();
            string pattern = @"&\w+"; // Busca variables GeneXus con "&"
            char[] separators = new char[] { ',', '\n', '\r' };

            if (string.IsNullOrEmpty(inputText))
                return variables.ToList();

            // Divide el texto en posibles variables separadas por comas, saltos de línea, etc.
            var rawVariables = inputText.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(v => v.Trim())
                                        .Where(v => !string.IsNullOrEmpty(v))
                                        .ToList();

            foreach (var variable in rawVariables)
            {
                Match match = Regex.Match(variable, pattern);
                if (match.Success)
                {
                    variables.Add(match.Value);
                }
                else
                {
                    variables.Add(variable);
                }
            }

            return variables.ToList();
        }

        public static void ShowInfo(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public  static void ShowResultForm(List<string> logLines)
        {
            using (LogDebugResultForm resultForm = new LogDebugResultForm(logLines))
            {
                resultForm.ShowDialog();
            }
        }
        public static List<string> GenerateMsgFormatLines(List<string> variables, bool isInRules = false)
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

        private static bool IsVariableSDT(string variableName)
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

        public static List<string> GenerateLogDebugLines(List<string> variables)
        {
            return variables.Where(variable => !string.IsNullOrEmpty(variable))
                            .Select(variable =>
                            {
                                string variableName = Utility.IsVariableSDT(variable) ? $"{variable}.ToJson()" : variable;
                                return $"Log.Debug(Format(\"{variable}=%1\", {variableName}), '{RemoveAmpersand(variable)}')";
                            })
                            .ToList();
        }


        public static void ReplaceSelectedTextInEditor(CommandData commandData, string oldText, string newText)
        {
            try
            {
                // Obtener el texto seleccionado de manera segura
                string selectedText = Utils.GetSelectedTextSafe(commandData);
                if (string.IsNullOrEmpty(selectedText) || !selectedText.Contains(oldText))
                {
                    Utils.Log($"⚠ No se encontró '{oldText}' en la selección.");
                    return;
                }

                // Reemplazar en la selección
                string updatedText = selectedText.Replace(oldText, newText);

                // Copiar el texto modificado al portapapeles
                Clipboard.SetText(updatedText);

                // Enviar el comando de pegar para sobrescribir la selección
                UIServices.CommandDispatcher.Dispatch(Artech.Architecture.UI.Framework.Commands.CommandKeys.Core.Paste);

                Utils.Log($"🔄 Reemplazado '{oldText}' por '{newText}' en el editor.");
            }
            catch (Exception ex)
            {
                Utils.ShowError($"❌ Error al reemplazar texto en el editor: {ex.Message}");
            }
        }




    }
}
