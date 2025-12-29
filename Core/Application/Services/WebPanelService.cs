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
using Artech.Common.Properties;

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
                        
                        // --- DEBUG DIAGNOSTIC START ---
                        if (webPanelsFound == 1)
                        {
                            LogDeepDebugInfo(obj);
                        }
                        // --- DEBUG DIAGNOSTIC END ---

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

        private void LogDeepDebugInfo(KBObject obj)
        {
            try
            {
                _logger.LogSuccess("**************************************************");
                _logger.LogSuccess(string.Format("DIAGNOSTIC REPORT FOR: {0}", obj.Name));
                _logger.LogSuccess("**************************************************");

                _logger.LogSuccess("--- OBJECT PROPERTIES ---");
                foreach (Property prop in obj.Properties)
                {
                    if (prop.Name.ToLower().Contains("class"))
                        _logger.LogSuccess(string.Format("PROP: {0} = {1}", prop.Name, prop.Value));
                }

                WebFormPart webForm = obj.Parts.Get<WebFormPart>();
                if (webForm != null)
                {
                    _logger.LogSuccess("--- WEBFORM PART PROPERTIES ---");
                    foreach (Property prop in webForm.Properties)
                    {
                        if (prop.Name.ToLower().Contains("class"))
                            _logger.LogSuccess(string.Format("PART PROP: {0} = {1}", prop.Name, prop.Value));
                    }

                    if (webForm is ISource sourcePart)
                    {
                        string src = sourcePart.Source;
                        if (!string.IsNullOrEmpty(src))
                        {
                            _logger.LogSuccess("--- SOURCE PREVIEW (First 500 chars) ---");
                            _logger.LogSuccess(src.Substring(0, Math.Min(src.Length, 500)));
                            
                            _logger.LogSuccess("--- REGEX TEST ---");
                            
                            // USING STANDARD STRINGS TO AVOID COMPILER CONFUSION
                            var regexes = new string[] { 
                                "Name=\"FormClass\"\s+Value=\"([^\"]+)\"",
                                "Name=\"Class\"\s+Value=\"([^\"]+)\"",
                                "FormClass=\"([^\"]+)\"",
                                "Class=\"([^\"]+)\""
                            };

                            foreach(var pat in regexes)
                            {
                                var m = Regex.Match(src, pat, RegexOptions.IgnoreCase);
                                if (m.Success)
                                    _logger.LogSuccess(string.Format("MATCH FOUND for '{0}': {1}", pat, m.Groups[1].Value));
                                else
                                    _logger.LogWarning(string.Format("NO MATCH for '{0}'", pat));
                            }
                        }
                        else
                        {
                             _logger.LogWarning("WebForm Source is empty.");
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("WebFormPart NOT FOUND on this object.");
                }
                _logger.LogSuccess("**************************************************");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Diagnostic: " + ex.Message);
            }
        }

        private string GetFormClassValue(KBObject obj)
        {
            try 
            {
                string val = GetPropString(obj, "FormClass");
                if (!string.IsNullOrEmpty(val)) return val;

                val = GetPropString(obj, "ThemeClass");
                if (!string.IsNullOrEmpty(val)) return val;

                WebFormPart webForm = obj.Parts.Get<WebFormPart>();
                if (webForm != null)
                {
                    val = GetPropString(webForm, "FormClass");
                    if (!string.IsNullOrEmpty(val)) return val;

                    val = GetPropString(webForm, "ThemeClass");
                    if (!string.IsNullOrEmpty(val)) return val;

                    string source = null;
                    if (webForm is ISource sourcePart)
                    {
                        source = sourcePart.Source;
                    }

                    if (!string.IsNullOrEmpty(source))
                    {
                        // USING STANDARD STRINGS TO AVOID COMPILER CONFUSION
                        var match = Regex.Match(source, "Name=\"FormClass\"\s+Value=\"([^\"]+)\"", RegexOptions.IgnoreCase);
                        if (match.Success) return match.Groups[1].Value;

                        match = Regex.Match(source, "Name=\"Class\"\s+Value=\"([^\"]+)\"", RegexOptions.IgnoreCase);
                        if (match.Success) return match.Groups[1].Value;

                        match = Regex.Match(source, "FormClass=\"([^\"]+)\"", RegexOptions.IgnoreCase);
                        if (match.Success) return match.Groups[1].Value;

                        match = Regex.Match(source, "Class=\"([^\"]+)\"", RegexOptions.IgnoreCase);
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

                if (obj is KBObject kbObj)
                {
                    object val = kbObj.GetPropertyValue(propName);
                    return val != null ? val.ToString() : null;
                }

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
            if (field.Contains(";") || field.Contains("\"") || field.Contains("\r") || field.Contains("\n"))
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
