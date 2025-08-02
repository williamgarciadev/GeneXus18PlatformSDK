using System;
using System.Collections.Generic;
using System.Linq;
using Artech.Genexus.Common.Parts;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Types;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common;
using Artech.Architecture.UI.Framework.Services;
using Artech.Architecture.Common.Services;
using Artech.Architecture.UI.Framework.Objects;
using System.Windows.Forms;
using Artech.Common.Framework.Commands;

namespace Acme.Packages.Menu.Utilities
{

    public static class VariableHelper
    {
       
        public static bool IsVariableDefined(string variableName, KBObjectPart currentPart)
        {
            if (currentPart == null)
                return false;

            VariablesPart variablesPart = currentPart.KBObject.Parts.Get<VariablesPart>();
            return variablesPart?.Variables.Any(v => v.Name.Equals(variableName, StringComparison.OrdinalIgnoreCase)) == true;
        }

        public static void AddVariable(string variableName, KBObjectPart currentPart, eDBType type, int length)
        {
            if (currentPart == null)
                return;

            VariablesPart variablesPart = currentPart.KBObject.Parts.Get<VariablesPart>();
            if (variablesPart == null)
                return;

            if (!variablesPart.Variables.Any(v => v.Name.Equals(variableName, StringComparison.OrdinalIgnoreCase)))
            {
                Variable newVar = new Variable(variableName, variablesPart) { Type = type, Length = length };
                variablesPart.Add(newVar);
            }

            //**Guardar los cambios**
            KBObjectSavePreferences savePreferences = new KBObjectSavePreferences(KBObjectSavePreferences.ForcedSave)
           {
               SkipValidation = false
           };
            currentPart.KBObject.Save(savePreferences);
        }

        public static (eDBType, int) GetTypeAndLengthFromVariable(string reference)
        {
            KBModel currentModel = UIServices.KB.CurrentModel;
            if (currentModel == null)
            {
                Utils.Log("❌ No se encontró el modelo actual.");
                return (eDBType.NONE, 0);
            }

            // Buscar en Atributos
            var attribute = Artech.Genexus.Common.Objects.Attribute.Get(currentModel, reference);
            if (attribute != null)
            {
                Utils.Log($"✅ Atributo encontrado: '{reference}', Tipo: {attribute.Type}, Longitud: {attribute.Length}");
                return (attribute.Type, attribute.Length);
            }

            // Buscar en Dominios
            var domain = Domain.Get(currentModel, new QualifiedName(reference));
            if (domain != null)
            {
                Utils.Log($"✅ Dominio encontrado: '{reference}', Tipo: {domain.Type}, Longitud: {domain.Length}");
                return (domain.Type, domain.Length);
            }

            Utils.Log($"⚠ No se encontró ninguna referencia válida para '{reference}'.");
            return (eDBType.NONE, 0);
        }
        public static (eDBType, int) GetTypeAndLengthFromKB(string reference)
        {
            KBModel currentModel = UIServices.KB.CurrentModel;
            if (currentModel == null)
            {
                Utils.Log("❌ No se encontró el modelo actual.");
                return (eDBType.NONE, 0);
            }

            // 🔹 **Paso 1: Buscar en Atributos**
            var attribute = Artech.Genexus.Common.Objects.Attribute.Get(currentModel, reference);
            if (attribute != null)
            {
                Utils.Log($"✅ Atributo encontrado: '{reference}', Tipo: {attribute.Type}, Longitud: {attribute.Length}");
                return (attribute.Type, attribute.Length);
            }

            // 🔹 **Paso 2: Buscar en Dominios**
            var domain = Domain.Get(currentModel, new QualifiedName(reference));
            if (domain != null)
            {
                Utils.Log($"✅ Dominio encontrado: '{reference}', Tipo: {domain.Type}, Longitud: {domain.Length}");
                return (domain.Type, domain.Length);
            }

            // 🔹 **Paso 3: Buscar en DataTypes Extendidos**
            string extendedType = GetExtendedDataType(reference);
            if (!string.IsNullOrEmpty(extendedType))
            {
                Utils.Log($"✅ DataType extendido encontrado: '{reference}', Tipo: External Object");
                return (eDBType.GX_EXTERNAL_OBJECT, 0);
            }

            // ❌ Si no se encontró en ninguna parte
            Utils.Log($"⚠ No se encontró ninguna referencia válida para '{reference}'.");
            return (eDBType.NONE, 0);
        }


        private static string GetExtendedDataType(string dataTypeName)
        {
            KBModel currentModel = UIServices.KB.CurrentModel;
            if (currentModel == null)
            {
                CommonServices.Output.AddLine("No se encontró el modelo actual.");
                return null;
            }

            var dataTypes = DataTypeProvider.GetProvider(currentModel).GetTypes(currentModel, ObjectBaseTypeValue.Extended);

            // Convertimos el nombre a minúsculas para hacer una comparación sin distinción de mayúsculas/minúsculas
            string lowerDataTypeName = dataTypeName.ToLower();

            // Usamos dt.FullName en lugar de dt.Name.ObjectName
            var foundType = dataTypes.FirstOrDefault(dt => dt.FullName.Equals(lowerDataTypeName, StringComparison.OrdinalIgnoreCase));

            return foundType?.FullName; // Retornamos el nombre del tipo correctamente
        }

        
        public static (eDBType, int) GetTypeFromPrefix(char prefix)
        {
            switch (prefix)
            {
                case 'N': return (eDBType.NUMERIC, 10);      // Numérico (10 dígitos)
                case 'V': return (eDBType.VARCHAR, 50);      // Cadena variable (50 caracteres)
                case 'C': return (eDBType.CHARACTER, 20);    // Carácter fijo (20 caracteres)
                case 'D': return (eDBType.DATE, 0);          // Fecha
                case 'T': return (eDBType.DATETIME, 0);      // Fecha y Hora
                case 'B': return (eDBType.Boolean, 0);       // Booleano
                case 'S': return (eDBType.GX_SDT, 0);        // SDT (Tipo estructurado)
                case 'L': return (eDBType.LONGVARCHAR, 2000);// Long Text
                case 'G': return (eDBType.GUID, 36);         // GUID (36 caracteres)
                default: return (eDBType.NONE, 0);           // No reconocido
            }
        }
        public static (eDBType, int, bool) GetTypeAndLengthFromReference(string reference)
        {
            KBModel currentModel = UIServices.KB.CurrentModel;
            if (currentModel == null)
            {
                Utils.Log("❌ No se encontró el modelo actual.");
                return (eDBType.NONE, 0, false);
            }

            // 🔹 **Paso 1: Buscar en Atributos**
            var attribute = Artech.Genexus.Common.Objects.Attribute.Get(currentModel, reference);
            if (attribute != null)
            {
                Utils.Log($"✅ Atributo encontrado: '{reference}', Tipo: {attribute.Type}, Longitud: {attribute.Length}");
                return (attribute.Type, attribute.Length, true);
            }

            // 🔹 **Paso 2: Buscar en Dominios**
            var domain = Domain.Get(currentModel, new QualifiedName(reference));
            if (domain != null)
            {
                Utils.Log($"✅ Dominio encontrado: '{reference}', Tipo: {domain.Type}, Longitud: {domain.Length}");
                return (domain.Type, domain.Length, true);
            }

            // 🔹 **Paso 3: Buscar en DataTypes Extendidos**
            string extendedType = GetExtendedDataType(reference);
            if (!string.IsNullOrEmpty(extendedType))
            {
                Utils.Log($"✅ DataType extendido encontrado: '{reference}', Tipo: External Object");
                return (eDBType.GX_EXTERNAL_OBJECT, 0, true);
            }

            // ❌ No se encontró la referencia
            Utils.Log($"⚠ No se encontró ninguna referencia válida para '{reference}'.");
            return (eDBType.NONE, 0, false);
        }

        //public static void AddVariableBasedOn(string variableName, KBObjectPart currentPart, string baseReference)
        //{
        //    if (currentPart == null)
        //        return;

        //    VariablesPart variablesPart = currentPart.KBObject.Parts.Get<VariablesPart>();
        //    if (variablesPart == null)
        //        return;

        //    if (!variablesPart.Variables.Any(v => v.Name.Equals(variableName, StringComparison.OrdinalIgnoreCase)))
        //    {
        //        Variable newVar = new Variable(variableName, variablesPart)
        //        {
        //            AttributeBasedOn = Artech.Genexus.Common.Objects.Attribute.Get(UIServices.KB.CurrentModel, baseReference)
        //        };

        //        if (newVar.AttributeBasedOn != null)
        //        {
        //            variablesPart.Add(newVar);
        //        }
        //        else
        //        {
        //            Domain domain = Domain.Get(UIServices.KB.CurrentModel, new QualifiedName(baseReference));
        //            if (domain != null)
        //            {
        //                newVar.DomainBasedOn = domain;
        //                variablesPart.Add(newVar);
        //            }
        //            else
        //            {
        //                Utils.Log($"⚠ No se pudo establecer la base de la variable '{variableName}' en '{baseReference}'.");
        //                return;
        //            }
        //        }

        //        // **Guardar cambios en la KB**
        //        KBObjectSavePreferences savePreferences = new KBObjectSavePreferences(KBObjectSavePreferences.ForcedSave)
        //        {
        //            SkipValidation = true
        //        };
        //        currentPart.KBObject.Save(savePreferences);

        //        // **Cerrar y reabrir el objeto para reflejar cambios en la UI**
        //        if (UIServices.DocumentManager.IsOpenDocument(currentPart.KBObject))
        //        {
        //            UIServices.DocumentManager.CloseDocument(currentPart.KBObject, false); // Cierra el objeto sin descartar cambios
        //            UIServices.DocumentManager.OpenDocument(currentPart.KBObject, new OpenDocumentOptions()); // Lo reabre
        //        }

        //        Utils.Log($"✅ Variable '{variableName}' creada basada en '{baseReference}' y reflejada en la UI inmediatamente.");
        //    }
        //}


        public static void AddVariableBasedOn(string variableName, KBObjectPart currentPart, string baseReference, CommandData commandData)
        {
            if (currentPart == null)
                return;

            VariablesPart variablesPart = currentPart.KBObject.Parts.Get<VariablesPart>();
            if (variablesPart == null)
                return;

            if (!variablesPart.Variables.Any(v => v.Name.Equals(variableName, StringComparison.OrdinalIgnoreCase)))
            {
                Variable newVar = new Variable(variableName, variablesPart)
                {
                    AttributeBasedOn = Artech.Genexus.Common.Objects.Attribute.Get(UIServices.KB.CurrentModel, baseReference)
                };

                if (newVar.AttributeBasedOn != null)
                {
                    variablesPart.Add(newVar);
                }
                else
                {
                    Domain domain = Domain.Get(UIServices.KB.CurrentModel, new QualifiedName(baseReference));
                    if (domain != null)
                    {
                        newVar.DomainBasedOn = domain;
                        variablesPart.Add(newVar);
                    }
                    else
                    {
                        Utils.Log($"⚠ No se pudo establecer la base de la variable '{variableName}' en '{baseReference}'.");
                        return;
                    }
                }

                // **Guardar cambios en la KB**
                KBObjectSavePreferences savePreferences = new KBObjectSavePreferences
                {
                    ForceSaveDefaultParts = true,
                    SkipValidation = true,
                    SkipChecksum = true
                };

                currentPart.KBObject.Save(savePreferences);
                Utils.Log($"✅ Variable '{variableName}' creada basada en '{baseReference}'.");

                // **Obtener el texto seleccionado del editor con commandData**
                string selectedText = Utils.GetSelectedTextSafe(commandData);
                if (!string.IsNullOrEmpty(selectedText))
                {
                    string oldVariable = $"&{baseReference}_{variableName}";
                    string newVariable = $"&{variableName}";

                    if (selectedText.Contains(oldVariable))
                    {
                        string updatedContent = selectedText.Replace(oldVariable, newVariable);

                        // **Copiar al portapapeles y pegar en el editor**
                        Clipboard.SetText(updatedContent);
                        UIServices.CommandDispatcher.Dispatch(Artech.Architecture.UI.Framework.Commands.CommandKeys.Core.Paste);
                        Utils.Log($"🔄 Reemplazado '{oldVariable}' por '{newVariable}' en el editor.");
                    }
                }

                // **Actualizar la UI sin cerrar el objeto**
                if (UIServices.EditorManager != null)
                {
                    var view = UIServices.EditorManager.GetEditor(currentPart.KBObject.Guid) as IGxView;
                    if (view != null)
                    {
                        view.UpdateData(); // Recargar datos
                        view.UpdateView(); // Refrescar la UI
                        Utils.Log($"🔄 UI actualizada para reflejar la nueva variable '{variableName}'.");
                    }
                }
            }
        }















    }
}
