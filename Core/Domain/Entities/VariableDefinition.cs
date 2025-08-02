using Artech.Genexus.Common;
using Artech.Genexus.Common.Types;

namespace Acme.Packages.Menu.Core.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio que representa la definición de una variable GeneXus
    /// </summary>
    public class VariableDefinition
    {
        public string Name { get; set; }
        public eDBType Type { get; set; }
        public int Length { get; set; }
        public string BaseReference { get; set; }
        public bool IsBasedOnReference { get; set; }

        public VariableDefinition(string name, eDBType type, int length)
        {
            Name = name;
            Type = type;
            Length = length;
            IsBasedOnReference = false;
        }

        public VariableDefinition(string name, eDBType type, int length, string baseReference)
        {
            Name = name;
            Type = type;
            Length = length;
            BaseReference = baseReference;
            IsBasedOnReference = !string.IsNullOrWhiteSpace(baseReference);
        }
    }
}