using Artech.Architecture.Common.Objects;

namespace Acme.Packages.Menu.Core.Domain.Interfaces
{
    public interface IVariableCleanerService
    {
        /// <summary>
        /// Analiza y elimina las variables no utilizadas del objeto proporcionado.
        /// </summary>
        /// <param name="obj">Objeto GeneXus a limpiar.</param>
        /// <returns>Número de variables eliminadas.</returns>
        int CleanUnusedVariables(KBObject obj);
    }
}
