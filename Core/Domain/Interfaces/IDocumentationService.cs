using Acme.Packages.Menu.Core.Domain.DTOs;
using Artech.Architecture.Common.Objects;

namespace Acme.Packages.Menu.Core.Domain.Interfaces
{
    public interface IDocumentationService
    {
        ObjectDocumentationDto ExtractDocumentation(KBObject obj);
    }
}
