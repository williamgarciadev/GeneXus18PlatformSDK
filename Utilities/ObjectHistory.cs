using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.Packages.Menu.Utilities
{
    public static class ObjectHistory
    {
        private static readonly List<string> _entries = new List<string>();

        public static void Add(string entry)
        {
            _entries.Add($"{DateTime.Now:HH:mm:ss} - {entry}");
        }

        public static List<string> GetAll() => _entries;

        public static void Clear() => _entries.Clear();
    }
}

