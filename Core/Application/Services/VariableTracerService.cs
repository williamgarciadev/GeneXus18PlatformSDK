using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Acme.Packages.Menu.Core.Domain.DTOs;
using Acme.Packages.Menu.Core.Domain.Interfaces;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common.Parts;

namespace Acme.Packages.Menu.Core.Application.Services
{
    public class VariableTracerService : IVariableTracerService
    {
        public List<VariableOccurrenceDto> GetOccurrences(KBObject obj, string variableName)
        {
            var result = new List<VariableOccurrenceDto>();
            if (obj == null || string.IsNullOrEmpty(variableName)) return result;

            string cleanName = variableName.StartsWith("&") ? variableName.Substring(1) : variableName;

            // Analizar cada parte relevante
            AnalyzePart(obj.Parts.Get<RulesPart>(), "Rules", cleanName, result);
            AnalyzePart(obj.Parts.Get<EventsPart>(), "Events", cleanName, result);
            AnalyzePart(obj.Parts.Get<ProcedurePart>(), "Source", cleanName, result);
            AnalyzePart(obj.Parts.Get<ConditionsPart>(), "Conditions", cleanName, result);

            return result;
        }

        private void AnalyzePart(KBObjectPart part, string partName, string varName, List<VariableOccurrenceDto> result)
        {
            if (part == null) return;

            string source = "";
            if (part is ISource s) source = s.Source;
            else if (part is RulesPart r) source = r.Source;
            else if (part is ConditionsPart c) source = c.Source;

            if (string.IsNullOrEmpty(source)) return;

            string[] lines = source.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            // Regex para encontrar la variable (case insensitive)
            string varPattern = "&" + Regex.Escape(varName) + @"\b";
            // Regex para detectar asignación: &var = ... (pero no ==)
            string writePattern = "&" + Regex.Escape(varName) + @"\s*=[^=]";

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (Regex.IsMatch(line, varPattern, RegexOptions.IgnoreCase))
                {
                    // Ignorar líneas comentadas
                    string trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("//")) continue;

                    result.Add(new VariableOccurrenceDto
                    {
                        LineNumber = i + 1,
                        PartName = partName,
                        FullLine = line.Trim(),
                        Context = line.Trim(),
                        Type = Regex.IsMatch(line, writePattern, RegexOptions.IgnoreCase) 
                               ? OccurrenceType.Write 
                               : OccurrenceType.Read
                    });
                }
            }
        }
    }
}