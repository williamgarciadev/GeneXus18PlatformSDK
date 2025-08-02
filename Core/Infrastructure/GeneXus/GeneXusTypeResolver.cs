using System;
using System.Linq;
using Acme.Packages.Menu.Core.Domain.Interfaces;
using Artech.Architecture.UI.Framework.Services;
using Artech.Genexus.Common;
using Artech.Genexus.Common.Types;
using Artech.Architecture.Common.Objects;

namespace Acme.Packages.Menu.Core.Infrastructure.GeneXus
{
    public class GeneXusTypeResolver : ITypeResolver
    {
        private readonly ILogger _logger;

        public GeneXusTypeResolver(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public (eDBType Type, int Length, bool Found) ResolveTypeFromReference(string reference)
        {
            var currentModel = UIServices.KB.CurrentModel;
            if (currentModel == null)
            {
                _logger.LogError("No se encontró el modelo actual.");
                return (eDBType.NONE, 0, false);
            }

            // Buscar en Atributos
            var attribute = Artech.Genexus.Common.Objects.Attribute.Get(currentModel, reference);
            if (attribute != null)
            {
                _logger.LogSuccess(string.Format("Atributo encontrado: '{0}', Tipo: {1}, Longitud: {2}", 
                    reference, attribute.Type, attribute.Length));
                return (attribute.Type, attribute.Length, true);
            }

            // Buscar en Dominios
            var domain = Artech.Genexus.Common.Objects.Domain.Get(currentModel, new QualifiedName(reference));
            if (domain != null)
            {
                _logger.LogSuccess(string.Format("Dominio encontrado: '{0}', Tipo: {1}, Longitud: {2}", 
                    reference, domain.Type, domain.Length));
                return (domain.Type, domain.Length, true);
            }

            // Buscar en DataTypes Extendidos
            var extendedType = GetExtendedDataType(reference, currentModel);
            if (!string.IsNullOrEmpty(extendedType))
            {
                _logger.LogSuccess(string.Format("DataType extendido encontrado: '{0}', Tipo: External Object", reference));
                return (eDBType.GX_EXTERNAL_OBJECT, 0, true);
            }

            _logger.LogWarning(string.Format("No se encontró ninguna referencia válida para '{0}'.", reference));
            return (eDBType.NONE, 0, false);
        }

        private string GetExtendedDataType(string dataTypeName, KBModel currentModel)
        {
            try
            {
                var dataTypes = DataTypeProvider.GetProvider(currentModel).GetTypes(currentModel, ObjectBaseTypeValue.Extended);
                var lowerDataTypeName = dataTypeName.ToLower();
                var foundType = dataTypes.FirstOrDefault(dt => dt.FullName.Equals(lowerDataTypeName, StringComparison.OrdinalIgnoreCase));
                return foundType?.FullName;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Error al buscar DataType extendido '{0}': {1}", dataTypeName, ex.Message));
                return null;
            }
        }
    }
}