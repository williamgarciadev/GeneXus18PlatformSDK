using System.Collections.Generic;
using Artech.Architecture.Common.Objects;
using Acme.Packages.Menu.Models;

namespace Acme.Packages.Menu.Core.Domain.Interfaces
{
    public interface ISmartVariableService
    {
        /// <summary>
        /// Escanea el objeto en busca de variables usadas (&var) que no estén definidas.
        /// </summary>
        List<VariableInfo> GetUndefinedVariables(KBObject obj);

        /// <summary>
        /// Crea las variables seleccionadas en el objeto.
        /// </summary>
        void DefineVariables(KBObject obj, List<VariableInfo> variables);
    }
}
