using System;
using Acme.Packages.Menu.Core.Domain.Entities;
using Acme.Packages.Menu.Core.Domain.Interfaces;
using Acme.Packages.Menu.Core.Domain.ValueObjects;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common;

namespace Acme.Packages.Menu.Core.Application.Services
{
    public class VariableService
    {
        private readonly IVariableRepository _variableRepository;
        private readonly ITypeResolver _typeResolver;
        private readonly ILogger _logger;

        public VariableService(
            IVariableRepository variableRepository, 
            ITypeResolver typeResolver,
            ILogger logger)
        {
            _variableRepository = variableRepository ?? throw new ArgumentNullException(nameof(variableRepository));
            _typeResolver = typeResolver ?? throw new ArgumentNullException(nameof(typeResolver));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsVariableDefined(string variableName, KBObjectPart currentPart)
        {
            if (string.IsNullOrWhiteSpace(variableName) || currentPart == null)
                return false;

            return _variableRepository.IsVariableDefined(variableName, currentPart);
        }

        public void CreateVariableFromPrefix(string variableName, KBObjectPart currentPart, char prefix)
        {
            if (string.IsNullOrWhiteSpace(variableName) || currentPart == null)
            {
                _logger.LogError("Nombre de variable o parte del objeto no válidos.");
                return;
            }

            var (type, length) = TypePrefixMapping.GetTypeFromPrefix(prefix);
            
            if (type == eDBType.NONE)
            {
                _logger.LogWarning(string.Format("Prefijo '{0}' no reconocido para la variable '{1}'.", prefix, variableName));
                return;
            }

            var variableDefinition = new VariableDefinition(variableName, type, length);
            CreateVariable(variableDefinition, currentPart);
        }

        public void CreateVariableFromReference(string variableName, KBObjectPart currentPart, string baseReference)
        {
            if (string.IsNullOrWhiteSpace(variableName) || currentPart == null || string.IsNullOrWhiteSpace(baseReference))
            {
                _logger.LogError("Parámetros no válidos para crear variable basada en referencia.");
                return;
            }

            var (type, length, found) = _typeResolver.ResolveTypeFromReference(baseReference);
            
            if (!found)
            {
                _logger.LogWarning(string.Format("No se encontró referencia válida para '{0}'.", baseReference));
                return;
            }

            var variableDefinition = new VariableDefinition(variableName, type, length, baseReference);
            CreateVariable(variableDefinition, currentPart);
        }

        private void CreateVariable(VariableDefinition variableDefinition, KBObjectPart currentPart)
        {
            if (_variableRepository.IsVariableDefined(variableDefinition.Name, currentPart))
            {
                _logger.LogWarning(string.Format("La variable '{0}' ya existe.", variableDefinition.Name));
                return;
            }

            try
            {
                _variableRepository.AddVariable(variableDefinition, currentPart);
                _variableRepository.SaveChanges(currentPart);
                
                string baseInfo = variableDefinition.IsBasedOnReference 
                    ? string.Format(" basada en '{0}'", variableDefinition.BaseReference)
                    : "";
                    
                _logger.LogSuccess(string.Format("Variable '{0}'{1} creada exitosamente.", variableDefinition.Name, baseInfo));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Error al crear variable '{0}': {1}", variableDefinition.Name, ex.Message));
            }
        }
    }
}