using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Acme.Packages.Menu.Core.Domain.Interfaces;
using Acme.Packages.Menu.Models;
using Acme.Packages.Menu.Utilities;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common;
using Artech.Genexus.Common.Parts;

namespace Acme.Packages.Menu.Core.Application.Services
{
    public class SmartVariableService : ISmartVariableService
    {
        private readonly string[] _protectedVariables = { "Pgmname", "Pgmdesc", "Time", "Today", "Mode", "Msg", "Output" };

        public List<VariableInfo> GetUndefinedVariables(KBObject obj)
        {
            if (obj == null) return new List<VariableInfo>();

            var definedVars = GetDefinedVariableNames(obj);
            string allCode = GetAllObjectCode(obj);
            
            // 1. Extraer menciones simples
            var mentionedVars = ExtractVariableMentions(allCode);

            // 2. Extraer asignaciones directas (ej: &var = 123)
            var assignments = ExtractAssignments(allCode);

            var undefinedNames = mentionedVars
                .Where(v => !definedVars.Contains(v.ToLower()))
                .Where(v => !_protectedVariables.Any(p => p.Equals(v, StringComparison.OrdinalIgnoreCase)))
                .Distinct()
                .ToList();

            // 3. Resolver tipos: Prioridad Asignación > KB > Sufijos
            return undefinedNames.Select(name => {
                // Si hay una asignación literal para esta variable, la usamos
                if (assignments.ContainsKey(name.ToLower()))
                {
                    var info = InferFromLiteral(name, assignments[name.ToLower()]);
                    if (info != null) return info;
                }
                return InferVariableInfo(name);
            }).ToList();
        }

        private Dictionary<string, string> ExtractAssignments(string code)
        {
            var assignments = new Dictionary<string, string>();
            // Regex para: &variable = valor (números, strings entre comillas, booleanos)
            var regex = new Regex(@"&([a-zA-Z_]\w*)\s*=\s*('[^']*'|""[^""]*""|\d+\.?\d*|true|false)", RegexOptions.IgnoreCase);
            var matches = regex.Matches(code);

            foreach (Match m in matches)
            {
                string name = m.Groups[1].Value.ToLower();
                string val = m.Groups[2].Value;
                if (!assignments.ContainsKey(name)) assignments[name] = val;
            }
            return assignments;
        }

        private VariableInfo InferFromLiteral(string name, string literal)
        {
            // A. Booleanos
            if (literal.Equals("true", StringComparison.OrdinalIgnoreCase) || 
                literal.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                return new VariableInfo { CleanName = name, Type = eDBType.Boolean };
            }

            // B. Strings
            if (literal.StartsWith("'") || literal.StartsWith("\""))
            {
                string content = literal.Trim('\'', '\"');
                int len = Math.Max(content.Length, 1);
                return new VariableInfo { CleanName = name, Type = eDBType.VARCHAR, Length = len };
            }

            // C. Números
            if (Regex.IsMatch(literal, @"^\d+\.?\d*$"))
            {
                string[] parts = literal.Split('.');
                int decimals = parts.Length > 1 ? parts[1].Length : 0;
                int length = parts[0].Length + decimals;
                return new VariableInfo { CleanName = name, Type = eDBType.NUMERIC, Length = length, Decimals = decimals };
            }

            return null;
        }

        public void DefineVariables(KBObject obj, List<VariableInfo> variables)
        {
            if (obj == null || variables == null || !variables.Any()) return;

            // Obtenemos la parte de variables, que es un KBObjectPart válido
            KBObjectPart part = obj.Parts.Get<VariablesPart>();
            if (part == null) return;
            
            foreach (var v in variables)
            {
                if (v.IsBasedOnAttributeOrDomain)
                {
                    VariableHelper.AddVariableBasedOn(v.CleanName, part, v.BaseReference, null);
                }
                else
                {
                    // Pasamos también los decimales inferidos
                    VariableHelper.AddVariable(v.CleanName, part, v.Type, v.Length, v.Decimals);
                }
            }
        }

        private HashSet<string> GetDefinedVariableNames(KBObject obj)
        {
            var variablesPart = obj.Parts.Get<VariablesPart>();
            if (variablesPart == null) return new HashSet<string>();

            // Solo consideramos como "definidas" aquellas que NO son autodefinidas por GX
            // y que NO son de sistema (Standard). 
            // Esto permite que el Smart Fix proponga "promover" las variables autodefinidas.
            return new HashSet<string>(
                variablesPart.Variables
                    .Where(v => !v.IsStandard)
                    .Where(v => !v.IsAutoDefined)
                    .Select(v => v.Name.ToLower())
            );
        }

        private bool IsImplicit(Variable v)
        {
            // Según el código decompilado, la propiedad correcta es IsAutoDefined
            return v.IsAutoDefined;
        }

        private List<string> ExtractVariableMentions(string code)
        {
            // Regex para capturar &variable (no capturamos el &)
            var regex = new Regex(@"&([a-zA-Z_][a-zA-Z0-9_]*)", RegexOptions.IgnoreCase);
            var matches = regex.Matches(code);
            
            var list = new List<string>();
            foreach (Match m in matches)
            {
                list.Add(m.Groups[1].Value);
            }
            return list;
        }

        private VariableInfo InferVariableInfo(string name)
        {
            // A. Primero intentar Búsqueda Semántica en la KB
            var (kbType, kbLength, isKb) = VariableHelper.GetTypeAndLengthFromReference(name);
            if (isKb)
            {
                return new VariableInfo {
                    CleanName = name,
                    Type = kbType,
                    Length = kbLength,
                    BaseReference = name,
                    IsBasedOnAttributeOrDomain = true
                };
            }

            // B. Si no, inferir por sufijos
            if (name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) || 
                name.EndsWith("Cod", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith("Code", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith("Num", StringComparison.OrdinalIgnoreCase))
            {
                return new VariableInfo { CleanName = name, Type = eDBType.NUMERIC, Length = 10 };
            }

            if (name.EndsWith("Name", StringComparison.OrdinalIgnoreCase) || 
                name.EndsWith("Dsc", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith("Desc", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith("Txt", StringComparison.OrdinalIgnoreCase))
            {
                return new VariableInfo { CleanName = name, Type = eDBType.VARCHAR, Length = 100 };
            }

            if (name.StartsWith("Is", StringComparison.OrdinalIgnoreCase) || 
                name.StartsWith("Has", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("Can", StringComparison.OrdinalIgnoreCase))
            {
                return new VariableInfo { CleanName = name, Type = eDBType.Boolean, Length = 1 };
            }

            if (name.EndsWith("Date", StringComparison.OrdinalIgnoreCase))
            {
                return new VariableInfo { CleanName = name, Type = eDBType.DATE };
            }

            if (name.EndsWith("Time", StringComparison.OrdinalIgnoreCase))
            {
                return new VariableInfo { CleanName = name, Type = eDBType.DATETIME };
            }

            // Default: VarChar(40)
            return new VariableInfo { CleanName = name, Type = eDBType.VARCHAR, Length = 40 };
        }

        private string GetAllObjectCode(KBObject obj)
        {
            // Reutilizamos la lógica del Cleaner, pero eliminando comentarios
            var sb = new System.Text.StringBuilder();
            var rules = obj.Parts.Get<RulesPart>(); if (rules != null) sb.AppendLine(rules.Source);
            var events = obj.Parts.Get<EventsPart>(); if (events != null) sb.AppendLine(events.Source);
            var procedure = obj.Parts.Get<ProcedurePart>(); if (procedure != null) sb.AppendLine(procedure.Source);
            var conditions = obj.Parts.Get<ConditionsPart>(); if (conditions != null) sb.AppendLine(conditions.Source);
            
            // Eliminar comentarios para no detectar variables que solo están comentadas
            string raw = sb.ToString();
            string noBlock = Regex.Replace(raw, @"/\*.*?\*/", "", RegexOptions.Singleline);
            return Regex.Replace(noBlock, @"//.*", "");
        }
    }
}
