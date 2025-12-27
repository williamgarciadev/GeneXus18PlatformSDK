using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Acme.Packages.Menu.Models;

// GeneXus SDK
using Artech.Architecture.Common;
using Artech.Architecture.Common.Objects;
using Artech.Architecture.Common.Services;
using Artech.Common.Diagnostics;
using Artech.Common.Exceptions;
using Artech.Common.Framework;
using Artech.Common.Language.Parser.Resolvers;
using Artech.Core;
using Artech.Genexus.Common.Objects;
using Artech.Udm.Framework;

namespace Acme.Packages.Menu.Services
{
    public sealed class GxCodeExtractor : IGxCodeExtractor
    {
        //private static bool _blStarted;

        //public Task<IReadOnlyList<GxObjectDto>> ExtractAsync(
        //    string kbPath,
        //    bool includeProcedures,
        //    bool includeWebPanels,
        //    string nameContains,
        //    CancellationToken ct)
        //{
        //    return Task.Run<IReadOnlyList<GxObjectDto>>(() =>
        //    {
        //        StartBL();

        //        var list = new List<GxObjectDto>();
        //        using (var kb = OpenKB(kbPath))
        //        {
        //            var model = kb.DesignModel;

        //            if (includeProcedures)
        //            {
        //                foreach (var proc in Procedure.GetAll(model).OrderBy(p => p.Name))
        //                {
        //                    ct.ThrowIfCancellationRequested();
        //                    if (!Pass(proc.Name, nameContains)) continue;

        //                    var src = (proc.ProcedurePart != null && proc.ProcedurePart.Source != null)
        //                        ? proc.ProcedurePart.Source
        //                        : string.Empty;

        //                    list.Add(new GxObjectDto
        //                    {
        //                        Type = GxObjectType.Procedure,
        //                        Name = proc.Name,
        //                        Part = "Source",
        //                        Content = src
        //                    });
        //                }
        //            }

        //            if (includeWebPanels)
        //            {
        //                foreach (var wp in WebPanel.GetAll(model).OrderBy(w => w.Name))
        //                {
        //                    ct.ThrowIfCancellationRequested();
        //                    if (!Pass(wp.Name, nameContains)) continue;

        //                    // EVENTS
        //                    var eventsPart = wp.Parts.Get<Artech.Genexus.Common.Parts.EventsPart>();
        //                    var eventsText = (eventsPart != null && eventsPart.Source != null) ? eventsPart.Source : string.Empty;
        //                    list.Add(new GxObjectDto
        //                    {
        //                        Type = GxObjectType.WebPanel,
        //                        Name = wp.Name,
        //                        Part = "Events",
        //                        Content = eventsText
        //                    });

        //                    // RULES
        //                    var rulesPart = wp.Parts.Get<Artech.Genexus.Common.Parts.RulesPart>();
        //                    var rulesText = string.Empty;
        //                    if (rulesPart != null)
        //                        rulesText = !string.IsNullOrEmpty(rulesPart.Text) ? rulesPart.Text : (rulesPart.Source ?? string.Empty);

        //                    list.Add(new GxObjectDto
        //                    {
        //                        Type = GxObjectType.WebPanel,
        //                        Name = wp.Name,
        //                        Part = "Rules",
        //                        Content = rulesText
        //                    });
        //                }
        //            }
        //        }

        //        return (IReadOnlyList<GxObjectDto>)list;
        //    }, ct);
        //}

        //private static bool Pass(string name, string contains)
        //{
        //    if (string.IsNullOrWhiteSpace(contains)) return true;
        //    return name.IndexOf(contains, StringComparison.OrdinalIgnoreCase) >= 0;
        //}

        //private static void StartBL()
        //{
        //    if (_blStarted) return;

        //    // Config del SDK (coloca genexus.exe.config junto al .exe)
        //    var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //    var cfg = Path.Combine(exeDir, "genexus.exe.config");

        //    ExceptionManager.ConfigurationFile = cfg;
        //    CacheManager.ConfigurationFile = cfg;

        //    // Ajustar versión si cambias de GX
        //    PathHelper.SetAssemblyInfo("Artech", "Genexus", "18");
        //    Connector.StartBL();

        //    _blStarted = true;
        //}

        //private static KnowledgeBase OpenKB(string kbPath)
        //{
        //    if (!Directory.Exists(kbPath))
        //        throw new DirectoryNotFoundException("No existe la KB: " + kbPath);

        //    var options = new KnowledgeBase.OpenOptions(kbPath);
        //    options.EnableMultiUser = false;
        //    return KnowledgeBase.Open(options);
        //}
        public Task<IReadOnlyList<GxObjectDto>> ExtractAsync(string kbPath, bool includeProcedures, bool includeWebPanels, string nameContains, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
