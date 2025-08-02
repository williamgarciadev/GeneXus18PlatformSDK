using Acme.Packages.Menu.Core.Domain.Interfaces;
using Acme.Packages.Menu.Utilities;

namespace Acme.Packages.Menu.Core.Infrastructure.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Utils.Log(message);
        }

        public void LogError(string message)
        {
            Utils.Log(string.Format("ERROR: {0}", message));
        }

        public void LogWarning(string message)
        {
            Utils.Log(string.Format("WARNING: {0}", message));
        }

        public void LogSuccess(string message)
        {
            Utils.Log(string.Format("SUCCESS: {0}", message));
        }
    }
}