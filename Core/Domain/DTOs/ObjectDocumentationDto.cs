using System;
using System.Collections.Generic;

namespace Acme.Packages.Menu.Core.Domain.DTOs
{
    public class ParameterDocumentationDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Access { get; set; } // in:, out:, inout:
        public string Description { get; set; }
    }

    public class VariableDocumentationDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }

    public class ObjectDocumentationDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Module { get; set; }
        public List<ParameterDocumentationDto> Parameters { get; set; } = new List<ParameterDocumentationDto>();
        public List<VariableDocumentationDto> Variables { get; set; } = new List<VariableDocumentationDto>();
        public DateTime LastModified { get; set; }
        public string KBName { get; set; }
    }
}
