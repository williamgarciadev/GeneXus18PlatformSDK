using Artech.Architecture.Common.Services;
using System;
using System.IO;
using System.Windows.Forms;

namespace Acme.Packages.Menu
{
    public static class LogService
    {
        public static void ShowWarning(string message, string title = "Advertencia")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ShowInfo(string message, string title = "Información")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void LogOutput(string message, string level = "INFO")
        {
            CommonServices.Output.AddLine($"[{level}] {message}");
            Console.WriteLine($"[{level}] {message}");
        }

        public static void LogToFile(string path, string message)
        {
            try
            {
                File.AppendAllText(path, $"{DateTime.Now}: {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                LogOutput($"Error writing log to file: {ex.Message}", "ERROR");
            }
        }
    }
}
