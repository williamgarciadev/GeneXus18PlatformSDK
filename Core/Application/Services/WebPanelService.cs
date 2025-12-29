using System;
using System.Linq;
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

        public void ListFormClassProperty()
        {
            try
            {
                var model = UIServices.KB.CurrentModel;
                if (model == null)
                {
                    _logger.LogError("No active model found.");
                    return;
                }

                _logger.LogSuccess("Listing 'Form Class' property for all WebPanels...");
                _logger.LogSuccess("--------------------------------------------------");

                int count = 0;
                foreach (var obj in model.Objects.GetAll())
                {
                    if (obj is WebPanel webPanel)
                    {
                        string formClass = "N/A";
                        
                        // Attempt 1: Direct property on WebPanel
                        object propValue = webPanel.GetPropertyValue("FormClass");

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

                        if (propValue != null)
                        {
                            formClass = propValue.ToString();
                        }
                        
                        _logger.LogSuccess(string.Format("{0,-30} | {1}", webPanel.Name, formClass));
                        count++;
                    }
                }

                _logger.LogSuccess("--------------------------------------------------");
                _logger.LogSuccess(string.Format("Total WebPanels processed: {0}", count));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error listing WebPanel properties: " + ex.Message);
            }
        }
    }
}
