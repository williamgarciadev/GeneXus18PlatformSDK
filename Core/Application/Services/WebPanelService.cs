using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

                _logger.LogSuccess("Analyzing 'Form Class' property for all WebPanels...");
                
                var results = new List<WebPanelInfo>();
                int count = 0;

                foreach (var obj in model.Objects.GetAll())
                {
                    if (obj is WebPanel webPanel)
                    {
                        string formClass = GetFormClassValue(webPanel);
                        results.Add(new WebPanelInfo { Name = webPanel.Name, Description = webPanel.Description, FormClass = formClass });
                        
                        // Optional: Log to output window as well (maybe just summary or first few)
                        // _logger.LogSuccess(string.Format("{0,-30} | {1}", webPanel.Name, formClass));
                        count++;
                    }
                }

                _logger.LogSuccess(string.Format("Total WebPanels processed: {0}", count));

                if (results.Count > 0)
                {
                    ExportToExcel(results);
                }
                else
                {
                    _logger.LogWarning("No WebPanels found to export.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error listing WebPanel properties: " + ex.Message);
            }
        }

        private string GetFormClassValue(WebPanel webPanel)
        {
            object propValue = null;

            // Attempt 1: Direct property on WebPanel
            propValue = webPanel.GetPropertyValue("FormClass");

            // Attempt 2: If null, try "ThemeClass" (common internal name)
            if (propValue == null)
            {
                propValue = webPanel.GetPropertyValue("ThemeClass");
            }

            // Attempt 3: Try on the WebForm part
            if (propValue == null)
            {
                var formPart = webPanel.Parts.Get<WebFormPart>();
                if (formPart != null)
                {
                    propValue = formPart.GetPropertyValue("FormClass");
                    if (propValue == null)
                        propValue = formPart.GetPropertyValue("ThemeClass");
                    if (propValue == null)
                        propValue = formPart.GetPropertyValue("Class");
                }
            }

            return propValue != null ? propValue.ToString() : "";
        }

        private void ExportToExcel(List<WebPanelInfo> data)
        {
            try
            {
                string tempFile = Path.Combine(Path.GetTempPath(), "WebPanels_FormClass_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                
                var sb = new StringBuilder();
                // Add BOM for Excel UTF-8 compatibility
                sb.Append(new string(new char[] { '\uFEFF' })); 
                
                // Header - Using semicolon for better Excel compatibility in many regions
                sb.AppendLine("WebPanel Name;Description;Form Class");

                foreach (var item in data)
                {
                    sb.AppendLine(string.Format("{0};{1};{2}", 
                        EscapeCsv(item.Name), 
                        EscapeCsv(item.Description), 
                        EscapeCsv(item.FormClass)));
                }

                File.WriteAllText(tempFile, sb.ToString(), Encoding.UTF8);
                _logger.LogSuccess("Export created successfully: " + tempFile);

                Process.Start(tempFile);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error exporting to Excel/CSV: " + ex.Message);
            }
        }

        private string EscapeCsv(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            if (field.Contains(";") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
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