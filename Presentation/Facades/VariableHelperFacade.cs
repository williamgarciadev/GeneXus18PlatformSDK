using Acme.Packages.Menu.Common.Factories;
using Acme.Packages.Menu.Core.Application.Services;
using Acme.Packages.Menu.Core.Domain.ValueObjects;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common;

namespace Acme.Packages.Menu.Presentation.Facades
{
    /// <summary>
    /// Facade para operaciones de variables, compatible con GeneXus.
    /// Reemplaza el VariableHelper.cs original aplicando principios SOLID.
    /// </summary>
    public static class VariableHelperFacade
    {
        private static VariableService _variableService;
        
        private static VariableService VariableService => 
            _variableService ?? (_variableService = ServiceFactory.GetVariableService());

        /// <summary>
        /// Verifica si una variable está definida en el objeto actual
        /// </summary>
        public static bool IsVariableDefined(string variableName, KBObjectPart currentPart)
        {
            return VariableService.IsVariableDefined(variableName, currentPart);
        }

        /// <summary>
        /// Agrega una variable basada en tipo y longitud específicos
        /// </summary>
        public static void AddVariable(string variableName, KBObjectPart currentPart, eDBType type, int length)
        {
            var variableDefinition = new Core.Domain.Entities.VariableDefinition(variableName, type, length);
            var variableRepository = ServiceFactory.GetVariableRepository();
            
            if (!variableRepository.IsVariableDefined(variableName, currentPart))
            {
                variableRepository.AddVariable(variableDefinition, currentPart);
                variableRepository.SaveChanges(currentPart);
            }
        }

        /// <summary>
        /// Obtiene tipo y longitud basado en prefijo
        /// </summary>
        public static (eDBType, int) GetTypeFromPrefix(char prefix)
        {
            return TypePrefixMapping.GetTypeFromPrefix(prefix);
        }

        /// <summary>
        /// Resuelve tipo desde referencia en la KB
        /// </summary>
        public static (eDBType, int, bool) GetTypeAndLengthFromReference(string reference)
        {
            var typeResolver = ServiceFactory.GetTypeResolver();
            return typeResolver.ResolveTypeFromReference(reference);
        }

        /// <summary>
        /// Crea variable basada en prefijo del primer carácter
        /// </summary>
        public static void CreateVariableFromPrefix(string variableName, KBObjectPart currentPart, char prefix)
        {
            VariableService.CreateVariableFromPrefix(variableName, currentPart, prefix);
        }

        /// <summary>
        /// Crea variable basada en referencia (atributo o dominio)
        /// </summary>
        public static void CreateVariableFromReference(string variableName, KBObjectPart currentPart, string baseReference)
        {
            VariableService.CreateVariableFromReference(variableName, currentPart, baseReference);
        }
    }
}