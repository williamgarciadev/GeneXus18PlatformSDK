using System;
using System.Collections.Generic;
using Artech.Common.Framework.Commands;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common;
using Acme.Packages.Menu.Models;
using Acme.Packages.Menu.Utilities;
using LSI.Packages.Extensiones.Utilidades;

namespace Acme.Packages.Menu.Services.Variables
{
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

            var variableInfo = DetermineVariableType(variableName);
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

        private VariableInfo DetermineVariableType(string variableName)
        {
            // 1. Si contiene "_", es el formato explícito "Atributo_Variable"
            if (variableName.Contains("_"))
                return ResolveAttributeBasedVariable(variableName);

            // 2. Intentar Búsqueda Semántica: ¿Existe un atributo o dominio con este nombre exacto?
            var (dataType, fieldLength, isAttributeBased) = VariableHelper.GetTypeAndLengthFromReference(variableName);
            if (isAttributeBased)
            {
                Utils.Log($"🧠 Inteligencia Semántica: Detectado Atributo/Dominio '{variableName}' en la KB.");
                return new VariableInfo
                {
                    CleanName = variableName,
                    Type = dataType,
                    Length = fieldLength,
                    BaseReference = variableName,
                    IsBasedOnAttributeOrDomain = true
                };
            }

            // 3. Fallback a Prefijos (N, V, C, etc.)
            return ResolveTypeFromPrefix(variableName);
        }

        private VariableInfo ResolveAttributeBasedVariable(string variableName)
        {
            var nameParts = variableName.Split(new char[] { '_' }, 2, StringSplitOptions.None);
            var attributeReference = nameParts[0].Trim();
            var targetVariableName = nameParts[1].Trim();

            Utils.Log($"🔹 Creando variable '{targetVariableName}' basada en atributo '{attributeReference}'");

            var (dataType, fieldLength, isAttributeBased) = VariableHelper.GetTypeAndLengthFromReference(attributeReference);

            return new VariableInfo
            {
                CleanName = targetVariableName,
                Type = dataType,
                Length = fieldLength,
                BaseReference = attributeReference,
                IsBasedOnAttributeOrDomain = isAttributeBased
            };
        }

        private VariableInfo ResolveTypeFromPrefix(string variableName)
        {
            ValidateVariableNameLength(variableName);

            var typePrefix = variableName[0];
            var actualVariableName = variableName.Substring(1).Trim();

            Utils.Log($"🔹 Prefijo de tipo detectado: '{typePrefix}', Variable: '{actualVariableName}'");

            var (dataType, defaultLength) = VariableHelper.GetTypeFromPrefix(typePrefix);

            return new VariableInfo
            {
                CleanName = actualVariableName,
                Type = dataType,
                Length = defaultLength,
                BaseReference = null,
                IsBasedOnAttributeOrDomain = false
            };
        }

        private void ValidateVariableNameLength(string variableName)
        {
            if (variableName.Length <= 1)
            {
                throw new ArgumentException("El nombre de la variable debe tener más de un carácter para incluir prefijo y nombre.");
            }
        }

        private void UpdateEditor(CommandData commandData, string oldText, string newText)
        {
            Utils.ReplaceSelectedTextInEditor(commandData, oldText, newText);
        }

        private void CreateVariableInKB(VariableInfo varInfo, KBObjectPart currentPart, CommandData commandData)
        {
            ValidateVariableInfo(varInfo);

            var fullVariableName = $"&{varInfo.CleanName}";

            if (!VariableHelper.IsVariableDefined(fullVariableName, currentPart))
            {
                CreateNewVariable(varInfo, currentPart, commandData);
            }
            else
            {
                Utils.Log($"📝 Variable '{fullVariableName}' ya existe, no se creará duplicado.");
            }
        }

        private void ValidateVariableInfo(VariableInfo varInfo)
        {
            if (varInfo == null)
                throw new ArgumentNullException(nameof(varInfo), "La información de la variable no puede ser nula.");

            if (string.IsNullOrWhiteSpace(varInfo.CleanName))
                throw new ArgumentException("El nombre de la variable no puede estar vacío.", nameof(varInfo));

            if (varInfo.Type == eDBType.NONE)
                throw new InvalidOperationException($"No se encontró un tipo válido para la variable '{varInfo.BaseReference ?? varInfo.CleanName}'.");

            // Validar compatibilidad con GeneXus: solo tipos escalares
            ValidateGeneXusCompatibility(varInfo);
        }

        private void ValidateGeneXusCompatibility(VariableInfo varInfo)
        {
            // Validar que el tipo sea compatible con GeneXus (solo escalares)
            var compatibleTypes = new List<eDBType>
            { 
                eDBType.CHARACTER, eDBType.VARCHAR, eDBType.LONGVARCHAR,
                eDBType.NUMERIC, eDBType.DATE, eDBType.DATETIME,
                eDBType.Boolean, eDBType.GUID
            };

            if (!compatibleTypes.Contains(varInfo.Type))
            {
                throw new InvalidOperationException(
                    $"El tipo '{varInfo.Type}' no es compatible con GeneXus. " +
                    "Solo se soportan tipos escalares: CHARACTER, VARCHAR, NUMERIC, DATE, DATETIME, Boolean, GUID.");
            }
        }

        private void CreateNewVariable(VariableInfo varInfo, KBObjectPart currentPart, CommandData commandData)
        {
            if (varInfo.IsBasedOnAttributeOrDomain)
            {
                VariableHelper.AddVariableBasedOn(varInfo.CleanName, currentPart, varInfo.BaseReference, commandData);
                Utils.Log($"✅ Variable GeneXus '{varInfo.CleanName}' creada basada en atributo/dominio '{varInfo.BaseReference}'.");
            }
            else
            {
                VariableHelper.AddVariable(varInfo.CleanName, currentPart, varInfo.Type, varInfo.Length);
                Utils.Log($"✅ Variable GeneXus '{varInfo.CleanName}' creada con tipo escalar {varInfo.Type} (longitud: {varInfo.Length}).");
            }
        }
    }
}