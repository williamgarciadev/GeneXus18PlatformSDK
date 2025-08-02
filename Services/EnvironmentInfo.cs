using Artech.Architecture.UI.Framework.Services;

namespace Acme.Packages.Menu
{
    public static class EnvironmentInfo
    {
        public static string GetEnvironmentName()
        {
            var environment = UIServices.KB?.CurrentModel?.Environment;
            return environment?.TargetName ?? "Unknown Environment";
        }

        public static string GetModelName()
        {
            return UIServices.KB?.CurrentModel?.Name ?? "Unknown Model";
        }

        public static string GetKBDirectory()
        {
            return UIServices.KB?.CurrentKB?.Location ?? "Unknown Directory";
        }
    }
}
