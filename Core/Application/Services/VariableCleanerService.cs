using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Acme.Packages.Menu.Core.Domain.Interfaces;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Parts;
using Artech.Genexus.Common.Parts.Layout;

namespace Acme.Packages.Menu.Core.Application.Services
{
    public class VariableCleanerService : IVariableCleanerService
    {
        public int CleanUnusedVariables(KBObject obj)
        {
            if (obj == null) return 0;

            VariablesPart variablesPart = obj.Parts.Get<VariablesPart>();
            if (variablesPart == null) return 0;

            string allCode = GetAllObjectCode(obj);
            string cleanCode = RemoveComments(allCode);
            
            int removedCount = 0;
            List<Variable> toRemove = new List<Variable>();

            // Lista blanca de variables que NUNCA debemos borrar
            string[] protectedVariables = { "Pgmname", "Pgmdesc", "Time", "Today", "Mode", "Msg", "Output" };

            Acme.Packages.Menu.Utilities.Utils.Log($"🧹 Iniciando limpieza de variables en '{obj.Name}'...");

            foreach (Variable variable in variablesPart.Variables)
            {
                // 1. Verificar si está en la lista protegida
                if (protectedVariables.Any(p => p.Equals(variable.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    Acme.Packages.Menu.Utilities.Utils.Log($"   - '{variable.Name}': Omitida (Protegida por el sistema)");
                    continue;
                }

                // 2. Verificar uso en código activo (sin comentarios)
                if (!IsVariableUsed(variable.Name, cleanCode))
                {
                    toRemove.Add(variable);
                    Acme.Packages.Menu.Utilities.Utils.Log($"   - '{variable.Name}': Marcada para eliminar (No se encontró uso)");
                }
                else
                {
                    Acme.Packages.Menu.Utilities.Utils.Log($"   + '{variable.Name}': En uso");
                }
            }

            foreach (var varToRemove in toRemove)
            {
                try 
                {
                    variablesPart.Remove(varToRemove);
                    removedCount++;
                }
                catch (Exception ex)
                {
                    Acme.Packages.Menu.Utilities.Utils.Log($"   ❌ Error al eliminar '{varToRemove.Name}': {ex.Message}");
                }
            }

            if (removedCount > 0)
            {
                obj.Save();
                Acme.Packages.Menu.Utilities.Utils.Log($"✅ Limpieza finalizada. Se eliminaron {removedCount} variables.");
            }
            else
            {
                Acme.Packages.Menu.Utilities.Utils.Log("✨ No se encontraron variables innecesarias.");
            }

            return removedCount;
        }

        private string RemoveComments(string code)
        {
            if (string.IsNullOrEmpty(code)) return "";
            // Bloques /* */
            string noBlockComments = Regex.Replace(code, @"/\*.*?\*/", "", RegexOptions.Singleline);
            // Líneas //
            return Regex.Replace(noBlockComments, @"//.*", "");
        }

        private bool IsVariableUsed(string name, string cleanCode)
        {
            // Usamos regex para buscar la variable con el prefijo &
            // Nos aseguramos de que sea una palabra completa para evitar que &Var coincida con &Variable
            string pattern = @"&" + Regex.Escape(name) + @"\b";
            return Regex.IsMatch(cleanCode, pattern, RegexOptions.IgnoreCase);
        }

        private string GetAllObjectCode(KBObject obj)
        {
            var sb = new System.Text.StringBuilder();

            // 1. Rules
            var rules = obj.Parts.Get<RulesPart>();
            if (rules != null) sb.AppendLine(rules.Source);

            // 2. Events (WebPanels, Transactions, etc.)
            var events = obj.Parts.Get<EventsPart>();
            if (events != null) sb.AppendLine(events.Source);

            // 3. Source (Procedures)
            var procedure = obj.Parts.Get<ProcedurePart>();
            if (procedure != null) sb.AppendLine(procedure.Source);

            // 4. Conditions
            var conditions = obj.Parts.Get<ConditionsPart>();
            if (conditions != null) sb.AppendLine(conditions.Source);

            // 5. Layout (WebPanels / Panels)
            var layout = obj.Parts.Get<LayoutPart>();
            if (layout != null)
            {
                // El LayoutPart suele tener una representación XML o texto
                sb.AppendLine(layout.ToString());
            }

            return sb.ToString();
        }
    }
}