using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
// using Newtonsoft.Json; // Comentado para evitar problemas de dependencias

// SDK GeneXus
using Artech.Architecture.Common.Objects;
using Artech.Architecture.UI.Framework.Services;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Parts;

namespace Acme.Packages.Menu.Utilities
{
    /// <summary>
    /// Exportador de estructura de tablas de transacciones GeneXus
    /// </summary>
    public class ExtractorTablasGX
    {
        #region Public Methods

        /// <summary>
        /// Exporta la estructura de todas las tablas de transacciones a JSON
        /// </summary>
        public static void ExportarEstructuraTablas()
        {
            try
            {
                Utils.Log("🔄 Iniciando exportación de estructura de tablas");
                
                var exporter = new TableStructureExporter();
                var filePath = exporter.ExportToJson();
                
                Utils.ShowInfo(
                    $"✅ Exportación completada exitosamente:\n\n📁 {filePath}", 
                    "Exportación Exitosa");
                    
                Utils.Log($"✅ Estructura exportada a: {filePath}");
            }
            catch (Exception ex)
            {
                string errorMsg = $"❌ Error al exportar estructura de tablas: {ex.Message}";
                Utils.ShowError(errorMsg);
                Utils.Log(errorMsg);
            }
        }

        #endregion
    }

    #region Core Classes

    /// <summary>
    /// Servicio principal para exportar estructura de tablas
    /// </summary>
    internal class TableStructureExporter
    {
        private readonly TransactionProcessor _transactionProcessor;
        private readonly JsonExportService _jsonExportService;
        private readonly FileExportService _fileExportService;

        public TableStructureExporter()
        {
            _transactionProcessor = new TransactionProcessor();
            _jsonExportService = new JsonExportService();
            _fileExportService = new FileExportService();
        }

        /// <summary>
        /// Exporta la estructura completa a archivo JSON
        /// </summary>
        /// <returns>Ruta del archivo generado</returns>
        public string ExportToJson()
        {
            var model = GetValidatedModel();
            var transactions = GetTransactions(model);
            
            Utils.Log($"📊 Procesando {transactions.Count} transacciones");
            
            var tableStructures = ProcessTransactions(transactions);
            var exportData = CreateExportData(model, tableStructures);
            
            return SaveToFile(exportData);
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

        private List<Transaction> GetTransactions(KBModel model)
        {
            return model.GetObjects<Transaction>().ToList();
        }

        private List<TableStructureData> ProcessTransactions(List<Transaction> transactions)
        {
            var results = new List<TableStructureData>();
            
            foreach (var transaction in transactions)
            {
                try
                {
                    var tableData = _transactionProcessor.ProcessTransaction(transaction);
                    if (tableData != null)
                    {
                        results.Add(tableData);
                        Utils.Log($"✅ Procesada: {transaction.Name}");
                    }
                }
                catch (Exception ex)
                {
                    Utils.Log($"⚠️ Error procesando {transaction.Name}: {ex.Message}");
                }
            }
            
            return results;
        }

        private TableExportData CreateExportData(KBModel model, List<TableStructureData> tableStructures)
        {
            return new TableExportData
            {
                FechaExportacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalTransacciones = tableStructures.Count,
                KnowledgeBase = model.KB.Name,
                Modelo = model.Name,
                Tablas = tableStructures
            };
        }

        private string SaveToFile(TableExportData exportData)
        {
            var json = _jsonExportService.SerializeToJson(exportData);
            return _fileExportService.SaveJsonToFile(json, "estructura_tablas_kb.json");
        }
    }

    /// <summary>
    /// Procesador de transacciones para extraer estructura de tablas
    /// </summary>
    internal class TransactionProcessor
    {
        private readonly AttributeProcessor _attributeProcessor;

        public TransactionProcessor()
        {
            _attributeProcessor = new AttributeProcessor();
        }

        /// <summary>
        /// Procesa una transacción individual y extrae su estructura
        /// </summary>
        /// <param name="transaction">Transacción a procesar</param>
        /// <returns>Estructura de tabla o null si no es válida</returns>
        public TableStructureData ProcessTransaction(Transaction transaction)
        {
            if (transaction?.Structure?.Root == null)
                return null;

            var structure = transaction.Structure.Root;
            var attributes = ProcessAttributes(structure.Attributes);

            if (!attributes.Any())
                return null;

            return new TableStructureData
            {
                Transaccion = transaction.Name,
                Descripcion = transaction.Description ?? "",
                TotalCampos = attributes.Count,
                CamposClave = attributes.Count(a => a.EsClave),
                CamposForaneos = 0, // No disponible en TransactionAttribute
                Campos = attributes
            };
        }

        private List<AttributeData> ProcessAttributes(IList<TransactionAttribute> attributes)
        {
            var results = new List<AttributeData>();

            foreach (var attr in attributes)
            {
                try
                {
                    var attributeData = _attributeProcessor.ProcessAttribute(attr);
                    if (attributeData != null)
                    {
                        results.Add(attributeData);
                    }
                }
                catch (Exception ex)
                {
                    Utils.Log($"⚠️ Error procesando atributo {attr.Name}: {ex.Message}");
                }
            }

            return results;
        }
    }

    /// <summary>
    /// Procesador de atributos individuales
    /// </summary>
    internal class AttributeProcessor
    {
        /// <summary>
        /// Procesa un atributo individual de transacción
        /// </summary>
        /// <param name="transactionAttribute">Atributo de transacción</param>
        /// <returns>Datos del atributo o null si no es válido</returns>
        public AttributeData ProcessAttribute(TransactionAttribute transactionAttribute)
        {
            if (transactionAttribute?.Attribute == null)
                return null;

            var attr = transactionAttribute;
            var attribute = attr.Attribute;

            return new AttributeData
            {
                Nombre = attr.Name,
                Tipo = attribute.Type.ToString(),
                Longitud = attribute.Length,
                Decimales = attribute.Decimals,
                EsClave = attr.IsKey,
                EsForanea = false, // No disponible en TransactionAttribute
                PermiteNulos = attr.IsNullable == TableAttribute.IsNullableValue.True,
                Dominio = attribute.DomainBasedOn?.Name ?? "",
                Descripcion = SanitizeDescription(attribute.Description ?? ""),
                EsImagen = false, // No disponible en TransactionAttribute
                EsReadOnly = false, // No disponible en TransactionAttribute  
                TieneValorPorDefecto = false, // No disponible en TransactionAttribute
                ValorPorDefecto = "" // No disponible en TransactionAttribute
            };
        }

        private string SanitizeDescription(string description)
        {
            return description
                .Replace("\"", "\\\"")
                .Replace("\r\n", " ")
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Trim();
        }
    }

    /// <summary>
    /// Servicio de exportación JSON manual (sin dependencias externas)
    /// </summary>
    internal class JsonExportService
    {
        /// <summary>
        /// Serializa objeto a JSON con formato legible
        /// </summary>
        /// <param name="data">Objeto a serializar</param>
        /// <returns>JSON formateado</returns>
        public string SerializeToJson(object data)
        {
            try
            {
                if (data is TableExportData exportData)
                {
                    return SerializeExportData(exportData);
                }
                throw new ArgumentException("Tipo de datos no soportado para serialización");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al serializar datos a JSON: {ex.Message}", ex);
            }
        }

        private string SerializeExportData(TableExportData data)
        {
            var json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine($"  \"FechaExportacion\": \"{EscapeJsonString(data.FechaExportacion)}\",");
            json.AppendLine($"  \"TotalTransacciones\": {data.TotalTransacciones},");
            json.AppendLine($"  \"KnowledgeBase\": \"{EscapeJsonString(data.KnowledgeBase)}\",");
            json.AppendLine($"  \"Modelo\": \"{EscapeJsonString(data.Modelo)}\",");
            json.AppendLine("  \"Tablas\": [");

            if (data.Tablas != null && data.Tablas.Any())
            {
                for (int i = 0; i < data.Tablas.Count; i++)
                {
                    var tabla = data.Tablas[i];
                    json.AppendLine("    {");
                    json.AppendLine($"      \"Transaccion\": \"{EscapeJsonString(tabla.Transaccion)}\",");
                    json.AppendLine($"      \"Descripcion\": \"{EscapeJsonString(tabla.Descripcion)}\",");
                    json.AppendLine($"      \"TotalCampos\": {tabla.TotalCampos},");
                    json.AppendLine($"      \"CamposClave\": {tabla.CamposClave},");
                    json.AppendLine($"      \"CamposForaneos\": {tabla.CamposForaneos},");
                    json.AppendLine("      \"Campos\": [");

                    if (tabla.Campos != null && tabla.Campos.Any())
                    {
                        for (int j = 0; j < tabla.Campos.Count; j++)
                        {
                            var campo = tabla.Campos[j];
                            json.AppendLine("        {");
                            json.AppendLine($"          \"Nombre\": \"{EscapeJsonString(campo.Nombre)}\",");
                            json.AppendLine($"          \"Tipo\": \"{EscapeJsonString(campo.Tipo)}\",");
                            json.AppendLine($"          \"Longitud\": {campo.Longitud},");
                            json.AppendLine($"          \"Decimales\": {campo.Decimales},");
                            json.AppendLine($"          \"EsClave\": {campo.EsClave.ToString().ToLower()},");
                            json.AppendLine($"          \"EsForanea\": {campo.EsForanea.ToString().ToLower()},");
                            json.AppendLine($"          \"PermiteNulos\": {campo.PermiteNulos.ToString().ToLower()},");
                            json.AppendLine($"          \"Dominio\": \"{EscapeJsonString(campo.Dominio)}\",");
                            json.AppendLine($"          \"Descripcion\": \"{EscapeJsonString(campo.Descripcion)}\",");
                            json.AppendLine($"          \"EsImagen\": {campo.EsImagen.ToString().ToLower()},");
                            json.AppendLine($"          \"EsReadOnly\": {campo.EsReadOnly.ToString().ToLower()},");
                            json.AppendLine($"          \"TieneValorPorDefecto\": {campo.TieneValorPorDefecto.ToString().ToLower()},");
                            json.Append($"          \"ValorPorDefecto\": \"{EscapeJsonString(campo.ValorPorDefecto)}\"");
                            json.AppendLine();
                            json.Append("        }");
                            if (j < tabla.Campos.Count - 1) json.Append(",");
                            json.AppendLine();
                        }
                    }

                    json.AppendLine("      ]");
                    json.Append("    }");
                    if (i < data.Tablas.Count - 1) json.Append(",");
                    json.AppendLine();
                }
            }

            json.AppendLine("  ]");
            json.AppendLine("}");

            return json.ToString();
        }

        private string EscapeJsonString(string input)
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

    /// <summary>
    /// Servicio de manejo de archivos
    /// </summary>
    internal class FileExportService
    {
        /// <summary>
        /// Guarda contenido JSON en archivo
        /// </summary>
        /// <param name="jsonContent">Contenido JSON</param>
        /// <param name="fileName">Nombre del archivo</param>
        /// <returns>Ruta completa del archivo guardado</returns>
        public string SaveJsonToFile(string jsonContent, string fileName)
        {
            try
            {
                var filePath = GenerateFilePath(fileName);
                File.WriteAllText(filePath, jsonContent, Encoding.UTF8);
                return filePath;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al guardar archivo: {ex.Message}", ex);
            }
        }

        private string GenerateFilePath(string fileName)
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            
            var timestampedFileName = $"{nameWithoutExtension}_{timestamp}{extension}";
            return Path.Combine(desktopPath, timestampedFileName);
        }
    }

    #endregion

    #region Data Models

    /// <summary>
    /// Modelo de datos para la exportación completa
    /// </summary>
    internal class TableExportData
    {
        public string FechaExportacion { get; set; }
        public int TotalTransacciones { get; set; }
        public string KnowledgeBase { get; set; }
        public string Modelo { get; set; }
        public List<TableStructureData> Tablas { get; set; }
    }

    /// <summary>
    /// Modelo de datos para estructura de tabla individual
    /// </summary>
    internal class TableStructureData
    {
        public string Transaccion { get; set; }
        public string Descripcion { get; set; }
        public int TotalCampos { get; set; }
        public int CamposClave { get; set; }
        public int CamposForaneos { get; set; } // Reservado para futuras versiones
        public List<AttributeData> Campos { get; set; }
    }

    /// <summary>
    /// Modelo de datos para atributo individual
    /// </summary>
    internal class AttributeData
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public int Longitud { get; set; }
        public int Decimales { get; set; }
        public bool EsClave { get; set; }
        public bool EsForanea { get; set; } // Reservado para futuras versiones
        public bool PermiteNulos { get; set; }
        public string Dominio { get; set; }
        public string Descripcion { get; set; }
        public bool EsImagen { get; set; } // Reservado para futuras versiones
        public bool EsReadOnly { get; set; } // Reservado para futuras versiones
        public bool TieneValorPorDefecto { get; set; } // Reservado para futuras versiones
        public string ValorPorDefecto { get; set; } // Reservado para futuras versiones
    }

    #endregion
}