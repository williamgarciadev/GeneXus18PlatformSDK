using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
// using Newtonsoft.Json; // Comentado para evitar problemas de dependencias

// SDK GeneXus
using Artech.Architecture.Common.Objects;
using Artech.Architecture.UI.Framework.Services;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Parts;

namespace Acme.Packages.Menu.Utilities
{
    public class ProcedureSourceExtractor
    {
        public static void ExportarSourceCodeProcedimientos()
        {
            try
            {
                var model = UIServices.KB.CurrentModel;
                if (model == null)
                {
                    Utils.ShowError("❌ No se pudo acceder al modelo actual de la KB.");
                    return;
                }

                var procedimientos = model.GetObjects<Procedure>().ToList();
                var proceduresData = new List<ProcedureData>();

                Utils.Log(string.Format("🔍 Encontrados {0} procedimientos para exportar", procedimientos.Count));

                foreach (var proc in procedimientos)
                {
                    try
                    {
                        var procedurePart = proc.Parts.Get<ProcedurePart>();
                        var rulesPart = proc.Parts.Get<RulesPart>();
                        var variablesPart = proc.Parts.Get<VariablesPart>();

                        // Extraer información básica del procedimiento
                        var procedureInfo = new ProcedureData
                        {
                            Nombre = proc.Name,
                            Descripcion = proc.Description ?? "",
                            GUID = proc.Guid.ToString(),
                            FechaCreacion = GetProcedureDate(proc),
                            Modulo = proc.Module?.Name ?? "",
                            
                            // Código fuente principal
                            SourceCode = procedurePart?.Source ?? "",
                            
                            // Reglas si existen
                            Rules = rulesPart?.Source ?? "",
                            
                            // Variables definidas
                            Variables = GetVariablesInfo(variablesPart),
                            
                            // Propiedades adicionales
                            Propiedades = GetProcedureProperties(proc),
                            
                            // Métricas de código
                            Metricas = GetCodeMetrics(procedurePart?.Source ?? "")
                        };

                        proceduresData.Add(procedureInfo);
                        Utils.Log(string.Format("✅ Procesado: {0}", proc.Name));
                    }
                    catch (Exception ex)
                    {
                        Utils.Log(string.Format("⚠️ Error procesando {0}: {1}", proc.Name, ex.Message));
                        
                        // Agregar información básica aunque haya error
                        proceduresData.Add(new ProcedureData
                        {
                            Nombre = proc.Name,
                            Descripcion = proc.Description ?? "",
                            GUID = proc.Guid.ToString(),
                            FechaCreacion = "",
                            Modulo = "",
                            SourceCode = "",
                            Rules = "",
                            Variables = new List<object>(),
                            Propiedades = new Dictionary<string, object> { {"Error", ex.Message} },
                            Metricas = new Dictionary<string, int>()
                        });
                    }
                }

                // Crear el objeto JSON final
                var exportData = new ProcedureExportData
                {
                    FechaExportacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    TotalProcedimientos = procedimientos.Count,
                    KnowledgeBase = UIServices.KB.CurrentModel.KB.Name,
                    Modelo = UIServices.KB.CurrentModel.Name,
                    Procedimientos = proceduresData
                };

                // Serializar a JSON con formato legible (implementación manual)
                string json = SerializeToJsonManual(exportData);

                // Guardar archivo
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
                    string.Format("ProcedimientosSource_{0:yyyyMMdd_HHmmss}.json", DateTime.Now));

                File.WriteAllText(filePath, json, Encoding.UTF8);

                Utils.ShowInfo(string.Format("✅ Source code de procedimientos exportado exitosamente:\n\n" +
                              "📁 Archivo: {0}\n" +
                              "📊 Total procedimientos: {1}\n" +
                              "📝 Formato: JSON estructurado", filePath, procedimientos.Count), 
                              "Exportación Completa");
                              
                Utils.Log(string.Format("📄 Archivo generado: {0}", filePath));
            }
            catch (Exception ex)
            {
                Utils.ShowError(string.Format("❌ Error general al exportar source code de procedimientos: {0}", ex.Message));
                Utils.Log(string.Format("❌ Error: {0}", ex.Message));
            }
        }

        private static List<object> GetVariablesInfo(VariablesPart variablesPart)
        {
            var variables = new List<object>();
            
            if (variablesPart != null)
            {
                foreach (var variable in variablesPart.Variables)
                {
                    try
                    {
                        variables.Add(new
                        {
                            Nombre = variable.Name,
                            Tipo = variable.Type.ToString(),
                            Longitud = variable.Length,
                            Decimales = variable.Decimals,
                            Descripcion = variable.Description ?? "",
                            BasadoEn = variable.AttributeBasedOn?.Name ?? variable.DomainBasedOn?.Name ?? "",
                            EsStandard = variable.IsStandard,
                            Firmado = variable.Signed
                        });
                    }
                    catch (Exception ex)
                    {
                        Utils.Log(string.Format("⚠️ Error procesando variable {0}: {1}", variable.Name, ex.Message));
                    }
                }
            }
            
            return variables;
        }

        private static Dictionary<string, object> GetProcedureProperties(Procedure proc)
        {
            var properties = new Dictionary<string, object>();
            
            try
            {
                // Propiedades comunes de procedimientos
                properties["EsMain"] = Utility.IsMain(proc);
                properties["EsGenerado"] = Utility.isGenerated(proc);
                properties["PuedeEjecutarse"] = Utility.isRunable(proc);
                properties["PuedeConstruirse"] = Utility.CanBeBuilt(proc);
                
                // Intentar obtener otras propiedades específicas
                try
                {
                    var callProtocol = proc.GetPropertyValue("CALL_PROTOCOL");
                    if (callProtocol != null)
                        properties["ProtocoloLlamada"] = callProtocol.ToString();
                }
                catch { }
                
                try
                {
                    var isWebService = proc.GetPropertyValue("IS_WEBSERVICE");
                    if (isWebService != null)
                        properties["EsWebService"] = isWebService.ToString();
                }
                catch { }
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("⚠️ Error obteniendo propiedades de {0}: {1}", proc.Name, ex.Message));
            }
            
            return properties;
        }

        private static Dictionary<string, int> GetCodeMetrics(string sourceCode)
        {
            var metrics = new Dictionary<string, int>();
            
            if (string.IsNullOrEmpty(sourceCode))
            {
                return new Dictionary<string, int>
                {
                    ["LineasCodigo"] = 0,
                    ["NivelComplejidad"] = 0,
                    ["MaxBloquesCodigo"] = 0,
                    ["MaxNivelAnidamiento"] = 0
                };
            }
            
            try
            {
                metrics["LineasCodigo"] = Utility.LineCount(sourceCode);
                metrics["NivelComplejidad"] = Utility.ComplexityLevel(sourceCode);
                metrics["MaxBloquesCodigo"] = Utility.MaxCodeBlock(sourceCode);
                metrics["MaxNivelAnidamiento"] = Utility.MaxNestLevel(sourceCode);
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("⚠️ Error calculando métricas de código: {0}", ex.Message));
                metrics["Error"] = 1;
            }
            
            return metrics;
        }

        private static string GetProcedureDate(Procedure proc)
        {
            try
            {
                // Intentar diferentes propiedades de fecha que podrían existir
                var dateProperties = new[] { "LastUpdate", "Created", "Modified", "Timestamp" };
                
                foreach (var propName in dateProperties)
                {
                    try
                    {
                        var dateValue = proc.GetPropertyValue(propName);
                        if (dateValue != null && DateTime.TryParse(dateValue.ToString(), out DateTime parsedDate))
                        {
                            return parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                    catch
                    {
                        // Continuar con la siguiente propiedad
                    }
                }
                
                // Si no se encuentra ninguna fecha válida, usar fecha actual
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("⚠️ Error obteniendo fecha de {0}: {1}", proc.Name, ex.Message));
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// Serialización manual de datos a JSON (sin dependencias externas)
        /// </summary>
        private static string SerializeToJsonManual(ProcedureExportData exportData)
        {
            var json = new StringBuilder();
            
            json.AppendLine("{");
            json.AppendLine(string.Format("  \"FechaExportacion\": \"{0}\",", exportData.FechaExportacion));
            json.AppendLine(string.Format("  \"TotalProcedimientos\": {0},", exportData.TotalProcedimientos));
            json.AppendLine(string.Format("  \"KnowledgeBase\": \"{0}\",", EscapeJsonString(exportData.KnowledgeBase)));
            json.AppendLine(string.Format("  \"Modelo\": \"{0}\",", EscapeJsonString(exportData.Modelo)));
            json.AppendLine("  \"Procedimientos\": [");

            var procedimientos = exportData.Procedimientos;
            if (procedimientos != null)
            {
                for (int i = 0; i < procedimientos.Count; i++)
                {
                    var proc = procedimientos[i];
                    json.AppendLine("    {");
                    json.AppendLine(string.Format("      \"Nombre\": \"{0}\",", EscapeJsonString(proc.Nombre ?? "")));
                    json.AppendLine(string.Format("      \"Descripcion\": \"{0}\",", EscapeJsonString(proc.Descripcion ?? "")));
                    json.AppendLine(string.Format("      \"GUID\": \"{0}\",", proc.GUID ?? ""));
                    json.AppendLine(string.Format("      \"FechaCreacion\": \"{0}\",", proc.FechaCreacion ?? ""));
                    json.AppendLine(string.Format("      \"Modulo\": \"{0}\",", EscapeJsonString(proc.Modulo ?? "")));
                    json.AppendLine(string.Format("      \"SourceCode\": \"{0}\",", EscapeJsonString(proc.SourceCode ?? "")));
                    json.AppendLine(string.Format("      \"Rules\": \"{0}\",", EscapeJsonString(proc.Rules ?? "")));
                    json.AppendLine("      \"Variables\": [],");
                    json.AppendLine("      \"Propiedades\": {},");
                    json.AppendLine("      \"Metricas\": {}");
                    json.Append("    }");
                    if (i < procedimientos.Count - 1) json.Append(",");
                    json.AppendLine();
                }
            }

            json.AppendLine("  ]");
            json.AppendLine("}");

            return json.ToString();
        }

        /// <summary>
        /// Escapa caracteres especiales para JSON
        /// </summary>
        private static string EscapeJsonString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r\n", "\\n")
                .Replace("\n", "\\n")
                .Replace("\r", "\\n")
                .Replace("\t", "\\t");
        }
    }

    public class ProcedureExportData
    {
        public string FechaExportacion { get; set; }
        public int TotalProcedimientos { get; set; }
        public string KnowledgeBase { get; set; }
        public string Modelo { get; set; }
        public List<ProcedureData> Procedimientos { get; set; }
    }

    public class ProcedureData
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string GUID { get; set; }
        public string FechaCreacion { get; set; }
        public string Modulo { get; set; }
        public string SourceCode { get; set; }
        public string Rules { get; set; }
        public List<object> Variables { get; set; }
        public Dictionary<string, object> Propiedades { get; set; }
        public Dictionary<string, int> Metricas { get; set; }
    }
}