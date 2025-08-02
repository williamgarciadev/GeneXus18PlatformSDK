using Artech.Genexus.Common.Types;

namespace Acme.Packages.Menu.Core.Domain.Entities
{
    public class VariableDefinition
    {
        public string Name { get; set; }
        public Artech.Genexus.Common.Types.eDBType Type { get; set; }
        public int Length { get; set; }
        public string BaseReference { get; set; }
        public bool IsBasedOnReference { get; set; }

        public VariableDefinition(string name, Artech.Genexus.Common.Types.eDBType type, int length, string baseReference = null)
        {
            Name = name;
            Type = type;
            Length = length;
            BaseReference = baseReference;
            IsBasedOnReference = !string.IsNullOrEmpty(baseReference);
        }
    }
}