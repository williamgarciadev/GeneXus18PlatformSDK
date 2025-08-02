using Acme.Packages.Menu.Core.Domain.Entities;
using Artech.Architecture.Common.Objects;

namespace Acme.Packages.Menu.Core.Domain.Interfaces
{
    public interface IVariableRepository
    {
        bool IsVariableDefined(string variableName, KBObjectPart currentPart);
        void AddVariable(VariableDefinition variableDefinition, KBObjectPart currentPart);
        void SaveChanges(KBObjectPart currentPart);
    }
}