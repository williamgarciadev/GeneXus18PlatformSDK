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
            // 1. Intentar método robusto vía Reflection (Estilo LSI/Descartes)
            string reflectedText = GetSelectedTextReflected(commandData);
            if (!string.IsNullOrEmpty(reflectedText)) return reflectedText;

            // 2. Intentar obtenerlo directamente del control del Editor (Estilo Descartes/WWP)
            try
            {
                KBObjectPart currentPart = LSI.Packages.Extensiones.Utilidades.Entorno.CurrentEditingPart;
                if (currentPart != null && UIServices.EditorManager != null)
                {
                    object editorObj = UIServices.EditorManager.GetEditor(currentPart.Guid);
                    if (editorObj != null)
                    {
                        // ITextView define SelectedText. Intentamos obtenerla por reflexión.
                        var prop = editorObj.GetType().GetProperty("SelectedText");
                        if (prop != null)
                        {
                            string sel = prop.GetValue(editorObj)?.ToString();
                            if (!string.IsNullOrWhiteSpace(sel)) return sel.Trim();
                        }
                    }
                }
            }
            catch { }

            // 3. Fallback: Extensión LSI (si está disponible y cargada)
            try 
            {
                string lsiText = CommandDataExtensions.LsiGetSelectedText(commandData);
                if (!string.IsNullOrWhiteSpace(lsiText)) return lsiText;
            }
            catch { }

            return string.Empty;
        }

        private static string GetSelectedTextReflected(CommandData commandData)
        {
            try
            {
                if (commandData == null || commandData.Context == null) return null;

                object context = commandData.Context;
                object textEditor = null;

                // Intentar obtener StandaloneTextEditor directamente o navegando la jerarquía
                // Lógica inspirada en LSI.Packages.Extensiones.Utilidades
                
                // Caso 1: Context es StandaloneTextEditor
                if (IsType(context, "StandaloneTextEditor"))
                {
                    textEditor = context;
                }
                // Caso 2: Virtual Editors (Events, Conditions, Rules)
                else if (IsType(context, "VirtualEventsEditor") || IsType(context, "VirtualConditionsEditor") || IsType(context, "VirtualRulesEditor"))
                {
                    // Estos suelen tener un editor de texto interno o ser wrappers
                    // En la lógica decompilada, retornaban null si era esto directamente, 
                    // pero a veces el contexto es un Container.
                }
                // Caso 3: BaseEditorContainer -> SourceEditor -> StandaloneTextEditor
                else if (IsType(context, "BaseEditorContainer"))
                {
                    // context.GetEditor(null) -> SourceEditor
                    var getEditorMethod = context.GetType().GetMethod("GetEditor", new Type[] { typeof(Type) });
                    if (getEditorMethod != null)
                    {
                        object sourceEditor = getEditorMethod.Invoke(context, new object[] { null });
                        if (sourceEditor != null && IsType(sourceEditor, "SourceEditor"))
                        {
                            // sourceEditor.GetEditor(null) -> StandaloneTextEditor
                            var getInnerEditorMethod = sourceEditor.GetType().GetMethod("GetEditor", new Type[] { typeof(Type) });
                            if (getInnerEditorMethod != null)
                            {
                                textEditor = getInnerEditorMethod.Invoke(sourceEditor, new object[] { null });
                            }
                        }
                    }
                }

                if (textEditor != null && IsType(textEditor, "StandaloneTextEditor"))
                {
                    var selectedTextProp = textEditor.GetType().GetProperty("SelectedText");
                    if (selectedTextProp != null)
                    {
                        string text = selectedTextProp.GetValue(textEditor) as string;
                        if (!string.IsNullOrWhiteSpace(text)) return text.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error en GetSelectedTextReflected: " + ex.Message);
            }
            return null;
        }

        private static bool IsType(object obj, string typeName)
        {
            if (obj == null) return false;
            Type t = obj.GetType();
            while (t != null)
            {
                if (t.Name == typeName) return true;
                t = t.BaseType;
            }
            return false;
        }

        public static string GetTextViaClipboard()
        {
            try 
            { 
                Clipboard.Clear(); 
                UIServices.CommandDispatcher.Dispatch(Artech.Architecture.UI.Framework.Commands.CommandKeys.Core.Copy);
                
                for (int i = 0; i < 5; i++)
                {
                    System.Threading.Thread.Sleep(50);
                    if (Clipboard.ContainsText()) return Clipboard.GetText().Trim();
                }
                return "";
            } 
            catch { return ""; }
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
            string pattern = @"&\w+"; 
            char[] separators = new char[] { ',', '\n', '\r' };

            if (string.IsNullOrEmpty(inputText))
                return variables.ToList();

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

        public static void ShowResultForm(List<string> logLines)
        {
            // Intentamos usar el formulario local si existe
            // Si no, mostramos un aviso
            MessageBox.Show(string.Join(Environment.NewLine, logLines), "Resultado");
        }

        public static List<string> GenerateMsgFormatLines(List<string> variables, bool isInRules = false)
        {
            List<string> msgLines = new List<string>();
            int batchSize = 7;

            if (!variables.Contains("&Pgmname"))
            {
                variables.Insert(0, "&Pgmname");
            }
            else
            {
                variables.Remove("&Pgmname");
                variables.Insert(0, "&Pgmname");
            }

            for (int i = 0; i < variables.Count; i += batchSize)
            {
                var batch = variables.Skip(i).Take(batchSize).ToList();
                var formats = new List<string>();
                var processedVariables = new List<string>();

                foreach (var varName in batch)
                {
                    string processedVar = IsVariableSDT(RemoveAmpersand(varName)) ? $"{varName}.ToJson()" : varName;
                    formats.Add($"{varName} %{formats.Count + 1}");
                    processedVariables.Add(processedVar);
                }

                var line = $"msg(Format(\"{string.Join(", ", formats)}\", {string.Join(", ", processedVariables)}), status)";
                if (isInRules) line += ";";
                msgLines.Add(line);
            }

            return msgLines;
        }

        private static bool IsVariableSDT(string variableName)
        {
            try 
            {
                KBObjectPart currentPart = LSI.Packages.Extensiones.Utilidades.Entorno.CurrentEditingPart;
                if (currentPart == null) return false;

                VariablesPart variablesPart = currentPart.KBObject.Parts.Get<VariablesPart>();
                if (variablesPart == null) return false;

                Variable variable = variablesPart.Variables.FirstOrDefault(v => v.Name.Equals(variableName, StringComparison.OrdinalIgnoreCase));
                if (variable == null) return false;

                return variable.Type == eDBType.GX_SDT || variable.Type == eDBType.GX_EXTERNAL_OBJECT;
            }
            catch { return false; }
        }

        public static List<string> GenerateLogDebugLines(List<string> variables)
        {
            return variables.Where(variable => !string.IsNullOrEmpty(variable))
                            .Select(variable =>
                            {
                                string variableName = IsVariableSDT(RemoveAmpersand(variable)) ? $"{variable}.ToJson()" : variable;
                                return $"Log.Debug(Format(\"{variable}=%1\", {variableName}), '{RemoveAmpersand(variable)}')";
                            })
                            .ToList();
        }

        public static void ReplaceSelectedTextInEditor(CommandData commandData, string oldText, string newText)
        {
            try
            {
                string selectedText = GetSelectedTextSafe(commandData);
                if (string.IsNullOrEmpty(selectedText) || !selectedText.Contains(oldText))
                {
                    Log($"⚠ No se encontró '{oldText}' en la selección.");
                    return;
                }

                string updatedText = selectedText.Replace(oldText, newText);
                Clipboard.SetText(updatedText);
                UIServices.CommandDispatcher.Dispatch(Artech.Architecture.UI.Framework.Commands.CommandKeys.Core.Paste);
                Log($"🔄 Reemplazado '{oldText}' por '{newText}' en el editor.");
            }
            catch (Exception ex)
            {
                ShowError($"❌ Error al reemplazar texto en el editor: {ex.Message}");
            }
        }
    }
}