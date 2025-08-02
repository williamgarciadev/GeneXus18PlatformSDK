using Acme.Packages.Menu.Core.Domain.Interfaces;
using Artech.Architecture.Common.Services;

namespace Acme.Packages.Menu.Core.Infrastructure.External
{
    public class GeneXusLogger : ILogger
    {
        public void Log(string message)
        {
            CommonServices.Output.AddLine(message);
        }

        public void LogError(string message)
        {
            CommonServices.Output.AddLine(string.Format("❌ {0}", message));
        }

        public void LogWarning(string message)
        {
            CommonServices.Output.AddLine(string.Format("⚠️ {0}", message));
        }

        public void LogSuccess(string message)
        {
            CommonServices.Output.AddLine(string.Format("✅ {0}", message));
        }
    }
}