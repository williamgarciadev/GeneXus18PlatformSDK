using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

// SDK GeneXus
using Artech.Architecture.Common.Objects;
using Artech.Architecture.UI.Framework.Services;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Parts;

namespace Acme.Packages.Menu.Utilities
{
    /// <summary>
    /// Exportador de objetos con líneas operativas que genera CSV y archivos individuales de código fuente
    /// </summary>
    public class ObjectSourceLineExporter
    {
        #region Public Methods

        /// <summary>
        /// Exporta objetos con líneas operativas a CSV y genera archivos individuales de código fuente
        /// </summary>
        public static void ExportarObjetosConLineasOperativas()
        {
            try
            {
                Utils.Log("🔄 Iniciando exportación de objetos con líneas operativas");

                var exporter = new ObjectSourceProcessor();
                var (csvPath, sourceFolder) = exporter.ExportObjectsWithSourceLines();

                Utils.ShowInfo(
                    $"✅ Exportación completada exitosamente:\n\n" +
                    $"📄 CSV: {csvPath}\n" +
                    $"📁 Código fuente: {sourceFolder}",
                    "Exportación Exitosa");

                Utils.Log($"✅ CSV exportado a: {csvPath}");
                Utils.Log($"✅ Código fuente exportado a: {sourceFolder}");
            }
            catch (Exception ex)
            {
                string errorMsg = $"❌ Error al exportar objetos con líneas operativas: {ex.Message}";
                Utils.ShowError(errorMsg);
                Utils.Log(errorMsg);
            }
        }

        #endregion
    }

    #region Core Classes

    /// <summary>
    /// Procesador principal para objetos con líneas de código
    /// </summary>
    internal class ObjectSourceProcessor
    {
        private readonly ObjectAnalyzer _objectAnalyzer;
        private readonly CsvExportService _csvExportService;
        private readonly SourceFileExportService _sourceFileExportService;

        public ObjectSourceProcessor()
        {
            _objectAnalyzer = new ObjectAnalyzer();
            _csvExportService = new CsvExportService();
            _sourceFileExportService = new SourceFileExportService();
        }

        /// <summary>
        /// Exporta objetos con líneas operativas a CSV y archivos individuales
        /// </summary>
        /// <returns>Tupla con ruta del CSV y carpeta de archivos fuente</returns>
        public (string csvPath, string sourceFolder) ExportObjectsWithSourceLines()
        {
            var model = GetValidatedModel();
            var objectsWithLines = ProcessAllObjects(model);

            Utils.Log($"📊 Encontrados {objectsWithLines.Count} objetos con líneas operativas");

            // Crear archivos de exportación
            var csvPath = _csvExportService.ExportToCsv(objectsWithLines, model);
            var sourceFolder = _sourceFileExportService.ExportSourceFiles(objectsWithLines, model);

            return (csvPath, sourceFolder);
        }

        private KBModel GetValidatedModel()
        {
            var model = UIServices.KB.CurrentModel;
            if (model == null)
            {
                throw new InvalidOperationException("No se pudo acceder al modelo actual de la KB.");
            }
            return model;
        }

        private List<ObjectWithSourceData> ProcessAllObjects(KBModel model)
        {
            var objectsWithLines = new List<ObjectWithSourceData>();

            // Procesar diferentes tipos de objetos
            ProcessObjectType<Procedure>(model, objectsWithLines, "Procedure");
            ProcessObjectType<Transaction>(model, objectsWithLines, "Transaction");
            ProcessObjectType<WebPanel>(model, objectsWithLines, "WebPanel");
            ProcessObjectType<WorkPanel>(model, objectsWithLines, "WorkPanel");
            ProcessObjectType<DataProvider>(model, objectsWithLines, "DataProvider");

            return objectsWithLines;
        }

        private void ProcessObjectType<T>(KBModel model, List<ObjectWithSourceData> results, string objectType) where T : KBObject
        {
            try
            {
                var objects = model.GetObjects<T>().ToList();
                Utils.Log($"🔍 Procesando {objects.Count} objetos de tipo {objectType}");

                foreach (var obj in objects)
                {
                    try
                    {
                        var objectData = _objectAnalyzer.AnalyzeObject(obj, objectType);
                        if (objectData != null && objectData.LineasOperativas > 0)
                        {
                            results.Add(objectData);
                            Utils.Log($"✅ {objectType}: {obj.Name} ({objectData.LineasOperativas} líneas)");
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.Log($"⚠️ Error procesando {objectType} {obj.Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log($"⚠️ Error procesando tipo {objectType}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Analizador de objetos individuales para extraer líneas operativas
    /// </summary>
    internal class ObjectAnalyzer
    {
        /// <summary>
        /// Analiza un objeto individual y extrae información de líneas operativas
        /// </summary>
        public ObjectWithSourceData AnalyzeObject(KBObject obj, string objectType)
        {
            if (obj == null) return null;

            var sourceCode = ExtractSourceCode(obj);
            var rulesCode = ExtractRulesCode(obj);
            var combinedCode = CombineSourceAndRules(sourceCode, rulesCode);

            // Contar líneas operativas (no comentarios ni líneas vacías)
            var operationalLines = CountOperationalLines(combinedCode);

            if (operationalLines == 0) return null;

            return new ObjectWithSourceData
            {
                Nombre = obj.Name,
                TipoObjeto = objectType,
                Descripcion = obj.Description ?? "",
                GUID = obj.Guid.ToString(),
                Modulo = obj.Module?.Name ?? "",
                LineasOperativas = operationalLines,
                LineasTotales = CountTotalLines(combinedCode),
                CodigoFuente = sourceCode,
                Reglas = rulesCode,
                CodigoCombinado = combinedCode,
                FechaAnalisis = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Propiedades = ExtractObjectProperties(obj),
                Variables = ExtractVariables(obj)
            };
        }

        private string ExtractSourceCode(KBObject obj)
        {
            try
            {
                // Intentar obtener diferentes tipos de source parts
                var procedurePart = obj.Parts.Get<ProcedurePart>();
                if (procedurePart != null)
                    return procedurePart.Source ?? "";

                var eventsPart = obj.Parts.Get<EventsPart>();
                if (eventsPart != null)
                    return eventsPart.Source ?? "";

                var conditionsPart = obj.Parts.Get<ConditionsPart>();
                if (conditionsPart != null)
                    return conditionsPart.Source ?? "";

                return "";
            }
            catch (Exception ex)
            {
                Utils.Log($"⚠️ Error extrayendo código fuente de {obj.Name}: {ex.Message}");
                return "";
            }
        }

        private string ExtractRulesCode(KBObject obj)
        {
            try
            {
                var rulesPart = obj.Parts.Get<RulesPart>();
                return rulesPart?.Source ?? "";
            }
            catch (Exception ex)
            {
                Utils.Log($"⚠️ Error extrayendo reglas de {obj.Name}: {ex.Message}");
                return "";
            }
        }

        private string CombineSourceAndRules(string sourceCode, string rulesCode)
        {
            var combined = new StringBuilder();

            if (!string.IsNullOrEmpty(sourceCode))
            {
                combined.AppendLine("// === CÓDIGO FUENTE ===");
                combined.AppendLine(sourceCode);
            }

            if (!string.IsNullOrEmpty(rulesCode))
            {
                if (combined.Length > 0)
                    combined.AppendLine();
                combined.AppendLine("// === REGLAS ===");
                combined.AppendLine(rulesCode);
            }

            return combined.ToString();
        }

        private int CountOperationalLines(string code)
        {
            if (string.IsNullOrEmpty(code)) return 0;

            var lines = code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int operationalCount = 0;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Saltar líneas vacías
                if (string.IsNullOrEmpty(trimmedLine))
                    continue;

                // Saltar comentarios completos
                if (trimmedLine.StartsWith("//") || trimmedLine.StartsWith("/*") || trimmedLine.StartsWith("*"))
                    continue;

                // Saltar separadores de sección
                if (trimmedLine.StartsWith("// ==="))
                    continue;

                // Es una línea operativa
                operationalCount++;
            }

            return operationalCount;
        }

        private int CountTotalLines(string code)
        {
            if (string.IsNullOrEmpty(code)) return 0;
            return code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private Dictionary<string, object> ExtractObjectProperties(KBObject obj)
        {
            var properties = new Dictionary<string, object>();

            try
            {
                // Propiedades comunes según el tipo de objeto
                if (obj is Procedure proc)
                {
                    properties["EsMain"] = Utility.IsMain(proc);
                    properties["EsGenerado"] = Utility.isGenerated(proc);
                    properties["PuedeEjecutarse"] = Utility.isRunable(proc);
                }

                properties["TieneDescripcion"] = !string.IsNullOrEmpty(obj.Description);
                properties["TieneModulo"] = obj.Module != null;
            }
            catch (Exception ex)
            {
                Utils.Log($"⚠️ Error extrayendo propiedades de {obj.Name}: {ex.Message}");
            }

            return properties;
        }

        private List<ObjectVariableInfo> ExtractVariables(KBObject obj)
        {
            var variables = new List<ObjectVariableInfo>();

            try
            {
                var variablesPart = obj.Parts.Get<VariablesPart>();
                if (variablesPart != null)
                {
                    foreach (var variable in variablesPart.Variables)
                    {
                        variables.Add(new ObjectVariableInfo
                        {
                            Nombre = variable.Name,
                            Tipo = variable.Type.ToString(),
                            Longitud = variable.Length,
                            Descripcion = variable.Description ?? ""
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log($"⚠️ Error extrayendo variables de {obj.Name}: {ex.Message}");
            }

            return variables;
        }
    }

    /// <summary>
    /// Servicio para exportar datos a archivo CSV
    /// </summary>
    internal class CsvExportService
    {
        /// <summary>
        /// Exporta los objetos con líneas operativas a archivo CSV
        /// </summary>
        public string ExportToCsv(List<ObjectWithSourceData> objects, KBModel model)
        {
            var csvContent = new StringBuilder();

            // Header del CSV
            csvContent.AppendLine("Nombre,TipoObjeto,Modulo,LineasOperativas,LineasTotales,Descripcion,GUID,FechaAnalisis");

            // Datos de cada objeto
            foreach (var obj in objects.OrderByDescending(o => o.LineasOperativas))
            {
                csvContent.AppendLine($"\"{EscapeCsvField(obj.Nombre)}\"," +
                                    $"\"{EscapeCsvField(obj.TipoObjeto)}\"," +
                                    $"\"{EscapeCsvField(obj.Modulo)}\"," +
                                    $"{obj.LineasOperativas}," +
                                    $"{obj.LineasTotales}," +
                                    $"\"{EscapeCsvField(obj.Descripcion)}\"," +
                                    $"\"{obj.GUID}\"," +
                                    $"\"{obj.FechaAnalisis}\"");
            }

            // Guardar archivo CSV
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"ObjetosLineasOperativas_{model.KB.Name}_{timestamp}.csv";
            var filePath = Path.Combine(desktopPath, fileName);

            File.WriteAllText(filePath, csvContent.ToString(), Encoding.UTF8);

            return filePath;
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            // Escapar comillas dobles duplicándolas
            return field.Replace("\"", "\"\"");
        }
    }

    /// <summary>
    /// Servicio para exportar archivos individuales de código fuente
    /// </summary>
    internal class SourceFileExportService
    {
        /// <summary>
        /// Exporta archivos individuales con el código fuente de cada objeto
        /// </summary>
        public string ExportSourceFiles(List<ObjectWithSourceData> objects, KBModel model)
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var folderName = $"CodigoFuente_{model.KB.Name}_{timestamp}";
            var folderPath = Path.Combine(desktopPath, folderName);

            // Crear carpeta principal
            Directory.CreateDirectory(folderPath);

            // Crear subcarpetas por tipo de objeto
            var createdFolders = new HashSet<string>();

            foreach (var obj in objects)
            {
                try
                {
                    // Crear subcarpeta por tipo si no existe
                    var typeFolderPath = Path.Combine(folderPath, obj.TipoObjeto);
                    if (!createdFolders.Contains(obj.TipoObjeto))
                    {
                        Directory.CreateDirectory(typeFolderPath);
                        createdFolders.Add(obj.TipoObjeto);
                    }

                    // Nombre de archivo seguro
                    var safeFileName = GetSafeFileName(obj.Nombre);
                    var fileName = $"{safeFileName}_({obj.LineasOperativas}lineas).txt";
                    var filePath = Path.Combine(typeFolderPath, fileName);

                    // Contenido del archivo
                    var fileContent = CreateFileContent(obj);

                    File.WriteAllText(filePath, fileContent, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    Utils.Log($"⚠️ Error exportando archivo para {obj.Nombre}: {ex.Message}");
                }
            }

            // Crear archivo resumen
            CreateSummaryFile(folderPath, objects, model);

            return folderPath;
        }

        private string GetSafeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeName = new StringBuilder();

            foreach (char c in fileName)
            {
                if (invalidChars.Contains(c))
                    safeName.Append('_');
                else
                    safeName.Append(c);
            }

            return safeName.ToString();
        }

        private string CreateFileContent(ObjectWithSourceData obj)
        {
            var content = new StringBuilder();

            // Header del archivo
            content.AppendLine("// ========================================");
            content.AppendLine($"// OBJETO: {obj.Nombre}");
            content.AppendLine($"// TIPO: {obj.TipoObjeto}");
            content.AppendLine($"// MÓDULO: {obj.Modulo}");
            content.AppendLine($"// LÍNEAS OPERATIVAS: {obj.LineasOperativas}");
            content.AppendLine($"// LÍNEAS TOTALES: {obj.LineasTotales}");
            content.AppendLine($"// DESCRIPCIÓN: {obj.Descripcion}");
            content.AppendLine($"// GUID: {obj.GUID}");
            content.AppendLine($"// FECHA ANÁLISIS: {obj.FechaAnalisis}");
            content.AppendLine("// ========================================");
            content.AppendLine();

            // Variables si existen
            if (obj.Variables != null && obj.Variables.Any())
            {
                content.AppendLine("// === VARIABLES ===");
                foreach (var variable in obj.Variables)
                {
                    content.AppendLine($"// {variable.Nombre} ({variable.Tipo}, {variable.Longitud}) - {variable.Descripcion}");
                }
                content.AppendLine();
            }

            // Código fuente
            if (!string.IsNullOrEmpty(obj.CodigoCombinado))
            {
                content.AppendLine(obj.CodigoCombinado);
            }

            return content.ToString();
        }

        private void CreateSummaryFile(string folderPath, List<ObjectWithSourceData> objects, KBModel model)
        {
            try
            {
                var summaryPath = Path.Combine(folderPath, "RESUMEN.txt");
                var summary = new StringBuilder();

                summary.AppendLine("========================================");
                summary.AppendLine("RESUMEN DE EXPORTACIÓN");
                summary.AppendLine("========================================");
                summary.AppendLine();
                summary.AppendLine($"Knowledge Base: {model.KB.Name}");
                summary.AppendLine($"Modelo: {model.Name}");
                summary.AppendLine($"Fecha exportación: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                summary.AppendLine($"Total objetos exportados: {objects.Count}");
                summary.AppendLine();

                // Resumen por tipo
                var byType = objects.GroupBy(o => o.TipoObjeto).OrderBy(g => g.Key);
                summary.AppendLine("OBJETOS POR TIPO:");
                foreach (var group in byType)
                {
                    var totalLines = group.Sum(o => o.LineasOperativas);
                    summary.AppendLine($"  {group.Key}: {group.Count()} objetos ({totalLines:N0} líneas operativas)");
                }
                summary.AppendLine();

                // Top 10 objetos con más líneas
                summary.AppendLine("TOP 10 OBJETOS CON MÁS LÍNEAS OPERATIVAS:");
                var top10 = objects.OrderByDescending(o => o.LineasOperativas).Take(10);
                foreach (var obj in top10)
                {
                    summary.AppendLine($"  {obj.Nombre} ({obj.TipoObjeto}): {obj.LineasOperativas:N0} líneas");
                }

                File.WriteAllText(summaryPath, summary.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Utils.Log($"⚠️ Error creando archivo resumen: {ex.Message}");
            }
        }
    }

    #endregion

    #region Data Models

    /// <summary>
    /// Información de objeto con líneas operativas
    /// </summary>
    internal class ObjectWithSourceData
    {
        public string Nombre { get; set; }
        public string TipoObjeto { get; set; }
        public string Descripcion { get; set; }
        public string GUID { get; set; }
        public string Modulo { get; set; }
        public int LineasOperativas { get; set; }
        public int LineasTotales { get; set; }
        public string CodigoFuente { get; set; }
        public string Reglas { get; set; }
        public string CodigoCombinado { get; set; }
        public string FechaAnalisis { get; set; }
        public Dictionary<string, object> Propiedades { get; set; }
        public List<ObjectVariableInfo> Variables { get; set; }
    }

    /// <summary>
    /// Información de variable para exportación
    /// </summary>
    internal class ObjectVariableInfo
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public int Longitud { get; set; }
        public string Descripcion { get; set; }
    }

    #endregion
}