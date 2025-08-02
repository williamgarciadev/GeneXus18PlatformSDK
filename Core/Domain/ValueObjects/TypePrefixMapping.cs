using System.Collections.Generic;
using Artech.Genexus.Common;

namespace Acme.Packages.Menu.Core.Domain.ValueObjects
{
    public static class TypePrefixMapping
    {
        private static readonly Dictionary<char, (eDBType Type, int DefaultLength)> PrefixMappings = 
            new Dictionary<char, (eDBType, int)>
            {
                ['N'] = (eDBType.NUMERIC, 10),
                ['V'] = (eDBType.VARCHAR, 50),
                ['C'] = (eDBType.CHARACTER, 20),
                ['D'] = (eDBType.DATE, 0),
                ['T'] = (eDBType.DATETIME, 0),
                ['B'] = (eDBType.Boolean, 0),
                ['S'] = (eDBType.GX_SDT, 0),
                ['L'] = (eDBType.LONGVARCHAR, 2000),
                ['G'] = (eDBType.GUID, 36)
            };

        public static (eDBType Type, int DefaultLength) GetTypeFromPrefix(char prefix)
        {
            return PrefixMappings.TryGetValue(prefix, out var mapping) 
                ? mapping 
                : (eDBType.NONE, 0);
        }

        public static bool IsValidPrefix(char prefix)
        {
            return PrefixMappings.ContainsKey(prefix);
        }
    }
}