using System;
using Artech.Architecture.Common.Objects;
using Artech.Udm.Framework;
using Acme.Packages.Menu.Infrastructure;

namespace Acme.Packages.Menu.Formatters
{
    /// <summary>
    /// Formateador de historial de objetos para CSV
    /// </summary>
    internal class HistoryCsvFormatter
    {
        private readonly KnowledgeBase _kb;
        private const string DEFAULT_DATE_FORMAT = "dd/MM/yyyy HH:mm";
        private const string UNKNOWN_VALUE = "Desconocido";

        public HistoryCsvFormatter(KnowledgeBase kb)
        {
            _kb = kb ?? throw new ArgumentNullException(nameof(kb));
        }

        public string FormatToCsv(KBObject obj, object history)
        {
            var nombre = SanitizeForCsv(obj.Name);
            var tipo = obj.TypeName;
            var fecha = ExtractTimestamp(history);
            var usuario = ResolveUserName(history);
            var operacion = ReflectionHelper.GetPropertyValue(history, "Operation", UNKNOWN_VALUE);
            var version = ReflectionHelper.GetPropertyValue(history, "EntityVersionId", "0");

            return $"{nombre},{tipo},{fecha},{usuario},{operacion},{version}";
        }

        private string SanitizeForCsv(string value)
        {
            return value?.Replace(",", "") ?? string.Empty;
        }

        private string ExtractTimestamp(object history)
        {
            var timestamp = ReflectionHelper.GetPropertyValue<object>(history, "Timestamp");
            if (timestamp == null) return DateTime.Now.ToString(DEFAULT_DATE_FORMAT);

            if (timestamp is DateTime dt)
                return dt.ToLocalTime().ToString(DEFAULT_DATE_FORMAT);

            if (DateTime.TryParse(timestamp.ToString(), out DateTime parsedDate))
                return parsedDate.ToLocalTime().ToString(DEFAULT_DATE_FORMAT);

            return DateTime.Now.ToString(DEFAULT_DATE_FORMAT);
        }

        private string ResolveUserName(object history)
        {
            var userId = ReflectionHelper.GetPropertyValue<object>(history, "HistoryUserId");
            if (userId == null) return UNKNOWN_VALUE;

            try
            {
                if (userId is int)
                {
                    var intUserId = (int)userId;
                    return _kb.LoadKBUser(intUserId)?.Name ?? UNKNOWN_VALUE;
                }
                else if (userId is Guid)
                {
                    var guidUserId = (Guid)userId;
                    return $"User-{guidUserId.ToString().Substring(0, 8)}";
                }
                else if (int.TryParse(userId.ToString(), out int parsedUserId))
                {
                    return _kb.LoadKBUser(parsedUserId)?.Name ?? UNKNOWN_VALUE;
                }
                else
                {
                    return userId.ToString();
                }
            }
            catch
            {
                return UNKNOWN_VALUE;
            }
        }
    }
}