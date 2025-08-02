using System.Collections.Generic;
using System.Linq;

namespace Acme.Packages.Menu.Services.Analysis
{
    /// <summary>
    /// Analizador de líneas de código individual
    /// </summary>
    internal class LineAnalyzer
    {
        private static readonly List<string> StructuralKeywords = new List<string>
            { "endfor", "endif", "enddo", "endcase", "else", "{", "}", "(" };

        public bool IsCommentLine(string line)
        {
            return !string.IsNullOrWhiteSpace(line) && line.StartsWith("//");
        }

        public bool IsOperativeLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return false;

            var cleanLine = line.Trim();
            if (string.IsNullOrEmpty(cleanLine)) return false;

            if (cleanLine.All(c => "{}();,".Contains(c))) return false;

            return !StructuralKeywords.Contains(cleanLine.ToLower());
        }
    }
}