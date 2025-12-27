using System.Collections.Generic;
using Artech.Architecture.Common.Objects;
using Acme.Packages.Menu.Core.Domain.DTOs;

namespace Acme.Packages.Menu.Core.Domain.Interfaces
{
    public interface IVariableTracerService
    {
        /// <summary>
        /// Busca todas las apariciones de una variable en el objeto y las clasifica.
        /// </summary>
        List<VariableOccurrenceDto> GetOccurrences(KBObject obj, string variableName);
    }
}
