using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Artech.Udm.Framework;
using Acme.Packages.Menu.Utilities;
using Artech.Architecture.Common.Objects;

namespace Acme.Packages.Menu.Infrastructure
{
    /// <summary>
    /// Clase base para exportadores que manejan acceso a KB
    /// </summary>
    internal abstract class BaseKBExporter
    {
        protected void ValidateKBAccess(KBModel model, KnowledgeBase kb)
        {
            if (model == null || kb == null)
            {
                throw new InvalidOperationException("No se pudo acceder al modelo o la KB actual.");
            }
        }

        protected string SaveToDesktop(List<string> data, string baseFileName)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = baseFileName.Contains("{timestamp}")
                ? baseFileName.Replace("{timestamp}", timestamp)
                : $"{baseFileName}_{timestamp}.csv";

            var filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                fileName);

            File.WriteAllLines(filePath, data, Encoding.UTF8);
            return filePath;
        }

        protected void ShowSuccessMessage(string filePath, string operation)
        {
            Utils.ShowInfo(
                $"✅ {operation} exportado exitosamente a:\n{filePath}",
                "Exportación completa");
        }
    }
}