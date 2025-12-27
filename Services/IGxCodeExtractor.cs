using Acme.Packages.Menu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Acme.Packages.Menu.Services
{
    public interface IGxCodeExtractor
    {
        Task<IReadOnlyList<GxObjectDto>> ExtractAsync(
            string kbPath,
            bool includeProcedures,
            bool includeWebPanels,
            string nameContains,
            CancellationToken ct);
    }
}
