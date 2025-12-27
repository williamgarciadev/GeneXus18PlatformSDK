using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Acme.Packages.Menu.Core.Domain.DTOs;
using Acme.Packages.Menu.Core.Domain.Interfaces;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Parts;

namespace Acme.Packages.Menu.Core.Application.Services
{
    public class DocumentationService : IDocumentationService
    {
        public ObjectDocumentationDto ExtractDocumentation(KBObject obj)
        {
            var dto = new ObjectDocumentationDto
            {
                Name = obj.Name,
                Description = obj.Description,
                Type = obj.TypeDescriptor.Name,
                Module = obj.Module?.Name,
                LastModified = obj.ModifiedAt,
                KBName = obj.Model?.ParentContext?.Name ?? "Unknown"
            };

            ExtractParameters(obj, dto);
            ExtractVariables(obj, dto);

            return dto;
        }

        private void ExtractParameters(KBObject obj, ObjectDocumentationDto dto)
        {
            var rulesPart = obj.Parts.Get<RulesPart>();
            if (rulesPart == null) return;

            // Regex simple para capturar la regla parm
            // parm(in:&var1, out:&var2, inout:&var3);
            var parmRegex = new Regex(@"parm\s*\((.*?)\)\s*;", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = parmRegex.Match(rulesPart.Source);

            if (match.Success)
            {
                var content = match.Groups[1].Value;
                var paramsList = content.Split(',');

                foreach (var p in paramsList)
                {
                    var cleanParam = p.Trim();
                    if (string.IsNullOrEmpty(cleanParam)) continue;

                    var paramDto = new ParameterDocumentationDto();
                    
                    // Detectar acceso
                    if (cleanParam.StartsWith("in:", StringComparison.OrdinalIgnoreCase))
                    {
                        paramDto.Access = "in";
                        paramDto.Name = cleanParam.Substring(3).Trim();
                    }
                    else if (cleanParam.StartsWith("out:", StringComparison.OrdinalIgnoreCase))
                    {
                        paramDto.Access = "out";
                        paramDto.Name = cleanParam.Substring(4).Trim();
                    }
                    else if (cleanParam.StartsWith("inout:", StringComparison.OrdinalIgnoreCase))
                    {
                        paramDto.Access = "inout";
                        paramDto.Name = cleanParam.Substring(6).Trim();
                    }
                    else
                    {
                        paramDto.Access = "inout (default)";
                        paramDto.Name = cleanParam;
                    }

                    // Intentar obtener el tipo de la parte de variables
                    paramDto.Type = GetVariableType(obj, paramDto.Name);
                    
                    dto.Parameters.Add(paramDto);
                }
            }
        }

        private void ExtractVariables(KBObject obj, ObjectDocumentationDto dto)
        {
            var variablesPart = obj.Parts.Get<VariablesPart>();
            if (variablesPart == null) return;

            foreach (var v in variablesPart.Variables)
            {
                // Solo incluimos variables que no sean parámetros para evitar duplicidad relevante
                if (!dto.Parameters.Any(p => p.Name.Equals(v.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    // Solo variables que parezcan "importantes" (opcional: filtrar por uso en el source)
                    dto.Variables.Add(new VariableDocumentationDto
                    {
                        Name = v.Name,
                        Type = v.Type.ToString() + (v.Length > 0 ? "(" + v.Length + (v.Decimals > 0 ? "." + v.Decimals : "") + ")" : ""),
                        Description = v.Description
                    });
                }
            }
        }

        private string GetVariableType(KBObject obj, string varName)
        {
            var variablesPart = obj.Parts.Get<VariablesPart>();
            if (variablesPart == null) return "Unknown";

            var variable = variablesPart.Variables.FirstOrDefault(v => v.Name.Equals(varName, StringComparison.OrdinalIgnoreCase));
            if (variable != null)
            {
                return variable.Type.ToString() + (variable.Length > 0 ? "(" + variable.Length + (variable.Decimals > 0 ? "." + variable.Decimals : "") + ")" : "");
            }

            return "Unknown";
        }
    }
}
