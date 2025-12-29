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
                    _logger.LogError("No active model found. Open a Knowledge Base first.");
                    return;
                }

                _logger.LogSuccess("Starting analysis of 'Form Class' property for all WebPanels...");
                
                var results = new List<WebPanelInfo>();
                int count = 0;
                int webPanelsFound = 0;

                // Iterate using a more generic approach to ensure we catch all WebPanels
                foreach (KBObject obj in model.Objects.GetAll())
                {
                    // Check by TypeDescriptor Name to be safer against assembly version mismatches
                    if (obj.TypeDescriptor.Name == "WebPanel")
                    {
                        webPanelsFound++;
                        string formClass = GetFormClassValue(obj);
                        
                        // Always add to result, even if empty, to show we found the object
                        results.Add(new WebPanelInfo { Name = obj.Name, Description = obj.Description, FormClass = formClass });
                        
                        // Log every 50 items to show progress without flooding
                        if (webPanelsFound % 50 == 0)
                             _logger.LogSuccess(string.Format("Processed {0} WebPanels...", webPanelsFound));
                    }
                }

                _logger.LogSuccess(string.Format("Finished processing. Total WebPanels found: {0}", webPanelsFound));

                if (results.Count > 0)
                {
                    ExportToExcel(results);
                }
                else
                {
                    _logger.LogWarning("No WebPanels found in the current Knowledge Base.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error listing WebPanel properties: " + ex.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        private string GetFormClassValue(KBObject webPanel)
        {
            try 
            {
                object propValue = null;

                // Attempt 1: Direct property on the Object
                propValue = webPanel.GetPropertyValue("FormClass");

                // Attempt 2: "ThemeClass" property
                if (propValue == null)
                {
                    propValue = webPanel.GetPropertyValue("ThemeClass");
                }

                // Attempt 3: Try to find the WebFormPart
                if (propValue == null)
                {
                    // Search for the part using its Guid or Type if generic Get<T> fails
                    KBObjectPart formPart = webPanel.Parts.Get("WebForm");
                    
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
            catch
            {
                return "ErrorReadingProperty";
            }
        }

        private void ExportToExcel(List<WebPanelInfo> data)
        {
            try
            {
                string tempFile = Path.Combine(Path.GetTempPath(), "WebPanels_FormClass_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                
                using (StreamWriter sw = new StreamWriter(tempFile, false, Encoding.UTF8))
                {
                    // Add BOM manually if needed, though Encoding.UTF8 usually handles it if configured, 
                    // but writing the bytes explicitly ensures Excel recognizes it.
                    // StreamWriter with Encoding.UTF8 might imply BOM, but let's be explicit with the preamble if we weren't using the writer's constructor handling.
                    // Actually, let's just use the constructor that includes BOM:
                    // new UTF8Encoding(true) -> true enables BOM
                    
                }
                
                // Let's stick to WriteAllText with specific encoding which is cleaner
                var sb = new StringBuilder();
                sb.AppendLine("WebPanel Name;Description;Form Class");

                foreach (var item in data)
                {
                    sb.AppendLine(string.Format("{0};{1};{2}", 
                        EscapeCsv(item.Name), 
                        EscapeCsv(item.Description), 
                        EscapeCsv(item.FormClass)));
                }

                // UTF8 with BOM
                File.WriteAllText(tempFile, sb.ToString(), new UTF8Encoding(true));
                
                _logger.LogSuccess("Export created successfully at: " + tempFile);
                _logger.LogSuccess("Opening file...");

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
