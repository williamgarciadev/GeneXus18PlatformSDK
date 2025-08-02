using System;
using System.Collections.Generic;
using System.Linq;
using Artech.Architecture.UI.Framework.Services;
using Artech.Architecture.Common.Objects;
using Artech.Udm.Framework;
using Acme.Packages.Menu.Infrastructure;
using Acme.Packages.Menu.Formatters;

namespace Acme.Packages.Menu.Services.Export
{
    /// <summary>
    /// Maneja la exportación del historial de objetos de la KB
    /// </summary>
    internal class ObjectHistoryExporter : BaseKBExporter
    {
        public void ExportToCSV()
        {
            var model = UIServices.KB.CurrentModel;
            var kb = model.KB;

            ValidateKBAccess(model, kb);

            var csvData = GenerateHistoryData(model, kb);
            var filePath = SaveToDesktop(csvData, "HistorialObjetos.csv");

            ShowSuccessMessage(filePath, "Historial");
        }

        private List<string> GenerateHistoryData(KBModel model, KnowledgeBase kb)
        {
            var lines = new List<string>
            {
                "Objeto,Tipo,Fecha,Usuario,Operación,Versión"
            };

            foreach (Guid objType in KnowledgeBase.GetKBObjectTypes())
            {
                ProcessObjectType(model, kb, objType, lines);
            }

            return lines;
        }

        private void ProcessObjectType(KBModel model, KnowledgeBase kb, Guid objType, List<string> lines)
        {
            var entities = kb.GetEntitiesByModelTypeOrderByName(model, objType);

            foreach (Entity entity in entities)
            {
                var obj = KBObject.Get(model, entity.Key);
                if (obj == null) continue;

                ProcessObjectHistory(kb, model, obj, lines);
            }
        }

        private void ProcessObjectHistory(KnowledgeBase kb, KBModel model, KBObject obj, List<string> lines)
        {
            var historyList = kb.GetModelEntityHistoryByModelEntityKey(model, obj.Key);
            if (historyList == null || !historyList.Any()) return;

            foreach (var history in historyList.OrderByDescending(h => h.Timestamp))
            {
                var csvLine = FormatHistoryToCsv(obj, history, kb);
                lines.Add(csvLine);
            }
        }

        private string FormatHistoryToCsv(KBObject obj, object history, KnowledgeBase kb)
        {
            var csvFormatter = new HistoryCsvFormatter(kb);
            return csvFormatter.FormatToCsv(obj, history);
        }
    }
}