using System.Text;
using System.Linq;
using Acme.Packages.Menu.Core.Domain.DTOs;
using Acme.Packages.Menu.Core.Domain.Interfaces;

namespace Acme.Packages.Menu.Core.Infrastructure.Formatters
{
    public class MarkdownDocumentationFormatter : IDocumentationFormatter
    {
        public string FileExtension => ".md";

        public string Format(ObjectDocumentationDto data)
        {
            var sb = new StringBuilder();

            sb.AppendLine("# " + data.Name);
            sb.AppendLine();
            sb.AppendLine("> " + (string.IsNullOrEmpty(data.Description) ? "Sin descripción" : data.Description));
            sb.AppendLine();
            sb.AppendLine("## ℹ️ Información General");
            sb.AppendLine("- **Tipo:** " + data.Type);
            if (!string.IsNullOrEmpty(data.Module))
                sb.AppendLine("- **Módulo:** " + data.Module);
            sb.AppendLine("- **KB:** " + data.KBName);
            sb.AppendLine("- **Última Modificación:** " + data.LastModified.ToString("g"));
            sb.AppendLine();

            if (data.Parameters != null && data.Parameters.Count > 0)
            {
                sb.AppendLine("## 📥 Parámetros (Rules)");
                sb.AppendLine("| Nombre | Acceso | Tipo | Descripción |");
                sb.AppendLine("| :--- | :--- | :--- | :--- |");
                foreach (var p in data.Parameters)
                {
                    sb.AppendLine("| " + p.Name + " | " + p.Access + " | " + p.Type + " | " + p.Description + " |");
                }
                sb.AppendLine();
            }

            if (data.Structure != null && data.Structure.Count > 0)
            {
                sb.AppendLine("## 🧱 Estructura de Tabla (Atributos)");
                
                var levels = data.Structure.Select(a => a.Level).Distinct();
                foreach (var levelName in levels)
                {
                    sb.AppendLine("### Nivel: " + levelName);
                    sb.AppendLine("| | Nombre | Tipo | Nulo | Descripción | Fórmula |");
                    sb.AppendLine("| :--- | :--- | :--- | :--- | :--- | :--- |");
                    
                    var levelAttributes = data.Structure.Where(a => a.Level == levelName);
                    foreach (var a in levelAttributes)
                    {
                        string icons = (a.IsKey ? "🔑" : "") + (a.IsForeignKey ? "🔗" : "");
                        string nullable = a.IsNullable ? "Sí" : "No";
                        sb.AppendLine("| " + icons + " | " + a.Name + " | " + a.Type + " | " + nullable + " | " + a.Description + " | " + a.Formula + " |");
                    }
                    sb.AppendLine();
                }
            }

            if (data.Variables != null && data.Variables.Count > 0)
            {
                sb.AppendLine("## 💎 Variables Relevantes");
                sb.AppendLine("| Nombre | Tipo | Descripción |");
                sb.AppendLine("| :--- | :--- | :--- |");
                foreach (var v in data.Variables)
                {
                    sb.AppendLine("| " + v.Name + " | " + v.Type + " | " + v.Description + " |");
                }
                sb.AppendLine();
            }

            sb.AppendLine("---");
            sb.AppendLine("*Generado automáticamente por GeneXus Menu Plugin*");

            return sb.ToString();
        }
    }
}
