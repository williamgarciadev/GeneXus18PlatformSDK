namespace Acme.Packages.Menu.Models
{
    /// <summary>
    /// Estadísticas de conteo de líneas de código
    /// </summary>
    internal class CodeStats
    {
        public int TotalLines { get; set; }
        public int NonCommentLines { get; set; }
        public int OperativeLines { get; set; }

        public void AddStats(CodeStats other)
        {
            TotalLines += other.TotalLines;
            NonCommentLines += other.NonCommentLines;
            OperativeLines += other.OperativeLines;
        }
    }
}