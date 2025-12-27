namespace Acme.Packages.Menu.Core.Domain.DTOs
{
    public enum OccurrenceType
    {
        Read,   // 📖 Lectura
        Write   // 📝 Escritura (Asignación)
    }

    public class VariableOccurrenceDto
    {
        public int LineNumber { get; set; }
        public string PartName { get; set; } // Source, Rules, Events
        public string Context { get; set; }
        public OccurrenceType Type { get; set; }
        public string FullLine { get; set; }
    }
}
