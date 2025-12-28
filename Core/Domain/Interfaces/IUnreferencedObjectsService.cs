using Artech.Architecture.Common.Objects;
using System.Collections.Generic;

namespace Acme.Packages.Menu.Core.Domain.Interfaces
{
    public interface IUnreferencedObjectsService
    {
        List<KBObject> GetUnreferencedObjects(KBModel model);
        int DeleteUnreferencedObjects(List<KBObject> objectsToDelete);
    }
}