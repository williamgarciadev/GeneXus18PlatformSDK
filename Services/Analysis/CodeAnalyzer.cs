using System;
using System.Text;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Parts;
using Acme.Packages.Menu.Infrastructure;
using Acme.Packages.Menu.Models;
using Acme.Packages.Menu.Utilities;

namespace Acme.Packages.Menu.Services.Analysis
{
    /// <summary>
    /// Analizador de código fuente para objetos GeneXus
    /// </summary>
    internal class CodeAnalyzer
    {
        public CodeStats AnalyzeObjectCode(KBObject obj)
        {
            var stats = new CodeStats();

            var sourceCode = GetObjectSourceCode(obj);
            if (!string.IsNullOrEmpty(sourceCode))
            {
                var mainStats = CountLinesInCode(sourceCode);
                stats.AddStats(mainStats);
            }

            if (obj is WebPanel)
            {
                var webPanel = (WebPanel)obj;
                var eventStats = AnalyzeWebPanelEvents(webPanel);
                stats.AddStats(eventStats);
            }

            return stats;
        }

        private string GetObjectSourceCode(KBObject obj)
        {
            try
            {
                if (obj is Procedure)
                {
                    var procedure = (Procedure)obj;
                    return GetProcedureSource(procedure);
                }
                else if (obj is WebPanel)
                {
                    var webPanel = (WebPanel)obj;
                    return GetWebPanelSource(webPanel);
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Utils.Log($"⚠️ Error obteniendo código fuente de '{obj.Name}': {ex.Message}");
                return string.Empty;
            }
        }

        private string GetProcedureSource(Procedure procedure)
        {
            var sourcePart = procedure.Parts.Get<ProcedurePart>();
            return sourcePart?.Source?.ToString() ?? string.Empty;
        }

        private string GetWebPanelSource(WebPanel webPanel)
        {
            var eventsPart = webPanel.Parts.Get<EventsPart>();
            if (eventsPart == null) return string.Empty;

            var source = new StringBuilder();
            var events = ReflectionHelper.GetEnumerableProperty(eventsPart, "Events");
            
            if (events != null)
            {
                foreach (var eventItem in events)
                {
                    var eventSource = ReflectionHelper.GetPropertyValue<string>(eventItem, "Source");
                    if (!string.IsNullOrEmpty(eventSource))
                    {
                        source.AppendLine(eventSource);
                    }
                }
            }

            return source.ToString();
        }

        private CodeStats AnalyzeWebPanelEvents(WebPanel webPanel)
        {
            var eventStats = new CodeStats();

            try
            {
                var eventsPart = webPanel.Parts.Get<EventsPart>();
                var events = ReflectionHelper.GetEnumerableProperty(eventsPart, "Events");
                Utils.Log($"🔍 Analizando {events?.ToString()} eventos en WebPanel '{webPanel.Name}'");
                if (events != null)
                {
                    foreach (var eventObj in events)
                    {
                        var eventCode = ReflectionHelper.GetPropertyValue<string>(eventObj, "Source");
                        if (!string.IsNullOrEmpty(eventCode))
                        {
                            var stats = CountLinesInCode(eventCode);
                            eventStats.AddStats(stats);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log($"⚠️ Error analizando eventos de WebPanel '{webPanel.Name}': {ex.Message}");
            }

            return eventStats;
        }

        private CodeStats CountLinesInCode(string sourceCode)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
                return new CodeStats();

            var lines = sourceCode.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            var stats = new CodeStats();
            var lineAnalyzer = new LineAnalyzer();

            foreach (var line in lines)
            {
                stats.TotalLines++;
                var trimmedLine = line.Trim();

                if (lineAnalyzer.IsCommentLine(trimmedLine))
                    continue;

                stats.NonCommentLines++;

                if (lineAnalyzer.IsOperativeLine(trimmedLine))
                    stats.OperativeLines++;
            }

            return stats;
        }
    }
}