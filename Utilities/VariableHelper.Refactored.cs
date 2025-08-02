using System;
using Acme.Packages.Menu.Presentation.Facades;
using Artech.Architecture.Common.Objects;
using Artech.Common.Framework.Commands;
using Artech.Genexus.Common.Types;

namespace Acme.Packages.Menu.Utilities
{
    /// <summary>
    /// Helper para operaciones de variables (Versión Refactorizada)
    /// Mantiene compatibilidad hacia atrás mientras usa la nueva arquitectura SOLID
    /// </summary>
    public static class VariableHelperRefactored
    {
        #region Métodos Públicos (Compatibilidad hacia atrás)

        /// <summary>
        /// Verifica si una variable está definida
        /// </summary>
        public static bool IsVariableDefined(string variableName, KBObjectPart currentPart)
        {
            return VariableHelperFacade.IsVariableDefined(variableName, currentPart);
        }

        /// <summary>
        /// Agrega una variable con tipo y longitud específicos
        /// </summary>
        public static void AddVariable(string variableName, KBObjectPart currentPart, eDBType type, int length)
        {
            VariableHelperFacade.AddVariable(variableName, currentPart, type, length);
        }

        /// <summary>
        /// Obtiene tipo basado en prefijo
        /// </summary>
        public static (eDBType, int) GetTypeFromPrefix(char prefix)
        {
            return VariableHelperFacade.GetTypeFromPrefix(prefix);
        }

        /// <summary>
        /// Resuelve tipo desde referencia en la KB
        /// </summary>
        public static (eDBType, int, bool) GetTypeAndLengthFromReference(string reference)
        {
            return VariableHelperFacade.GetTypeAndLengthFromReference(reference);
        }

        /// <summary>
        /// Método legacy - usar CreateVariableFromReference en su lugar
        /// </summary>
        [Obsolete("Use CreateVariableFromReference instead")]
        public static (eDBType, int) GetTypeAndLengthFromVariable(string reference)
        {
            var (type, length, found) = VariableHelperFacade.GetTypeAndLengthFromReference(reference);
            return (type, length);
        }

        /// <summary>
        /// Método legacy - usar CreateVariableFromReference en su lugar
        /// </summary>
        [Obsolete("Use CreateVariableFromReference instead")]
        public static (eDBType, int) GetTypeAndLengthFromKB(string reference)
        {
            var (type, length, found) = VariableHelperFacade.GetTypeAndLengthFromReference(reference);
            return (type, length);
        }

        /// <summary>
        /// Crea variable basada en referencia - Versión simplificada sin CommandData
        /// </summary>
        public static void CreateVariableFromReference(string variableName, KBObjectPart currentPart, string baseReference)
        {
            VariableHelperFacade.CreateVariableFromReference(variableName, currentPart, baseReference);
        }

        /// <summary>
        /// Método legacy con CommandData - mantenido para compatibilidad
        /// </summary>
        [Obsolete("Use CreateVariableFromReference without CommandData parameter")]
        public static void AddVariableBasedOn(string variableName, KBObjectPart currentPart, string baseReference, CommandData commandData)
        {
            // Por ahora simplemente llamamos al método sin CommandData
            // En el futuro se puede implementar la lógica de UI por separado
            VariableHelperFacade.CreateVariableFromReference(variableName, currentPart, baseReference);
            
            // TODO: Implementar lógica de UI en un servicio separado
            // - Reemplazo de texto en editor
            // - Actualización de UI
            // - Manejo de clipboard
        }

        #endregion

        #region Métodos Nuevos (API Mejorada)

        /// <summary>
        /// Crea variable usando prefijo del primer carácter
        /// </summary>
        public static void CreateVariableFromPrefix(string variableName, KBObjectPart currentPart, char prefix)
        {
            VariableHelperFacade.CreateVariableFromPrefix(variableName, currentPart, prefix);
        }

        /// <summary>
        /// Valida si un prefijo es válido
        /// </summary>
        public static bool IsValidPrefix(char prefix)
        {
            var (type, _) = VariableHelperFacade.GetTypeFromPrefix(prefix);
            return type != eDBType.NONE;
        }

        #endregion
    }
}