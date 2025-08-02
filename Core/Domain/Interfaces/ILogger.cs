namespace Acme.Packages.Menu.Core.Domain.Interfaces
{
    public interface ILogger
    {
        void Log(string message);
        void LogError(string message);
        void LogWarning(string message);
        void LogSuccess(string message);
    }
}