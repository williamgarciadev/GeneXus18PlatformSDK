using System;
using System.Text.RegularExpressions;
using Acme.Packages.Menu.Core.Domain.Interfaces;

namespace Acme.Packages.Menu.Core.Application.Services
{
    public class SubroutineNavigatorService : ISubroutineNavigatorService
    {
        public string CleanSubroutineName(string selectedText)
        {
            if (string.IsNullOrEmpty(selectedText)) return "";

            // 1. Quitar posibles comentarios al final de la línea
            string text = Regex.Replace(selectedText, @"//.*", "").Trim();

            // 2. Intentar capturar el nombre dentro de un comando 'Do'
            // Soporta: Do 'SubName', Do "SubName" o Do SubName (sin comillas)
            var match = Regex.Match(text, @"Do\s+(['""]?)(?<name>[^'""\r\n]+)\1", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups["name"].Value.Trim();
            }

            // 3. Fallback: Si no hay 'Do', simplemente limpiar comillas y espacios de los extremos
            return text.Trim('\'', '"', ' ', '\t');
        }

        public int FindDefinitionLine(string sourceCode, string subroutineName)
        {
            if (string.IsNullOrEmpty(sourceCode) || string.IsNullOrEmpty(subroutineName)) return -1;

            string[] lines = sourceCode.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            string cleanTarget = subroutineName.Trim().ToLower();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim().ToLower();
                
                // Búsqueda robusta: 
                // Debe empezar con 'sub ' y contener el nombre de la subrutina
                if (line.StartsWith("sub ") && line.Contains(cleanTarget))
                {
                    // Verificamos que sea una definición real y no un comentario
                    // (el Trim ya quitó espacios iniciales)
                    if (line.Contains("sub") && !line.StartsWith("//"))
                    {
                        return i + 1;
                    }
                }
            }

            return -1;
        }
    }
}
