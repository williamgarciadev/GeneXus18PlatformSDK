using Artech.Genexus.Common;

namespace Acme.Packages.Menu.Core.Domain.Interfaces
{
    public interface ITypeResolver
    {
        (eDBType Type, int Length, bool Found) ResolveTypeFromReference(string reference);
    }
}