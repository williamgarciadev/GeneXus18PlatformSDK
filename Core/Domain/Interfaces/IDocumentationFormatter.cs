using Acme.Packages.Menu.Core.Domain.DTOs;

namespace Acme.Packages.Menu.Core.Domain.Interfaces
{
    public interface IDocumentationFormatter
    {
        string Format(ObjectDocumentationDto data);
        string FileExtension { get; }
    }
}
