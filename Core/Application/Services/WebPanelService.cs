using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Artech.Architecture.Common.Objects;
using Artech.Architecture.UI.Framework.Services;
using Artech.Genexus.Common.Objects;
using Acme.Packages.Menu.Core.Domain.Interfaces;
using Artech.Genexus.Common.Parts;

namespace Acme.Packages.Menu.Core.Application.Services
{
    public class WebPanelService
    {
        private readonly ILogger _logger;

        public WebPanelService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void ListFormClassPropertyAndExport()
        {
            try
            {
                var model = UIServices.KB.CurrentModel;
                if (model == null)
                {
                    _logger.LogError("No active model found.");
                    return;
                }

                _logger.LogSuccess("Analyzing WebPanels for 'Form Class'...");
                
                var results = new List<WebPanelInfo>();
                int webPanelsFound = 0;

                foreach (KBObject obj in model.Objects.GetAll())
                {
                    if (obj.TypeDescriptor.Name == "WebPanel")
                    {
                        webPanelsFound++;
                        string formClass = GetFormClassValue(obj);
                        
                        results.Add(new WebPanelInfo { Name = obj.Name, Description = obj.Description, FormClass = formClass });
                        
                        if (webPanelsFound % 50 == 0)
                             _logger.LogSuccess(string.Format("Processed {0} WebPanels...", webPanelsFound));
                    }
                }

                _logger.LogSuccess(string.Format("Finished. Total WebPanels: {0}", webPanelsFound));

                if (results.Count > 0)
                    ExportToExcel(results);
                else
                    _logger.LogWarning("No WebPanels found.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: " + ex.Message);
            }
        }

        private string GetFormClassValue(KBObject obj)
        {
            try 
            {
                // 1. Intentar propiedades directas
                string val = GetPropString(obj, "FormClass");
                if (!string.IsNullOrEmpty(val)) return val;

                val = GetPropString(obj, "ThemeClass");
                if (!string.IsNullOrEmpty(val)) return val;

                // 2. Buscar en WebFormPart
                WebFormPart webForm = obj.Parts.Get<WebFormPart>();
                if (webForm != null)
                {
                    val = GetPropString(webForm, "FormClass");
                    if (!string.IsNullOrEmpty(val)) return val;

                    val = GetPropString(webForm, "ThemeClass");
                    if (!string.IsNullOrEmpty(val)) return val;

                    // 3. Fallback: Analizar el XML del Layout directamente
                    // Intentamos obtener el source casteando a ISource
                    string source = null;
                    if (webForm is ISource sourcePart)
                    {
                        source = sourcePart.Source;
                    }

                    if (!string.IsNullOrEmpty(source))
                    {
                        // Expresiones regulares con string verbatim (@) para evitar errores de escape
                        var match = Regex.Match(source, @"Name=""FormClass""\s+Value=""([^""]+)""", RegexOptions.IgnoreCase);
                        if (match.Success) return match.Groups[1].Value;

                        match = Regex.Match(source, @"Name=""Class""\s+Value=""([^""]+)""", RegexOptions.IgnoreCase);
                        if (match.Success) return match.Groups[1].Value;

                        match = Regex.Match(source, @"FormClass=""([^""]+)""", RegexOptions.IgnoreCase);
                        if (match.Success) return match.Groups[1].Value;

                        match = Regex.Match(source, @"Class=""([^""]+)""", RegexOptions.IgnoreCase);
                        if (match.Success) return match.Groups[1].Value;
                    }
                }

                return "";
            }
            catch { return ""; }
        }

        private string GetPropString(object obj, string propName)
        {
            try
            {
                if (obj == null) return null;

                // Si es un KBObject
                if (obj is KBObject kbObj)
                {
                    object val = kbObj.GetPropertyValue(propName);
                    return val != null ? val.ToString() : null;
                }

                // Si es un KBObjectPart
                if (obj is KBObjectPart kbPart)
                {
                    object val = kbPart.GetPropertyValue(propName);
                    return val != null ? val.ToString() : null;
                }

                return null;
            }
            catch { return null; }
        }

        private void ExportToExcel(List<WebPanelInfo> data)
        {
            try
            {
                string tempFile = Path.Combine(Path.GetTempPath(), "WebPanels_Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                var sb = new StringBuilder();
                
                // Header
                sb.AppendLine("WebPanel Name;Description;Form Class");

                foreach (var item in data)
                {
                    sb.AppendLine(string.Format("{0};{1};{2}", 
                        EscapeCsv(item.Name), 
                        EscapeCsv(item.Description), 
                        EscapeCsv(item.FormClass)));
                }

                File.WriteAllText(tempFile, sb.ToString(), new UTF8Encoding(true));
                _logger.LogSuccess("Excel/CSV created: " + tempFile);
                Process.Start(tempFile);
            }
            catch (Exception ex)
            {
                _logger.LogError("Export Error: " + ex.Message);
            }
        }

        private string EscapeCsv(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            if (field.Contains(";") || field.Contains(""") || field.Contains("\r") || field.Contains("\n"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }
            return field;
        }

        private class WebPanelInfo
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string FormClass { get; set; }
        }
    }
}