using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Xml;
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
                if (model == null) { _logger.LogError("No active model."); return; }

                _logger.LogSuccess("Analyzing WebPanels for 'Form Class'...");
                
                var results = new List<WebPanelInfo>();
                int count = 0;
                bool debugDone = false;

                foreach (KBObject obj in model.Objects.GetAll())
                {
                    if (obj.TypeDescriptor.Name == "WebPanel")
                    {
                        count++;
                        string formClass = GetFormClassValue(obj);
                        
                        if (string.IsNullOrEmpty(formClass) && !debugDone)
                        {
                            formClass = GetFormClassViaFullXml(obj);
                            debugDone = true;
                        }
                        
                        if (string.IsNullOrEmpty(formClass))
                        {
                             formClass = GetFormClassViaFullXml(obj);
                        }

                        results.Add(new WebPanelInfo { Name = obj.Name, Description = obj.Description, FormClass = formClass });
                        
                        if (count % 50 == 0) _logger.LogSuccess(string.Format("Processed {0}...", count));
                    }
                }
                
                _logger.LogSuccess(string.Format("Finished. Total: {0}", count));

                if (results.Count > 0)
                    ExportToExcel(results);
                else
                    _logger.LogWarning("No WebPanels found.");
            }
            catch (Exception ex) { _logger.LogError("Error: " + ex.Message); }
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
                }
                
                return "";
            }
            catch { return ""; }
        }

        private string GetFormClassViaFullXml(KBObject obj)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                using (XmlTextWriter writer = new XmlTextWriter(sw))
                {
                    MethodInfo serializeMethod = obj.GetType().GetMethod("Serialize", new Type[] { typeof(XmlWriter) });
                    if (serializeMethod != null)
                    {
                        serializeMethod.Invoke(obj, new object[] { writer });
                    }
                    else
                    {
                        return ""; 
                    }
                }
                
                string xmlContent = sb.ToString();
                if (string.IsNullOrEmpty(xmlContent)) return "";

                // Using character class instead of backslash-s to avoid compiler escape issues
                var regexes = new string[] { 
                    "Name=\"FormClass\"[ \t\r\n]+Value=\"([^\"]+)\"",
                    "Name=\"Class\"[ \t\r\n]+Value=\"([^\"]+)\"",
                    "FormClass=\"([^\"]+)\"",
                    "Class=\"([^\"]+)\"",
                    "<Name>FormClass</Name>[ \t\r\n]*<Value>([^<]+)</Value>",
                    "<Name>Class</Name>[ \t\r\n]*<Value>([^<]+)</Value>"
                };

                foreach(var pat in regexes)
                {
                    var m = Regex.Match(xmlContent, pat, RegexOptions.IgnoreCase);
                    if (m.Success) return m.Groups[1].Value;
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
                sb.Append(new char[] { '\uFEFF' });
                sb.AppendLine("WebPanel Name;Description;Form Class");

                foreach (var item in data)
                {
                    sb.AppendLine(string.Format("{0};{1};{2}", 
                        EscapeCsv(item.Name), 
                        EscapeCsv(item.Description), 
                        EscapeCsv(item.FormClass)));
                }

                File.WriteAllText(tempFile, sb.ToString(), Encoding.UTF8);
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

        private class WebPanelInfo { public string Name { get; set; } public string Description { get; set; } public string FormClass { get; set; } }
    }
}
