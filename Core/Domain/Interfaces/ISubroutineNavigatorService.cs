using Artech.Architecture.Common.Objects;

namespace Acme.Packages.Menu.Core.Domain.Interfaces
{
    public interface ISubroutineNavigatorService
    {
        /// <summary>
        /// Busca la línea donde se define una subrutina dentro del código de una parte.
        /// </summary>
        /// <param name="sourceCode">El código fuente de la parte actual.</param>
        /// <param name="subroutineName">El nombre de la subrutina a buscar.</param>
        /// <returns>Número de línea (1-based) o -1 si no se encuentra.</returns>
        int FindDefinitionLine(string sourceCode, string subroutineName);

        /// <summary>
        /// Limpia el nombre de la subrutina si viene con el comando 'Do' o comillas.
        /// </summary>
        string CleanSubroutineName(string selectedText);
    }
}
