using Artech.Genexus.Common;

namespace Acme.Packages.Menu.Models
{
    /// <summary>
    /// Información de una variable extraída
    /// </summary>
    public class VariableInfo
    {
        public string CleanName { get; set; }
        public eDBType Type { get; set; }
        public int Length { get; set; }
        public string BaseReference { get; set; }
        public bool IsBasedOnAttributeOrDomain { get; set; }
    }
}