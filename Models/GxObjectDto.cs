using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.Packages.Menu.Models
{
    public sealed class GxObjectDto
    {
        public GxObjectType Type { get; set; }
        public string Name { get; set; } = "";
        public string Part { get; set; } = "";   // "Source", "Events", "Rules"
        public string Content { get; set; } = "";

        public int LineCount
        {
            get
            {
                return string.IsNullOrEmpty(Content) ? 0 : Content.Split('\n').Length;
            }
        }
    }
}
