using System;
using System.Collections.Generic;
using System.Linq;
using Artech.Architecture.UI.Framework.Services;
using Artech.Architecture.Common.Objects;
using Artech.Genexus.Common.Objects;
using Acme.Packages.Menu.Infrastructure;
using Acme.Packages.Menu.Services.Analysis;
using Acme.Packages.Menu.Utilities;

namespace Acme.Packages.Menu.Services.Export
{
    /// <summary>
    /// Contador de líneas de código para Procedures y WebPanels
    /// </summary>
    internal class CodeLineCounter : BaseKBExporter
    {
        public void ExportCodeLinesToCSV()
        {
            var model = UIServices.KB.CurrentModel;
            var kb = model.KB;

            ValidateKBAccess(model, kb);

            var csvData = GenerateCodeLinesData(model);
            var filePath = SaveToDesktop(csvData, "ConteoLineasCodigo_{timestamp}.csv");

            ShowSuccessMessage(filePath, "Conteo de líneas");
        }

        private List<string> GenerateCodeLinesData(KBModel model)
        {
            var lines = new List<string>
            {
                "Objeto,Tipo,TotalLineas,LineasSinComentarios,LineasOperativas"
            };

            var codeAnalyzer = new CodeAnalyzer();
            
            // Procesar Procedures
            ProcessObjectsOfType<Procedure>(model, lines, "Procedure", codeAnalyzer);

            // Procesar WebPanels
            ProcessObjectsOfType<WebPanel>(model, lines, "WebPanel", codeAnalyzer);

            return lines;
        }

        private void ProcessObjectsOfType<T>(KBModel model, List<string> lines, string typeName, CodeAnalyzer analyzer) where T : KBObject
        {
            var objects = model.GetObjects<T>().ToList();

            foreach (var obj in objects)
            {
                try
                {
                    var codeStats = analyzer.AnalyzeObjectCode(obj);
                    var csvLine = $"{obj.Name},{typeName},{codeStats.TotalLines},{codeStats.NonCommentLines},{codeStats.OperativeLines}";
                    lines.Add(csvLine);
                }
                catch (Exception ex)
                {
                    Utils.Log($"⚠️ Error procesando {typeName} '{obj.Name}': {ex.Message}");
                    lines.Add($"{obj.Name},{typeName},0,0,0");
                }
            }
        }
    }
}