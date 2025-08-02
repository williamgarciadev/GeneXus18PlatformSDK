using System;
using System.Linq;
using Acme.Packages.Menu.Core.Domain.Entities;
using Acme.Packages.Menu.Core.Domain.Interfaces;
using Artech.Architecture.Common.Objects;
using Artech.Architecture.UI.Framework.Services;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Parts;
using Artech.Genexus.Common;

namespace Acme.Packages.Menu.Core.Infrastructure.GeneXus
{
    public class GeneXusVariableRepository : IVariableRepository
    {
        public bool IsVariableDefined(string variableName, KBObjectPart currentPart)
        {
            if (currentPart == null || string.IsNullOrWhiteSpace(variableName))
                return false;

            var variablesPart = currentPart.KBObject.Parts.Get<VariablesPart>();
            return variablesPart?.Variables.Any(v => v.Name.Equals(variableName, StringComparison.OrdinalIgnoreCase)) == true;
        }

        public void AddVariable(VariableDefinition variableDefinition, KBObjectPart currentPart)
        {
            if (currentPart == null || variableDefinition == null)
                return;

            var variablesPart = currentPart.KBObject.Parts.Get<VariablesPart>();
            if (variablesPart == null)
                return;

            var newVar = new Variable(variableDefinition.Name, variablesPart)
            {
                Type = variableDefinition.Type,
                Length = variableDefinition.Length
            };

            // Si está basada en referencia, intentar establecer la referencia
            if (variableDefinition.IsBasedOnReference)
            {
                SetVariableReference(newVar, variableDefinition.BaseReference);
            }

            variablesPart.Add(newVar);
        }

        public void SaveChanges(KBObjectPart currentPart)
        {
            if (currentPart == null)
                return;

            var savePreferences = new KBObjectSavePreferences(KBObjectSavePreferences.ForcedSave)
            {
                SkipValidation = false
            };

            currentPart.KBObject.Save(savePreferences);
        }

        private void SetVariableReference(Variable variable, string baseReference)
        {
            // Intentar establecer basado en atributo
            var attribute = Artech.Genexus.Common.Objects.Attribute.Get(UIServices.KB.CurrentModel, baseReference);
            if (attribute != null)
            {
                variable.AttributeBasedOn = attribute;
                return;
            }

            // Intentar establecer basado en dominio
            var domain = Artech.Genexus.Common.Objects.Domain.Get(UIServices.KB.CurrentModel, new QualifiedName(baseReference));
            if (domain != null)
            {
                variable.DomainBasedOn = domain;
            }
        }
    }
}