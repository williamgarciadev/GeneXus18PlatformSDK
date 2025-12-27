using System;
using System.Text;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Parts;
using Artech.Architecture.Common.Parts;
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
            var source = new StringBuilder();
            
            try
            {
                Utils.Log($"🔍 Iniciando análisis de WebPanel '{webPanel.Name}'");
                
                // Método 1: Intentar obtener código usando SourcePart directamente (similar a Procedures)
                var sourcePart = webPanel.Parts.Get<SourcePart>();
                if (sourcePart != null)
                {
                    Utils.Log($"🔍 Encontrado SourcePart en WebPanel '{webPanel.Name}'");
                    var directSource = sourcePart.Source?.ToString();
                    if (!string.IsNullOrEmpty(directSource))
                    {
                        source.AppendLine(directSource);
                        Utils.Log($"✅ SourcePart encontró {directSource.Split('\n').Length} líneas");
                    }
                }
                
                // Método 2: EventsPart - Eventos del WebPanel
                var eventsPart = webPanel.Parts.Get<EventsPart>();
                if (eventsPart != null)
                {
                    Utils.Log($"🔍 Analizando EventsPart de WebPanel '{webPanel.Name}'");
                    var eventsSource = GetEventsPartSource(eventsPart);
                    if (!string.IsNullOrEmpty(eventsSource))
                    {
                        source.AppendLine(eventsSource);
                        Utils.Log($"✅ EventsPart encontró {eventsSource.Split('\n').Length} líneas");
                    }
                    else
                    {
                        Utils.Log($"⚠️ EventsPart existe pero sin código en '{webPanel.Name}'");
                    }
                }
                else
                {
                    Utils.Log($"⚠️ No se encontró EventsPart en WebPanel '{webPanel.Name}'");
                }
                
                // Método 3: WebForm Part - Eventos de controles
                var webFormPart = webPanel.Parts.Get<WebFormPart>();
                if (webFormPart != null)
                {
                    Utils.Log($"🔍 Analizando WebFormPart de WebPanel '{webPanel.Name}'");
                    var webFormSource = GetWebFormPartSource(webFormPart);
                    if (!string.IsNullOrEmpty(webFormSource))
                    {
                        source.AppendLine(webFormSource);
                        Utils.Log($"✅ WebFormPart encontró {webFormSource.Split('\n').Length} líneas");
                    }
                }
                
                // Método 4: Rules Part - Reglas del WebPanel
                var rulesPart = webPanel.Parts.Get<RulesPart>();
                if (rulesPart != null)
                {
                    Utils.Log($"🔍 Analizando RulesPart de WebPanel '{webPanel.Name}'");
                    var rulesSource = GetRulesPartSource(rulesPart);
                    if (!string.IsNullOrEmpty(rulesSource))
                    {
                        source.AppendLine(rulesSource);
                        Utils.Log($"✅ RulesPart encontró {rulesSource.Split('\n').Length} líneas");
                    }
                }
                
                // Método 5: Listar todas las partes disponibles para debug
                Utils.Log($"🔍 Partes disponibles en WebPanel '{webPanel.Name}':");
                foreach (var part in webPanel.Parts)
                {
                    Utils.Log($"   - {part.GetType().Name}");
                }
                
                Utils.Log($"📊 WebPanel '{webPanel.Name}' total: {source.Length} caracteres de código");
            }
            catch (Exception ex)
            {
                Utils.Log($"❌ Error procesando WebPanel '{webPanel.Name}': {ex.Message}");
            }

            return source.ToString();
        }
        
        private string GetEventsPartSource(EventsPart eventsPart)
        {
            var source = new StringBuilder();
            
            try
            {
                Utils.Log($"🔍 Analizando EventsPart - tipo: {eventsPart.GetType().Name}");
                
                // Método 1: Propiedad Events
                var events = ReflectionHelper.GetEnumerableProperty(eventsPart, "Events");
                if (events != null)
                {
                    Utils.Log($"✅ Encontrada propiedad Events");
                    int eventCount = 0;
                    foreach (var eventItem in events)
                    {
                        eventCount++;
                        Utils.Log($"🔍 Procesando evento #{eventCount} - tipo: {eventItem?.GetType().Name}");
                        
                        // Intentar múltiples propiedades para obtener el código
                        var eventSource = ReflectionHelper.GetPropertyValue<string>(eventItem, "Source");
                        if (string.IsNullOrEmpty(eventSource))
                        {
                            eventSource = ReflectionHelper.GetPropertyValue<string>(eventItem, "Code");
                        }
                        if (string.IsNullOrEmpty(eventSource))
                        {
                            eventSource = ReflectionHelper.GetPropertyValue<string>(eventItem, "Text");
                        }
                        
                        if (!string.IsNullOrEmpty(eventSource))
                        {
                            source.AppendLine(eventSource);
                            Utils.Log($"✅ Evento #{eventCount} encontró {eventSource.Split('\n').Length} líneas");
                        }
                        else
                        {
                            Utils.Log($"⚠️ Evento #{eventCount} sin código fuente encontrado");
                            
                            // Debug: listar todas las propiedades del evento
                            if (eventItem != null)
                            {
                                var eventProps = eventItem.GetType().GetProperties();
                                Utils.Log($"🔍 Propiedades disponibles en evento #{eventCount}:");
                                foreach (var prop in eventProps)
                                {
                                    try
                                    {
                                        var propValue = prop.GetValue(eventItem);
                                        Utils.Log($"   - {prop.Name}: {propValue?.GetType().Name} = '{propValue?.ToString()?.Substring(0, Math.Min(50, propValue?.ToString()?.Length ?? 0))}'");
                                    }
                                    catch (Exception ex)
                                    {
                                        Utils.Log($"   - {prop.Name}: Error al obtener valor - {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                    Utils.Log($"📊 Total eventos procesados: {eventCount}");
                }
                else
                {
                    Utils.Log($"❌ No se encontró propiedad Events en EventsPart");
                }
                
                // Método 2: Intentar acceso directo al Source del EventsPart
                var directSource = ReflectionHelper.GetPropertyValue<string>(eventsPart, "Source");
                if (!string.IsNullOrEmpty(directSource))
                {
                    source.AppendLine(directSource);
                    Utils.Log($"✅ EventsPart.Source encontró {directSource.Split('\n').Length} líneas");
                }
                
                // Método 3: Debug - listar todas las propiedades del EventsPart
                Utils.Log($"🔍 Propiedades disponibles en EventsPart:");
                var eventsPartProps = eventsPart.GetType().GetProperties();
                foreach (var prop in eventsPartProps)
                {
                    try
                    {
                        var propValue = prop.GetValue(eventsPart);
                        Utils.Log($"   - {prop.Name}: {propValue?.GetType().Name}");
                    }
                    catch (Exception ex)
                    {
                        Utils.Log($"   - {prop.Name}: Error - {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log($"❌ Error en GetEventsPartSource: {ex.Message}");
            }
            
            return source.ToString();
        }
        
        private string GetWebFormPartSource(WebFormPart webFormPart)
        {
            var source = new StringBuilder();
            
            try
            {
                // Intentar obtener eventos de controles usando reflexión
                var form = ReflectionHelper.GetPropertyValue<object>(webFormPart, "Form");
                if (form != null)
                {
                    var controls = ReflectionHelper.GetEnumerableProperty(form, "Controls");
                    if (controls != null)
                    {
                        foreach (var control in controls)
                        {
                            var controlEvents = ReflectionHelper.GetEnumerableProperty(control, "Events");
                            if (controlEvents != null)
                            {
                                foreach (var eventItem in controlEvents)
                                {
                                    var eventSource = ReflectionHelper.GetPropertyValue<string>(eventItem, "Source");
                                    if (!string.IsNullOrEmpty(eventSource))
                                    {
                                        source.AppendLine(eventSource);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log($"⚠️ Error procesando WebFormPart: {ex.Message}");
            }
            
            return source.ToString();
        }
        
        private string GetRulesPartSource(RulesPart rulesPart)
        {
            try
            {
                var rulesSource = ReflectionHelper.GetPropertyValue<string>(rulesPart, "Source");
                return rulesSource ?? string.Empty;
            }
            catch (Exception ex)
            {
                Utils.Log($"⚠️ Error procesando RulesPart: {ex.Message}");
                return string.Empty;
            }
        }

        private CodeStats AnalyzeWebPanelEvents(WebPanel webPanel)
        {
            var eventStats = new CodeStats();

            try
            {
                var eventsPart = webPanel.Parts.Get<EventsPart>();
                var events = ReflectionHelper.GetEnumerableProperty(eventsPart, "Events");
                
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